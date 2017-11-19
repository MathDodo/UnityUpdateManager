using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Derive from this class to get the default implementation of the interface IInitializeMain.
/// </summary>
/// <typeparam name="T">This is used to make sure the derived class is implementing IInitializeMain</typeparam>
[DisallowMultipleComponent]
public abstract class MainComponent<T> : GameComponent where T : MainComponent<T>, IInitializeMain
{
    [NonSerialized] //These bools handle the updating of this main and the connected subs, if they are false it wont run
    private bool updating, lateUpdating, fixedUpdating;

    [NonSerialized] //These are the ids for the update system which this main is attached to.
    private string updateID, lateUpdateID, fixedUpdateID;

    [SerializeField] //The mark for the scene this gameobject was build with, this can be used to restrict which scene this main object can be in.
    private SceneMarker scene = SceneMarker.All;

    [SerializeField, Tooltip("The starting subs Init method will be called with this as args, no need to use it because it will already have a reference to main."),
        ObjectRestriction(typeof(IInitializeSub), SearchForm.HierarchyOnly, CollectionMode.List)] //This is all the subs attached to this main from the start
    private List<Component> startingSubs = new List<Component>();

    [NonSerialized] //This field is to have a direct reference to the child class
    private T thisIsMe;

    [NonSerialized] //UnityEngine.Object instance id
    private int instanceID;

    //These actions will be run when the method Invoke then the action name is called by the world manager, so these are reference to the derived class' methods
    private Action update, lateUpdate, fixedUpdate;

    //This is run before the main is destroyed it will always have a value because of the SceneObserver
    private Action<IInitializeMain> beforeMainDestroyed;

    //These actions are run to give or remove references to subs on this main
    private Action<IInitializeSub> afterSubAdded, beforeSubRemoved;

    //These are the lists for subs on this main, two of the lists only holds subs in one frame
    private List<IInitializeSub> subsToAdd, subsToRemove, initializeSubs;

    /// <summary>
    /// Reference to this, default implementation from the interface IInitializeMain, so you can have a component reference to the main if needed.
    /// </summary>
    public Component This
    {
        get
        {
            if (thisIsMe == null)
            {
                thisIsMe = (T)this;
            }

            return thisIsMe;
        }
    }

    /// <summary>
    /// The scene of this main, implemented through the interface IInitializeMain
    /// </summary>
    public SceneMarker Scene { get { return scene; } }

    /// <summary>
    /// The instance id of this initialize needs to be run first, this is implemented through IInitializeMain by IInitialize.
    /// </summary>
    public int InstanceID { get { return instanceID; } }

    /// <summary>
    /// This is a readonlycollection to get all initialize subs in this main component, this is implemented through IInitializeMain.
    /// If you just want to get references use GetSubComponent<T>(), GetSubComponents<T>() or AddAndOrGetSubComponent<T>().
    /// </summary>
    public ReadOnlyCollection<IInitializeSub> InitializeSubs { get { return initializeSubs.AsReadOnly(); } }

    /// <summary>
    /// This action is invoked right after a sub has been added to this main, this is implemented through IInitializeMain.
    /// You do need to use the methods AddInitializeSub(IInitializeSub sub, int priority, bool inInspector = false),
    /// AddAndOrGetSubComponent<T>() where a sub is added or CreateSubComponentAndAddToGameObject<T>() for invokation.
    /// </summary>
    public Action<IInitializeSub> AfterSubAdded { get { return afterSubAdded; } set { afterSubAdded = value; } }

    /// <summary>
    /// This action is invoked just before a sub is removed from this main, this is implemented through IInitializeMain.
    /// You do need to use the RemoveInitializeSub(IInitializeSub sub) for the invokation.
    /// </summary>
    public Action<IInitializeSub> BeforeSubRemoved { get { return beforeSubRemoved; } set { beforeSubRemoved = value; } }

    /// <summary>
    /// This is invoked when this is destroyed, mainly for everyone who has a reference to this, this is implemented through IInitializeMain.
    /// You need to use the methods DestroyThis() or DestroyGameObject() for invokation.
    /// </summary>
    public Action<IInitializeMain> BeforeMainDestroyed { get { return beforeMainDestroyed; } set { beforeMainDestroyed = value; } }

    /// <summary>
    /// This method is called right before Init and is implemented through the IInitialize interface, only let the system call this method
    /// </summary>
    /// <param name="sceneObserver">The observer this was build with.</param>
    public void Initialize(SceneObserver sceneObserver)
    {
        thisIsMe = (T)this;
        scene = sceneObserver.Scene;
        instanceID = GetInstanceID();
        this.sceneObserver = sceneObserver;
        subsToAdd = new List<IInitializeSub>();
        subsToRemove = new List<IInitializeSub>();
        initializeSubs = new List<IInitializeSub>();

        for (int i = 0; i < startingSubs.Count; i++)
        {
            IInitializeSub sub = (IInitializeSub)startingSubs[i];
            sub.Initialize(sceneObserver);
            sub.Init(this);
            initializeSubs.Add(sub);
        }

        startingSubs.Clear();
    }

