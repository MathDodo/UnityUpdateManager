using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class Singletons
{
    private static List<ISingleton> instances = new List<ISingleton>();
    private static List<ISingleton> singletonResources = new List<ISingleton>();

    static Singletons()
    {
        var array = Resources.LoadAll("ISingletons", typeof(ISingleton));

        for (int i = 0; i < array.Length; i++)
        {
            singletonResources.Add((ISingleton)array[i]);
        }
    }

    internal static T GetInstance<T>() where T : UnityEngine.Object, ISingleton
    {
        T instance;

        if ((instance = (T)instances.Find(i => i.TypeOfThis == typeof(T))) != null)
        {
            return instance;
        }
        else
        {
            instance = (UnityEngine.Object.Instantiate((UnityEngine.Object)singletonResources.Find(i => i.TypeOfThis == typeof(T))) as T);
            instances.Add(instance);
            instance.Init();
            UnityEngine.Object.DontDestroyOnLoad(instance);

            return instance;
        }
    }
}