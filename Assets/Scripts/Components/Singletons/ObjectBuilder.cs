using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectBuilder", menuName = "ObjectBuilder")]
public class ObjectBuilder : ScriptableObjectSingleton<ObjectBuilder>, ISingleton
{
    /// <summary>
    /// Only let the Singletons class call this method
    /// </summary>
    public void Init()
    {
    }

    /// <summary>
    /// Use this method to instantiate IInitializeMain Objects
    /// </summary>
    /// <typeparam name="T">Reference to the asset you want instantiated.</typeparam>
    /// <typeparam name="T1">The type of the value to give to the main.</typeparam>
    /// <param name="asset">Reference to the prefab or ScriptableObject to be instantiated.</param>
    /// <param name="args">The value given to the instantiated main.</param>
    /// <param name="observer">Reference to the sceneobserver</param>
    /// <returns></returns>
    internal T Create<T, T1>(T asset, T1 args, SceneObserver observer) where T : UnityEngine.Object
    {
        IInitializeMain mainI = (IInitializeMain)asset;

        if (mainI.Scene == SceneMarker.All || mainI.Scene == observer.Scene)
        {
            T instance = Instantiate(asset);
            var main = (IInitializeMain)instance;
            main.Initialize(observer);
            main.Init(args);
            observer.AddMain(main);

            return instance;
        }
        else
        {
            Debug.LogError("The asset " + asset + " cant be instantiated in this scene because it is Scenebound");
            return null;
        }
    }
}