    /// <summary>
    /// Use this method if you are unsure if this main has a subcomponent of type T1 attached to this main, if it doesnt it will add a component of that type to the gameobject.
    /// This method is implemented through IInitializeMain.
    /// </summary>
    /// <typeparam name="T1">Type of the sub component</typeparam>
    /// <typeparam name="T2">The args if needed to create a new sub component.</typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual T1 AddAndOrGetSubComponent<T1, T2>(T2 args) where T1 : class, IInitializeSub
    {
        T1 sub = default(T1);

        if ((sub = (T1)initializeSubs.Find(i => i.ThisType == typeof(T1))) == null)
        {
            sub = (gameObject.AddComponent(typeof(T1)) as T1);
            sub.Initialize(sceneObserver);
            sub.Init(args);
            subsToAdd.Add(sub);
        }

        return sub;
    }

    /// <summary>
    /// Use this method to get a component of type T1 which is attached to this main, null will be returned if the main doesnt have a subcomponent like that attached.
    /// This method is implemented through IInitializeMain.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    public virtual T1 GetSubComponent<T1>() where T1 : IInitializeSub
    {
        return (T1)initializeSubs.Find(i => i.ThisType == typeof(T1));
    }

    /// <summary>
    /// This method will create a subcomponent of T1 type and add it to the gameobject.
    /// This method is implemented through IInitializeMain.
    /// </summary>
    /// <typeparam name="T1">The type of the sub component.</typeparam>
    /// <typeparam name="T2">The type of args given to the new subcomponent.</typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual T1 CreateSubComponentAndAddToGameObject<T1, T2>(T2 args) where T1 : Component, IInitializeSub
    {
        T1 sub = gameObject.AddComponent<T1>();
        sub.Initialize(sceneObserver);
        sub.Init(args);
        subsToAdd.Add(sub);
        return sub;
    }

    /// <summary>
    /// This method returns all subcomponents of type T1 which are attached to this main.
    /// This method is implemented through IInitializeMain.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    public virtual T1[] GetSubComponents<T1>() where T1 : IInitializeSub
    {
        var arraySubs = initializeSubs.Where(i => i.ThisType == typeof(T1)).ToArray();
        T1[] returned = new T1[arraySubs.Length];

        for (int i = 0; i < arraySubs.Length; i++)
        {
            returned[i] = (T1)arraySubs[i];
        }

        return returned;
    }

    /// <summary>
    /// This method will add an initialize sub to this main, doesnt need to be a component.
    /// This method is implemented through IInitializeMain.
    /// </summary>
    /// <param name="sub">The sub that is to be added.</param>
    /// <param name="priority">The priority of the sub.</param>
    /// <param name="inInspector">Just let this be with the default value.</param>
    /// <param name="args">The value to be send to the Init method in the sub.</param>
    public virtual void AddInitializeSub(IInitializeSub sub, int priority, bool inInspector = false, object args = null)
    {
        if (inInspector)
        {
            if (startingSubs.Count < priority)
            {
                startingSubs.Add((Component)sub);
            }
            else
            {
                startingSubs.Insert(priority - 1, (Component)sub);
            }
        }
        else
        {
            if (subsToAdd.Count < priority)
            {
                subsToAdd.Add(sub);
            }
            else
            {
                subsToAdd.Insert(priority - 1, sub);
            }

            sub.Initialize(sceneObserver);
            sub.Init(args);
        }
    }

    public virtual void RemoveInitializeSub(IInitializeSub sub)
    {
        subsToRemove.Add(sub);
    }

    public override void DestroyGameObject()
    {
        beforeMainDestroyed.Invoke(thisIsMe);

        if (beforeDestroyed != null)
        {
            beforeDestroyed.Invoke();
        }

        if (fixedUpdate != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeFixedUpdate, fixedUpdateID, UpdateReceiver.FixedUpdate);
        }

