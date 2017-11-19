using UnityEngine;

/// <summary>
/// This interface enables objectpooling. Inherit from PoolableComponent to get default implementation, it already implements this interface.
/// </summary>
public interface IPoolableUnityObject : IInitializeMain
{
    /// <summary>
    /// Only let the objectpool change this.
    /// </summary>
    int PrefabID { get; set; }

    /// <summary>
    /// Return the component's gameobject.
    /// </summary>
    GameObject GameObject { get; }

    /// <summary>
    /// This is called by the ObjectPool, when this is spawned, after Init the first time.
    /// </summary>
    void OnSpawned();

    /// <summary>
    /// This is called by the ObjectPool, when this is despawned.
    /// </summary>
    void OnDespawned();
}