using System;
using System.Collections.Generic;
using UnityEngine;

internal abstract class SubComponent<T> : GameComponent where T : SubComponent<T>, IInitializeSub
{
    [SerializeField, Tooltip("If more than one has the same priority then it is the order they are added"), Range(1, 15)]
    private int priority = -1;

    [NonSerialized]
    private bool fixedUpdating, updating, lateUpdating;

    private T thisIsMe;
    private int instanceID;
    private Action update, lateUpdate, fixedUpdate;

    protected IInitializeMain main;

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

    public int Priority { get { return priority; } }

    /// <summary>
    /// The instance id of this initialize needs to be run first, this is implemented through IInitializeMain by IInitialize.
    /// </summary>
    public int InstanceID { get { return instanceID; } }

    public IInitializeMain Main { get { return main; } }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sceneObserver">The observer this was build with.</param>
    public void Initialize(SceneObserver sceneObserver)
    {
        thisIsMe = (T)this;
        instanceID = GetInstanceID();
        this.sceneObserver = sceneObserver;
        main = transform.root.GetComponent<IInitializeMain>();
    }

    public override void DestroyGameObject()
    {
        if (gameObject == main.This.gameObject)
        {
            main.DestroyGameObject();
        }
        else
        {
            main.RemoveInitializeSub(thisIsMe);

            if (beforeDestroyed != null)
            {
                beforeDestroyed.Invoke();
            }

            Destroy(gameObject);
        }
    }

    public override void DestroyThis()
    {
        main.RemoveInitializeSub(thisIsMe);

        if (beforeDestroyed != null)
        {
            beforeDestroyed.Invoke();
        }

        Destroy(this);
    }

    public override void DisableGameObject()
    {
        if (gameObject == main.This.gameObject)
        {
            main.DisableGameObject();
        }
        else
        {
            main.RemoveInitializeSub(thisIsMe);

            gameObject.SetActive(false);

            if (afterDisabled != null)
            {
                afterDisabled.Invoke();
            }
        }
    }

    public override void DisableThis()
    {
        main.RemoveInitializeSub(thisIsMe);

        enabled = false;

        if (afterDisabled != null)
        {
            afterDisabled.Invoke();
        }
    }

    public override void EnableGameObject()
    {
        main.AddInitializeSub(thisIsMe, priority);

        gameObject.SetActive(true);

        if (afterEnabled != null)
        {
            afterEnabled.Invoke();
        }
    }

    public override void EnableThis()
    {
        main.AddInitializeSub(thisIsMe, priority);

        enabled = true;

        if (afterEnabled != null)
        {
            afterEnabled.Invoke();
        }
    }

    /// <summary>
    /// This method invokes your fixed updates if needed, ONLY LET THE MAIN OF THIS SUB CALL THIS METHOD.
    /// </summary>
    public void InvokeFixedUpdating()
    {
        if (fixedUpdating)
        {
            fixedUpdate.Invoke();
        }
    }

    /// <summary>
    /// This method invokes your updates if needed, ONLY LET THE MAIN OF THIS SUB CALL THIS METHOD.
    /// </summary>
    public void InvokeUpdating()
    {
        if (updating)
        {
            update.Invoke();
        }
    }

    /// <summary>
    /// This method invokes your late updates if needed, ONLY LET THE MAIN OF THIS SUB CALL THIS METHOD.
    /// </summary>
    public void InvokeLateUpdating()
    {
        if (lateUpdating)
        {
            lateUpdate.Invoke();
        }
    }

    protected void SubscribeToUpdate(Action method, UpdateReceiver updateReceiver)
    {
        switch (updateReceiver)
        {
            case UpdateReceiver.FixedUpdate:

                fixedUpdating = true;
                fixedUpdate += method;

                break;

            case UpdateReceiver.Update:

                updating = true;
                update += method;

                break;

            case UpdateReceiver.LateUpdate:

                lateUpdating = true;
                lateUpdate += method;

                break;
        }
    }

#if UNITY_EDITOR

    protected virtual void Reset()
    {
        if (!Application.isPlaying)
        {
            if (transform.root.GetComponent<IInitializeMain>() == null)
            {
                Debug.LogError("There needs to be an IInitializeMain component on the top parent for an IInitializeSub component");
                Debug.LogError(GetType().ToString() + " will be removed from " + gameObject.name);

                WarningWindow.ShowWindow("There needs to be an IInitializeMain component on the top parent for an IInitializeSub component", this);
            }

            List<IInitializeSub> list = new List<IInitializeSub>();
            list.AddRange(transform.root.GetComponentsInChildren<IInitializeSub>());
            priority = list.Count;

            if (transform.root.GetComponent<IInitializeMain>() != null)
            {
                transform.root.GetComponent<IInitializeMain>().AddInitializeSub((IInitializeSub)this, priority, true);
            }
        }
    }

#endif
}