        if (update != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeUpdate, updateID, UpdateReceiver.Update);
        }

        if (lateUpdate != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeLateUpdate, lateUpdateID, UpdateReceiver.LateUpdate);
        }

        Destroy(gameObject);
    }

    public override void DestroyThis()
    {
        beforeMainDestroyed.Invoke(thisIsMe);

        if (beforeDestroyed != null)
        {
            beforeDestroyed.Invoke();
        }

        if (fixedUpdate != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeFixedUpdate, fixedUpdateID, UpdateReceiver.FixedUpdate);
        }

        if (update != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeUpdate, updateID, UpdateReceiver.Update);
        }

        if (lateUpdate != null)
        {
            UpdateManager.Instance.RemoveFromUpdateEvent(InvokeLateUpdate, lateUpdateID, UpdateReceiver.LateUpdate);
        }

        for (int i = 0; i < initializeSubs.Count; i++)
        {
            Destroy(initializeSubs[i].This);
        }

        Destroy(this);
    }

    public override void DisableGameObject()
    {
        gameObject.SetActive(false);

        if (afterDisabled != null)
        {
            afterDisabled.Invoke();
        }

        updating = !updating;
        lateUpdating = !lateUpdating;
        fixedUpdating = !fixedUpdating;
    }

    public override void DisableThis()
    {
        enabled = false;

        if (afterDisabled != null)
        {
            afterDisabled.Invoke();
        }

        updating = !updating;
        lateUpdating = !lateUpdating;
        fixedUpdating = !fixedUpdating;
    }

    public override void EnableGameObject()
    {
        updating = !updating;
        lateUpdating = !lateUpdating;
        fixedUpdating = !fixedUpdating;

        gameObject.SetActive(true);

        if (afterEnabled != null)
        {
            afterEnabled.Invoke();
        }
    }

    public override void EnableThis()
    {
        updating = !updating;
        lateUpdating = !lateUpdating;
        fixedUpdating = !fixedUpdating;

        enabled = true;

        if (afterEnabled != null)
        {
            afterEnabled.Invoke();
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// This method is only called when a button is clicked in the inspector it wont work if the game is running and it wont exist in builds
    /// </summary>
    public override void UpdateStartingSubsList()
    {
        if (!Application.isPlaying)
        {
            var array = GetComponentsInChildren<IInitializeSub>();

            startingSubs.Clear();

            for (int i = 0; i < array.Length; i++)
            {
                AddInitializeSub(array[i], array[i].Priority, true);
            }
        }
    }

#endif

    protected virtual void SubscribeToUpdate(Action method)
    {
        initializeSubs.OrderBy(i => i.Priority);
        updating = true;
        updateID = UpdateManager.Instance.SubscribeToUpdateEvent(InvokeUpdate, UpdateReceiver.Update);
        update = method;
    }

    protected virtual void SubscribeToLateUpdate(Action method)
    {
        initializeSubs.OrderBy(i => i.Priority);
        lateUpdating = true;
        lateUpdateID = UpdateManager.Instance.SubscribeToUpdateEvent(InvokeLateUpdate, UpdateReceiver.LateUpdate);
        lateUpdate = method;
    }

    protected virtual void SubscribeToFixedUpdate(Action method)
    {
        initializeSubs.OrderBy(i => i.Priority);
        fixedUpdating = true;
        fixedUpdateID = UpdateManager.Instance.SubscribeToUpdateEvent(InvokeFixedUpdate, UpdateReceiver.FixedUpdate);
        fixedUpdate = method;
    }

#if UNITY_EDITOR

    protected virtual void Reset()
    {
        if (!Application.isPlaying)
        {
            if (transform != transform.root)
            {
                Debug.LogError("The component with IInitializeMain needs to be on the top gameobject in hierarchy");
                Debug.LogError(GetType().ToString() + " will be removed from " + gameObject.name);

                WarningWindow.ShowWindow("The component with IInitializeMain needs to be on the top gameobject in hierarchy", this);
            }
        }
    }

#endif

    private void RemoveSubs()
    {
        if (subsToRemove.Count > 0)
        {
            if (beforeSubRemoved != null)
            {
                for (int i = 0; i < subsToRemove.Count; i++)
                {
                    beforeSubRemoved.Invoke(subsToRemove[i]);

                    initializeSubs.Remove(subsToRemove[i]);
                }
            }
            else
            {
                for (int i = 0; i < subsToRemove.Count; i++)
                {
                    initializeSubs.Remove(subsToRemove[i]);
                }
            }

            subsToRemove.Clear();
        }
    }

    private void AddSubs()
    {
        if (subsToAdd.Count > 0)
        {
            if (afterSubAdded != null)
            {
                for (int i = 0; i < subsToAdd.Count; i++)
                {
                    var sub = subsToAdd[i];
                    initializeSubs.Add(sub);
                    afterSubAdded.Invoke(sub);
                }
            }
            else
            {
                for (int i = 0; i < subsToAdd.Count; i++)
                {
                    initializeSubs.Add(subsToAdd[i]);
                }
            }

            subsToAdd.Clear();
        }
    }

    private void InvokeFixedUpdate()
    {
        RemoveSubs();
        AddSubs();

        if (fixedUpdating)
        {
            fixedUpdate.Invoke();

            for (int i = 0; i < initializeSubs.Count; i++)
            {
                initializeSubs[i].InvokeFixedUpdating();
            }
        }
    }

    private void InvokeUpdate()
    {
        RemoveSubs();
        AddSubs();

        if (updating)
        {
            update.Invoke();

            for (int i = 0; i < initializeSubs.Count; i++)
            {
                initializeSubs[i].InvokeUpdating();
            }
        }
    }

    private void InvokeLateUpdate()
    {
        RemoveSubs();
        AddSubs();

        if (lateUpdating)
        {
            lateUpdate.Invoke();

            for (int i = 0; i < initializeSubs.Count; i++)
            {
                initializeSubs[i].InvokeLateUpdating();
            }
        }
    }
}