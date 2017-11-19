using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializeSub : IInitialize
{
    int Priority { get; }
    Type ThisType { get; }
    IInitializeMain Main { get; }

    void InvokeFixedUpdating();

    void InvokeUpdating();

    void InvokeLateUpdating();
}