using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

[DisallowMultipleComponent]
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>, ISingleton
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

    private void OnValidate()
    {
        if (gameObject.scene.name != null)
        {
            if (!Application.isPlaying)
            {
                WarningWindow.ShowWindow("A singleton can't start in a scene", gameObject);
            }
        }

        if (!Application.isPlaying && gameObject.scene.name == null)
        {
            UnityEngine.Object[] obj = Resources.LoadAll("ISingletons", thisType);

            if (obj.Length > 0 && obj[0] != this)
            {
                WarningWindow.ShowWindow("Can't have two copies of the singleton: " + thisType.ToString(), gameObject);

                string path = AssetDatabase.GetAssetPath(GetInstanceID());

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

    private void Reset()
    {
        if (gameObject.scene.name != null)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("A singleton can't start in a scene move it to Resources/ISingletons");
            }
        }

        if (transform != transform.root)
        {
            Debug.LogError("A singleton needs to be on the top gameobject of hierarchy");
            DestroyImmediate(this);
        }
    }
}