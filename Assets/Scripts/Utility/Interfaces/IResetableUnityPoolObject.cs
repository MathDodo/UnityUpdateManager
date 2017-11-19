using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When implementing this interface by default through GameComponent reflection will be used.
/// You need to use the attribute ResetField for this to work. Inherit from ResetablePoolComponent to get default implementation.
/// </summary>
public interface IResetableUnityPoolObject : IPoolableUnityObject
{
    /// <summary>
    /// Called for getting references to the marked fields, called after OnSpawn only once.
    /// </summary>
    void ResetSetup();

    /// <summary>
    /// This method resets the fieldvalues, and is run after OnDespawned.
    /// </summary>
    void RunReset();
}