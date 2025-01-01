using System;
using UnityEngine;

public abstract class SOVariable : ScriptableObject { }

public abstract class SOVariable<T> : SOVariable
{
    [NonSerialized] public T Value;

    public static implicit operator T(SOVariable<T> reference)
    {
        return reference.Value;
    }
}