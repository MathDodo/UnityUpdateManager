using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameComponent), editorForChildClasses: true)]
public class MainComponentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (target.GetType().GetInterfaces().Contains(typeof(IInitializeMain)))
        {
            if (GUILayout.Button("Starting subs"))
            {
                GameComponent gc = (GameComponent)target;
                gc.UpdateStartingSubsList();
            }
        }
    }
}