using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FoldingHeaderAttribute : PropertyAttribute
{
    public string name;
    public FoldingHeaderAttribute(string groupName)
    {
        name = groupName;
    }
}