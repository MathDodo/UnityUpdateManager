using System;
using UnityEngine;

[DisallowMultipleComponent]
internal class DemoSub : SubComponent<DemoSub>, IInitializeSub
{
    [SerializeField]
    private int updatesRunned;

    [SerializeField]
    private bool disableThis;

    [SerializeField]
    private bool destroyThis;

    [SerializeField]
    private bool disableGameObject;

    [SerializeField]
    private bool destroyGameobject;

    private Type thisType = typeof(DemoSub);

    public Type ThisType { get { return thisType; } }

    public void Init<T>(T args)
    {
        SubscribeToUpdate(Updating, UpdateReceiver.Update);
    }

    private void Updating()
    {
        updatesRunned++;

        if (disableThis)
        {
            DisableThis();
        }

        if (destroyThis)
        {
            DestroyThis();
        }

        if (destroyGameobject)
        {
            DestroyGameObject();
        }

        if (disableGameObject)
        {
            DisableGameObject();
        }
    }
}