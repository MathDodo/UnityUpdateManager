using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class PoolableComponent : MainComponent<PoolableComponent>, IPoolableUnityObject
{
    /// <summary>
    /// Only let the objectpool change this. The object pool uses this and it is implemented through IPoolableUnityObject.
    /// </summary>
    public int PrefabID { get; set; }

    /// <summary>
    /// Return the component's gameobject. The object pool uses this and it is implemented through IPoolableUnityObject.
    /// </summary>
    public GameObject GameObject { get { return gameObject; } }

    public abstract void Init<T>(T args);
    public abstract void OnSpawned();
    public abstract void OnDespawned();
}