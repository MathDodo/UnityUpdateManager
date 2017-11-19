using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneObserver))]
public class SceneObserverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneObserver sceneObserver = (SceneObserver)target;

        if (GUILayout.Button("GatherMains"))
        {
            sceneObserver.GatherInitializeMains();
        }
    }
}