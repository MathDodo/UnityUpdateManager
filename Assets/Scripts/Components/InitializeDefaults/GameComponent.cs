using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// This class is the baseclass for the initialize components, it holds the basic method names for the IInitialize interface
/// </summary>
public abstract class GameComponent : UnityEngine.MonoBehaviour
{
    [NonSerialized] //The sceneobserver this GameComponent was build with
    protected SceneObserver sceneObserver;

    //These are the actions for the IInitialize interface implementation
    protected Action afterEnabled, afterDisabled, beforeDestroyed;

    /// <summary>
    /// This Action is invoked each time this component is enabled, this is implemented through IInitializeMain by IInitialize.
    /// You do need to use the methods, EnableThis() or EnableGameObject for the invokation.
    /// </summary>
    public Action AfterEnabled { get { return afterEnabled; } set { afterEnabled = value; } }

    /// <summary>
    /// This Action is invoked each time this component is disabled, this is implemented through IInitializeMain by IInitialize.
    /// You do need to use the methods, DisableThis() or DisableGameObject() for the invokation.
    /// </summary>
    public Action AfterDisabled { get { return afterDisabled; } set { afterDisabled = value; } }

    /// <summary>
    /// This action is invoked before this or this gameobject is destroyed, this is implemented through IInitializeMain by IInitialize.
    /// You do need to use the methods, DestroyThis() or DestroyGameObject() for the invokation.
    /// </summary>
    public Action BeforeDestroyed { get { return beforeDestroyed; } set { beforeDestroyed = value; } }

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void EnableThis();

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void DestroyThis();

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void DisableThis();

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void EnableGameObject();

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void DisableGameObject();

    /// <summary>
    /// Abstract method for forced implementation, made for the IInitialize interface.
    /// </summary>
    public abstract void DestroyGameObject();

#if UNITY_EDITOR

    /// <summary>
    /// This is a standin method for maincomponent, only available in the editor
    /// </summary>
    public virtual void UpdateStartingSubsList() { }

#endif
}