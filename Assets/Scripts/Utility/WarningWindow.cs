#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WarningWindow : EditorWindow
{
    private bool isDestroyed;

    private static string msg;
    private static EditorWindow window;
    private static UnityEngine.Object obj;

    public static void ShowWindow(string message, UnityEngine.Object toBeDestroyed)
    {
        msg = message;
        obj = toBeDestroyed;

        window = GetWindow(typeof(WarningWindow));

        //Setting the size of the window
        window.maxSize = new Vector2(500, 70);
        window.minSize = window.maxSize;
    }

    /// <summary>
    /// The ongui method called by unity
    /// </summary>
    private void OnGUI()
    {
        //Showing the message in the window
        EditorGUILayout.TextField(msg);

        //Making four spaces
        for (int i = 0; i < 4; i++)
        {
            MakeSpace();
        }

        //Setting up the close button in the middle of the window
        GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, 50, 100, 100));
        if (GUILayout.Button("Close"))
        {
            //Closing the window if the button is clicked
            window.Close();
        }
        GUILayout.EndArea();
    }

    private void OnLostFocus()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;

            DestroyImmediate(obj, true);
        }
    }

    /// <summary>
    /// Method for making spaces
    /// </summary>
    private void MakeSpace()
    {
        EditorGUILayout.Space();
    }
}

#endif