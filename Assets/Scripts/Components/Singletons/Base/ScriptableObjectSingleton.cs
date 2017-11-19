using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>, ISingleton
{
    private Type thisType = typeof(T);

    public Type TypeOfThis { get { return thisType; } }

    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = Singletons.GetInstance<T>();
            }

            return instance;
        }
    }

#if UNITY_EDITOR

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            UnityEngine.Object[] obj = Resources.LoadAll("ISingletons", thisType);

            if (obj.Length == 0)
            {
                Debug.Log(thisType + " have been created.");
            }

            if (obj.Length > 0 && obj[0] != this)
            {
                WarningWindow.ShowWindow("Can't have two copies of the singleton: " + thisType.ToString(), this);

                string path = UnityEditor.AssetDatabase.GetAssetPath(GetInstanceID());

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                if (System.IO.File.Exists(path + ".meta"))
                {
                    System.IO.File.Delete(path + ".meta");
                }
            }
        }
    }

#endif
}