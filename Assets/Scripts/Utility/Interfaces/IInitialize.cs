using System;
using UnityEngine;

public interface IInitialize
{
    int InstanceID { get; }
    Component This { get; }
    Action AfterEnabled { get; set; }
    Action AfterDisabled { get; set; }
    Action BeforeDestroyed { get; set; }

    void EnableThis();

    void DestroyThis();

    void DisableThis();

    void EnableGameObject();

    void DisableGameObject();

    void DestroyGameObject();

    void Initialize(SceneObserver sceneObserver);

    void Init<T>(T args);
}