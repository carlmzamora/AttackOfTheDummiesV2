using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : ScriptableObject
{
    [Header("General")]
    public string modifierName;

    public bool allowOnlyOneInstance = true;
    public int maxStacks;
    public bool refreshOnReapply;
    public bool independentStackTimers;

    [HideInInspector] public int currentStacks;
    [HideInInspector] public MonoBehaviour affected;
    [HideInInspector] public MonoBehaviour source;
    [HideInInspector] private ModifiersController controller;

    public virtual void SetAffected(GameObject affected)
    {
        this.affected = affected.GetComponent<MonoBehaviour>();
        controller = affected.GetComponent<ModifiersController>();
    }

    public virtual void SetSource(GameObject source)
    {
        this.source = source.GetComponent<MonoBehaviour>();
    }

    public virtual void Instantiate()
    {

    }
}
