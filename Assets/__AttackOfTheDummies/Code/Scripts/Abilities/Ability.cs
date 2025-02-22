using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Ability : ScriptableObject
{
    [Header("General")]
    public string displayName;
    //public Sprite abilityIcon;
    [Manipulable] public float cooldown;

    protected GameObject owner;
    public int level;

    public Dictionary<string, FloatParameter> floatParameters = new();
    public Dictionary<string, IntParameter> intParameters = new();

    public virtual void Setup(GameObject owner, int level = -1)
    {
        this.owner = owner;
        this.level = level;

        CacheParameters(GetType(), this, this);

        //TODO:
        //abilities should be upgradable? not like leveling up, but like aghanim scepter upgrades
        //and not just variable manipulation, but behaviour changes as well
    }

    private void CacheParameters(Type type, object target, Ability root)
    {
        foreach (FieldInfo field in type.GetFields())
        {
            if (Attribute.IsDefined(field, typeof(ManipulableAttribute)))
            {
                if (field.FieldType == typeof(float))
                {
                    floatParameters.Add($"{root.name}_{type}_{field.Name}", new((float)field.GetValue(target)));
                }
                else if (field.FieldType == typeof(int))
                {
                    intParameters.Add($"{root.name}_{type}_{field.Name}", new((int)field.GetValue(target)));
                }
            }

            if (field.FieldType == typeof(List<Modifier>))
            {
                if (field.GetValue(this) is List<Modifier> modifierList)
                {
                    CacheModifiersManipulableFields(modifierList, root);
                }
            }
        }
    }

    private void CacheModifiersManipulableFields(List<Modifier> modifierList, Ability abilityRoot)
    {
        foreach (Modifier modifier in modifierList)
        {
            CacheParameters(modifier.GetType(), modifier, abilityRoot);
        }
    }

    public FloatParameter GetFloatParameter(string parameterName, Type parentType)
    {
        return floatParameters[$"{name}_{parentType}_{parameterName}"];
    }

    public IntParameter GetIntParameter(string parameterName, Type parentType)
    {
        return intParameters[$"{name}_{parentType}_{parameterName}"];
    }
}

public class FloatParameter
{
    public float value;

    public FloatParameter(float value)
    {
        this.value = value;
    }

    public void AddValue(float value)
    {
        this.value += value;
    }

    public static implicit operator float(FloatParameter parameter)
    {
        return parameter.value;
    }
}

public class IntParameter
{
    public int value;

    public IntParameter(int value)
    {
        this.value = value;
    }

    public void AddValue(int value)
    {
        this.value += value;
    }

    public static implicit operator int(IntParameter parameter)
    {
        return parameter.value;
    }
}