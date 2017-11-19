using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ObjectRestrictionAttribute : PropertyAttribute
{
    public Type Restriction;
    public SearchForm SearchForm;
    public CollectionMode CollectionMode;

    public ObjectRestrictionAttribute(Type restriction, SearchForm searchForm, CollectionMode collectionMode)
    {
        SearchForm = searchForm;
        Restriction = restriction;
        CollectionMode = collectionMode;
    }
}