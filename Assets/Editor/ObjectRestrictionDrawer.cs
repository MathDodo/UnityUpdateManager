using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(ObjectRestrictionAttribute))]
public class ObjectRestrictionDrawer : PropertyDrawer
{
    private int index;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetAttribute = attribute as ObjectRestrictionAttribute;

        if (targetAttribute.Restriction == typeof(ISingleton) || targetAttribute.Restriction.GetInterfaces().Contains(typeof(ISingleton)))
        {
            Debug.LogError("Can't have reference to singleton asset, and singletons shouldnt start in a scene");
        }
        else if (fieldInfo.FieldType == typeof(ISingleton) || fieldInfo.FieldType.GetInterfaces().Contains(typeof(ISingleton)))
        {
            Debug.LogError("Can't have reference to singleton asset, and singletons shouldnt start in a scene");
        }
        else
        {
            switch (targetAttribute.SearchForm)
            {
                case SearchForm.SceneOnly:
                    LoadFromScene(position, property, label, targetAttribute);
                    break;

                case SearchForm.AssetsOnly:
                    LoadFromResources(position, property, label, targetAttribute);
                    break;

                case SearchForm.HierarchyOnly:
                    LoadFromHierarchy(position, property, label, targetAttribute);
                    break;
            }
        }
    }

    private void LoadFromScene(Rect position, SerializedProperty property, GUIContent label, ObjectRestrictionAttribute targetAttribute)
    {
        var array = SceneManager.GetActiveScene().GetRootGameObjects();
        List<UnityEngine.Object> targets = new List<UnityEngine.Object>();
        Type type = fieldInfo.FieldType;

        switch (targetAttribute.CollectionMode)
        {
            case CollectionMode.Array:
                type = fieldInfo.FieldType.GetElementType();
                break;

            case CollectionMode.List:
                type = fieldInfo.FieldType.GetGenericArguments()[0];
                break;
        }

        for (int i = 0; i < array.Length; i++)
        {
            UnityEngine.Object[] objects;

            if ((objects = array[i].GetComponents(targetAttribute.Restriction)).Length > 0)
            {
                List<UnityEngine.Object> objectsToBeAdded = new List<UnityEngine.Object>();

                for (int t = 0; t < objects.Length; t++)
                {
                    if (objects[t].GetType() == type || objects[t].GetType().IsSubclassOf(type))
                    {
                        objectsToBeAdded.Add(objects[t]);
                    }
                }

                targets.AddRange(objectsToBeAdded);
            }
        }

        string[] usage = new string[targets.Count + 1];

        usage[0] = "null";

        for (int i = 0; i < targets.Count; i++)
        {
            usage[i + 1] = targets[i].ToString() + " (scene)";
        }

        if (!property.objectReferenceValue)
        {
            index = 0;
        }
        else
        {
            index = targets.IndexOf(targets.Find(o => o.ToString() == property.objectReferenceValue.ToString())) + 1;
        }

        index = EditorGUI.Popup(position, label.text, index, usage);

        if (index == 0)
        {
            property.objectReferenceValue = null;
        }
        else
        {
            if (targets[index - 1] is ISingleton)
            {
                Debug.LogError("Can't have references to singleton use 'Singleton class name'.Instance at runtime");
                property.objectReferenceValue = null;
            }
            else
            {
                property.objectReferenceValue = targets[index - 1];
            }
        }
    }

    private void LoadFromHierarchy(Rect position, SerializedProperty property, GUIContent label, ObjectRestrictionAttribute targetAttribute)
    {
        if (property.serializedObject.targetObject is Component)
        {
            var comp = (property.serializedObject.targetObject as Component);
            var targets = comp.GetComponents(targetAttribute.Restriction).ToList();

            var children = comp.GetComponentsInChildren(targetAttribute.Restriction);

            for (int i = 0; i < children.Length; i++)
            {
                targets.Add(children[i]);
            }

            List<UnityEngine.Component> targetsToRemove = new List<UnityEngine.Component>();
            List<string> usage = new List<string>();
            Type type = fieldInfo.FieldType;

            switch (targetAttribute.CollectionMode)
            {
                case CollectionMode.Array:
                    type = fieldInfo.FieldType.GetElementType();
                    break;

                case CollectionMode.List:
                    type = fieldInfo.FieldType.GetGenericArguments()[0];
                    break;
            }

            usage.Add("null");

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].GetType() == type || targets[i].GetType().IsSubclassOf(type))
                {
                    usage.Add(targets[i].ToString() + " (Hierarchy)");
                }
                else
                {
                    targetsToRemove.Add(targets[i]);
                }
            }

            for (int i = 0; i < targetsToRemove.Count; i++)
            {
                targets.Remove(targetsToRemove[i]);
            }

            if (property.objectReferenceValue == null)
            {
                index = 0;
            }
            else
            {
                index = targets.IndexOf(targets.Find(o => o.ToString() == property.objectReferenceValue.ToString())) + 1;
            }

            index = EditorGUI.Popup(position, label.text, index, usage.ToArray());

            if (index == 0)
            {
                property.objectReferenceValue = null;
            }
            else
            {
                if (targets[index - 1] is ISingleton)
                {
                    Debug.LogError("Can't have references to singleton use 'Singleton class name'.Instance at runtime");
                    property.objectReferenceValue = null;
                }
                else
                {
                    property.objectReferenceValue = targets[index - 1];
                }
            }
        }
        else
        {
            label.text = "This Object doesnt have a hierarchy";
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private void LoadFromResources(Rect position, SerializedProperty property, GUIContent label, ObjectRestrictionAttribute targetAttribute)
    {
        List<string> paths = new List<string>();

        paths.AddRange(Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories));
        paths.AddRange(Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories));

        List<UnityEngine.Object> targets = new List<UnityEngine.Object>();
        UnityEngine.Object obj;

        for (int i = 0; i < paths.Count; i++)
        {
            string assetPath = "Assets" + paths[i].Replace(Application.dataPath, "").Replace('\\', '/');

            if ((obj = AssetDatabase.LoadAssetAtPath(assetPath, targetAttribute.Restriction)))
            {
                targets.Add(obj);
            }
        }

        List<UnityEngine.Object> targetsToRemove = new List<UnityEngine.Object>();
        List<Component> extraTargets = new List<Component>();
        List<string> usage = new List<string>();
        Type type = fieldInfo.FieldType;

        switch (targetAttribute.CollectionMode)
        {
            case CollectionMode.Array:
                type = fieldInfo.FieldType.GetElementType();
                break;

            case CollectionMode.List:
                type = fieldInfo.FieldType.GetGenericArguments()[0];
                break;
        }

        usage.Add("null");

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].GetType() == type || targets[i].GetType().IsSubclassOf(type))
            {
                if (targets[i] is Component)
                {
                    var array = (targets[i] as Component).GetComponents(targetAttribute.Restriction);

                    for (int l = 0; l < array.Length; l++)
                    {
                        extraTargets.Add(array[l]);
                    }
                }

                usage.Add(targets[i].ToString() + " (asset)");
            }
            else
            {
                targetsToRemove.Add(targets[i]);
            }
        }

        for (int i = 0; i < extraTargets.Count; i++)
        {
            if (extraTargets[i].GetType() == type || extraTargets[i].GetType().IsSubclassOf(type))
            {
                usage.Add(extraTargets[i].ToString() + " (asset)");
                targets.Add(extraTargets[i]);
            }
        }

        for (int i = 0; i < targetsToRemove.Count; i++)
        {
            targets.Remove(targetsToRemove[i]);
        }

        if (!property.objectReferenceValue)
        {
            index = 0;
        }
        else
        {
            index = targets.IndexOf(targets.Find(o => o.ToString() == property.objectReferenceValue.ToString())) + 1;
        }

        index = EditorGUI.Popup(position, label.text, index, usage.ToArray());

        if (index == 0)
        {
            property.objectReferenceValue = null;
        }
        else
        {
            if (targets[index - 1] is ISingleton)
            {
                Debug.LogError("Can't have references to singleton use 'Singleton class name'.Instance at runtime");
                property.objectReferenceValue = null;
            }
            else
            {
                property.objectReferenceValue = targets[index - 1];
            }
        }
    }
}