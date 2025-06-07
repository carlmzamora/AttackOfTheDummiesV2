using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[Serializable]
public abstract class Modifier
{
    [SerializeField] private bool IsSingular;
    [SerializeField] private bool IsLimited;
    [SerializeField] private bool IsNotSingular;
    [SerializeField] private bool IsNotSingularAndFirstInstanceTimer;

    [FoldingHeader("General")]
    public string modifierName;
    public StackingBehaviour stackingBehaviour;
    public bool refreshOnReapply;
    public int maxStacks;
    public StackDurationMode stackDurationMode;
    public bool refreshWholeStackOnReapply;

    [FoldingHeader("Application")]
    public bool allowOnlyOneInstance = true;

    [HideInInspector] public int currentStacks;
    [HideInInspector] public Guid id = Guid.NewGuid();
    [HideInInspector] public MonoBehaviour affected;
    [HideInInspector] public MonoBehaviour source;
    [HideInInspector] public Ability abilityRoot;
    [HideInInspector] public ModifiersController controller;
    [HideInInspector] public float stackDuration;

    public float totalDuration;
    public float durationCountdown;
    protected float startTime;
    public CancellationTokenSource modifierCts = new();

    public virtual async void Instantiate()
    {
        AddStack();        

        await ModDurationTask(modifierCts.Token);
    }

    public virtual void AddStack()
    {
        currentStacks++;

        /*if (timedStacks)
            affected.StartCoroutine(TimedStackCoroutine());*/
    }

    public async UniTask ModDurationTask(CancellationToken ct)
    {
        try
        {
            startTime = Time.time;
            durationCountdown = 0;

            while ((durationCountdown = Time.time - startTime) < totalDuration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            Expire(); //only called when not cancelled
        }
        catch (OperationCanceledException) { }
    }

    public virtual void RefreshDuration()
    {
        startTime = Time.time;
    }    

    protected virtual IEnumerator TimedStackCoroutine()
    {
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

    public void CancelAndExpire()
    {
        if (modifierCts != null)
        {
            modifierCts.Cancel();
            modifierCts.Dispose();
            modifierCts = null;
        }

        Expire();
    }

    protected virtual void Expire()
    {
        //ensure no more stacks remain
        currentStacks = 0;

        controller.UnregisterModifierFromActiveList(this);
    }

    public virtual Modifier Clone()
    {
        return (Modifier)MemberwiseClone();
    }

    public FloatParameter GetFloatParameter(string parameterName)
    {
        return abilityRoot != null ? abilityRoot.GetFloatParameter(parameterName, GetType()) : new FloatParameter(0);
    }

    public IntParameter GetIntParameter(string parameterName)
    {
        return abilityRoot != null ? abilityRoot.GetIntParameter(parameterName, GetType()) : new IntParameter(0);
    }

    public void UpdateShowIfFlags()
    {
        IsSingular = stackingBehaviour == StackingBehaviour.SINGULAR;
        IsLimited = stackingBehaviour == StackingBehaviour.LIMITED;
        IsNotSingular = stackingBehaviour != StackingBehaviour.SINGULAR;
        IsNotSingularAndFirstInstanceTimer = IsNotSingular && stackDurationMode == StackDurationMode.FIRST_INSTANCE_TIMER;
    }
}

public enum StackingBehaviour
{
    SINGULAR = 0,
    LIMITED = 1,
    UNLIMITED = 2
}

public enum StackDurationMode
{
    FIRST_INSTANCE_TIMER,
    INDIVIDUAL_TIMERS
}