using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneTracker", menuName = "SceneTracker")]
public class SceneTracker : ScriptableObjectSingleton<SceneTracker>, ISingleton
{
    private List<SceneObserver> observers;
    private Dictionary<SceneMarker, string> scenes;
    private Dictionary<SceneMarker, string> activeScenes;

    /// <summary>
    /// Only let the Singletons class call this method
    /// </summary>
    public void Init()
    {
        scenes = new Dictionary<SceneMarker, string>();
        activeScenes = new Dictionary<SceneMarker, string>();

        foreach (SceneMarker mark in Enum.GetValues(typeof(SceneMarker)))
        {
            scenes.Add(mark, mark.ToString());
        }
    }

    public void LoadScene(SceneMarker scene)
    {
    }
}