using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleton
{
    Type TypeOfThis { get; }

    void Init();
}