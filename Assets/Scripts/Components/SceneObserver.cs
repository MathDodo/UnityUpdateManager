using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneObserver : MonoBehaviour
{
    [SerializeField]
    private SceneMarker scene;

    //Placeholder
    [SerializeField]
    private int amountToSpawn;

    //Placeholder
    [SerializeField, ObjectRestriction(typeof(IInitializeMain), SearchForm.AssetsOnly, CollectionMode.None)]
    private DemoMain demoPrefab;

    [SerializeField, ObjectRestriction(typeof(IInitializeMain), SearchForm.SceneOnly, CollectionMode.Array)]
    private Component[] initializeMains;

    private List<IInitializeMain> mainsInScene;

    public SceneMarker Scene { get { return scene; } }

    private void Awake()
    {
        mainsInScene = new List<IInitializeMain>();
        SceneManager.sceneLoaded += Initialize;

        for (int i = 0; i < initializeMains.Length; i++)
        {
            mainsInScene.Add(initializeMains[i] as IInitializeMain);
        }

        initializeMains = null;

        for (int i = 0; i < amountToSpawn; i++)
        {
            var init = Instantiate(demoPrefab);
            init.transform.position = new Vector3(i, i, i);
            init.Initialize(this);
            init.Init("Stating");
        }
    }

    private void Initialize(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < mainsInScene.Count; i++)
        {
            mainsInScene[i].Initialize(this);
            mainsInScene[i].Init("SceneStart");
        }

        SceneManager.sceneLoaded -= Initialize;
    }

    internal void AddMain(IInitializeMain initializeMain)
    {
        mainsInScene.Add(initializeMain);
        initializeMain.BeforeMainDestroyed += RemoveMain;
    }

    internal void RemoveMain(IInitializeMain initializeMain)
    {
        mainsInScene.Remove(initializeMain);
    }

#if UNITY_EDITOR

    public void GatherInitializeMains()
    {
        if (!Application.isPlaying)
        {
            List<Component> mains = new List<Component>();
            var array = gameObject.scene.GetRootGameObjects();

            for (int i = 0; i < array.Length; i++)
            {
                Component component;

                if ((component = array[i].GetComponent(typeof(ISingleton))))
                {
                    Debug.LogError("A singleton cant start in a scene");
                    DestroyImmediate(component.gameObject);
                }

                if ((component = array[i].GetComponent(typeof(IInitializeMain))))
                {
                    if ((component as IInitializeMain).Scene == scene || (component as IInitializeMain).Scene == SceneMarker.All)
                    {
                        mains.Add(component);
                    }
                    else
                    {
                        Debug.LogError("The gameobject " + component.gameObject + " has a main component which can't be in this scene");
                        DestroyImmediate(component.gameObject);
                    }
                }
            }

            initializeMains = new Component[mains.Count];

            for (int i = 0; i < mains.Count; i++)
            {
                initializeMains[i] = mains[i];
            }
        }
        else
        {
            Debug.Log("Cant add when playing");
        }
    }

#endif
}