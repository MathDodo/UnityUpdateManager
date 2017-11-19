using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SearchForm
{
    /// <summary>
    /// With this searchform, the restricion or popup will search the entire scene, root objects and children
    /// </summary>
    SceneOnly,

    /// <summary>
    /// With this searchform, the restricion or popup will search the entire Prefabs folder in Resources
    /// </summary>
    AssetsOnly,

    /// <summary>
    /// With this searchform, the restricion or popup will search an entire hierarchy which Root object's name is set as the attributes Target
    /// </summary>
    HierarchyOnly
}