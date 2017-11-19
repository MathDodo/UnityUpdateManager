using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

internal class DelegateSystem
{
    private Action action;

    internal int Added;
    internal string SystemID;

    internal DelegateSystem()
    {
        Added = 0;
        SystemID = Guid.NewGuid().ToString();
    }

    internal string AddAction(Action action)
    {
        this.action += action;
        return SystemID;
    }

    internal void RemoveAction(Action action)
    {
        this.action -= action;
    }

    internal void Invoke()
    {
        if (action != null)
        {
            action.Invoke();
        }
    }
}