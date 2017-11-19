using System;
using System.Collections.ObjectModel;
using UnityEngine;

public interface IInitializeMain : IInitialize
{
    SceneMarker Scene { get; }
    Action<IInitializeSub> AfterSubAdded { get; set; }
    Action<IInitializeSub> BeforeSubRemoved { get; set; }
    Action<IInitializeMain> BeforeMainDestroyed { get; set; }
    ReadOnlyCollection<IInitializeSub> InitializeSubs { get; }

    T GetSubComponent<T>() where T : IInitializeSub;

    T[] GetSubComponents<T>() where T : IInitializeSub;

    T1 AddAndOrGetSubComponent<T1, T2>(T2 args) where T1 : class, IInitializeSub;

    T1 CreateSubComponentAndAddToGameObject<T1, T2>(T2 args) where T1 : Component, IInitializeSub;

    void AddInitializeSub(IInitializeSub sub, int priority, bool inInspector = false, object args = null);

    void RemoveInitializeSub(IInitializeSub sub);
}