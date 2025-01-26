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
    [HideInInspector] public ModifiersController controller;

    protected float startTime;

    public virtual void SetAffected(GameObject affected)
    {
        this.affected = affected.GetComponent<MonoBehaviour>();
        controller = affected.GetComponent<ModifiersController>();
    }

    public virtual void SetSource(GameObject source)
    {
        this.source = source.GetComponent<MonoBehaviour>();
    }

    public virtual void Instantiate(bool timedStacks)
    {
        AddStack(timedStacks);
    }

    public void AddStack(bool timedStacks)
    {
        currentStacks++;
    }

    public virtual void RemoveStack()
    {
        currentStacks--;

        if (currentStacks <= 0)
            Expire();
    }

    public virtual void RefreshDuration()
    {
        startTime = Time.time;
    }

    public virtual void Expire()
    {
        currentStacks = 0;
        controller.RemoveModifier(this);
    }
}
