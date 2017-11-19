using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class UpdateManager : MonoBehaviourSingleton<UpdateManager>, ISingleton
{
    [SerializeField]
    private int updatesOnDelegate = 500;

    private List<DelegateSystem> updates = new List<DelegateSystem>();
    private List<DelegateSystem> lateUpdates = new List<DelegateSystem>();
    private List<DelegateSystem> fixedUpdates = new List<DelegateSystem>();

    public void Init()
    {
    }

    internal string SubscribeToUpdateEvent(Action action, UpdateReceiver updateReceiver)
    {
        bool added = false;

        if (updateReceiver == UpdateReceiver.FixedUpdate)
        {
            while (!added)
            {
                for (int i = 0; i < fixedUpdates.Count; i++)
                {
                    if (fixedUpdates[i].Added < updatesOnDelegate)
                    {
                        fixedUpdates[i].Added++;
                        added = true;
                        return fixedUpdates[i].AddAction(action);
                    }
                }

                if (!added)
                {
                    fixedUpdates.Add(new DelegateSystem());
                }
            }
        }

        if (updateReceiver == UpdateReceiver.Update)
        {
            while (!added)
            {
                for (int i = 0; i < updates.Count; i++)
                {
                    if (updates[i].Added < updatesOnDelegate)
                    {
                        updates[i].Added++;
                        added = true;
                        return updates[i].AddAction(action);
                    }
                }

                if (!added)
                {
                    updates.Add(new DelegateSystem());
                }
            }
        }
        else
        {
            while (!added)
            {
                for (int i = 0; i < lateUpdates.Count; i++)
                {
                    if (lateUpdates[i].Added < updatesOnDelegate)
                    {
                        lateUpdates[i].Added++;
                        added = true;
                        return lateUpdates[i].AddAction(action);
                    }
                }

                if (!added)
                {
                    lateUpdates.Add(new DelegateSystem());
                }
            }
        }

        return string.Empty;
    }

    internal void RemoveFromUpdateEvent(Action action, string systemID, UpdateReceiver updateReceiver)
    {
        if (updateReceiver == UpdateReceiver.FixedUpdate)
        {
            fixedUpdates.Find(d => d.SystemID == systemID).RemoveAction(action);

            return;
        }

        if (updateReceiver == UpdateReceiver.Update)
        {
            updates.Find(d => d.SystemID == systemID).RemoveAction(action);
        }
        else
        {
            lateUpdates.Find(d => d.SystemID == systemID).RemoveAction(action);
        }
    }

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 2000;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < fixedUpdates.Count; i++)
        {
            fixedUpdates[i].Invoke();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        for (int i = 0; i < updates.Count; i++)
        {
            updates[i].Invoke();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < lateUpdates.Count; i++)
        {
            lateUpdates[i].Invoke();
        }
    }
}