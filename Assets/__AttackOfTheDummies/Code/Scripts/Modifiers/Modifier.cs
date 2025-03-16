using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modifier
{
    [Header("General")]
    public string modifierName;
    [Tooltip("What factions does this modifier affect?")]
    public AffectRule affectRule;

    [Header("Application")]
    public bool allowOnlyOneInstance = true;
    public int maxStacks;
    public bool refreshOnReapply;
    public bool independentStackTimers;

    [HideInInspector] public int currentStacks;
    [HideInInspector] public MonoBehaviour affected;
    [HideInInspector] public MonoBehaviour source;
    [HideInInspector] public Ability abilityRoot;
    [HideInInspector] public ModifiersController controller;
    [HideInInspector] public float stackDuration;

    protected float startTime;

    public virtual void Instantiate(bool timedStacks)
    {
        AddStack(timedStacks);
    }

    public virtual void AddStack(bool timedStacks)
    {
        currentStacks++;

        if (timedStacks)
            affected.StartCoroutine(TimedStackCoroutine());
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

    protected virtual IEnumerator TimedStackCoroutine()
    {
        if (stackDuration <= 0)
            Debug.LogError($"Stack duration is invalid!");

        yield return new WaitForSeconds(stackDuration + 0.1f);

        if (currentStacks == 0) yield break;

        currentStacks--;
    }

    public virtual void RemoveStack()
    {
        currentStacks--;

        if (currentStacks <= 0)
            Expire();
    }

    public FloatParameter GetFloatParameter(string parameterName)
    {
        return abilityRoot.GetFloatParameter(parameterName, GetType());
    }

    public IntParameter GetIntParameter(string parameterName)
    {
        return abilityRoot.GetIntParameter(parameterName, GetType());
    }
}
