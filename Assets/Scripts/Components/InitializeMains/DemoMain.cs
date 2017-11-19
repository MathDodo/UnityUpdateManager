using System;
using UnityEngine;

internal class DemoMain : MainComponent<DemoMain>, IInitializeMain
{
    [SerializeField]
    private int updatesRunned;

    public void Init<T>(T args)
    {
        SubscribeToUpdate(Updating);
    }

    private void Updating()
    {
        updatesRunned++;
    }
}