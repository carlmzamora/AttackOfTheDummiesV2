using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Modifier
{
    [FoldingHeader("General")]
    public string modifierName;

    [FoldingHeader("Application")]
    public bool allowOnlyOneInstance = true;

    [ShowIf(nameof(allowOnlyOneInstance), true)] public int maxStacks;
    [ShowIf(nameof(allowOnlyOneInstance), true)] public bool refreshOnReapply;
    [ShowIf(nameof(allowOnlyOneInstance), true)] public bool independentStackTimers;

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

    public abstract Modifier Clone();

    public FloatParameter GetFloatParameter(string parameterName)
    {
        return abilityRoot != null ? abilityRoot.GetFloatParameter(parameterName, GetType()) : new FloatParameter(0);
    }

    public IntParameter GetIntParameter(string parameterName)
    {
        return abilityRoot != null ? abilityRoot.GetIntParameter(parameterName, GetType()) : new IntParameter(0);
    }
}
