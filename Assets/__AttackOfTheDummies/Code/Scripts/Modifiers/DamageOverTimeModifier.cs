using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Modifiers/DamageOverTime")]
public class DamageOverTimeModifier : Modifier
{
    [Header("DamageOverTime")]
    public float tickDamage;
    public int tickCount;
    public float tickInterval;

    [HideInInspector, Manipulable] public float tickDamageAdditive;
    [HideInInspector, Manipulable] public int tickCountAdditive;

    private float currentTickDamage;
    private int currentTickCount;
    private float currentTickInterval;

    private HealthEntity healthEntity;
    private float duration;
    private float lastTickTime;
    private float timeBetweenRefreshAndLastTick;

    public override void Instantiate(bool timedStacks)
    {
        currentTickDamage = tickDamage + abilityRoot.floatParameters[nameof(tickDamageAdditive)];
        currentTickCount = tickCount + abilityRoot.intParameters[nameof(tickCountAdditive)];
        currentTickInterval = tickInterval;
        if(currentTickInterval <= 0)
        {
            currentTickInterval = 0.1f; //minimum
        }

        duration = currentTickInterval * currentTickCount;
        stackDuration = duration;

        healthEntity = affected.GetComponent<HealthEntity>();

        affected.StartCoroutine(DurationCoroutine());

        base.Instantiate(timedStacks);
    }

    private IEnumerator DurationCoroutine()
    {
        startTime = Time.time;

        while (Time.time - startTime < duration) // If duration elapsed is more than totalDuration
        {
            yield return new WaitForSeconds(currentTickInterval);

            healthEntity.TakeDamage(currentTickDamage * currentStacks);

            lastTickTime = Time.time;
        }

        Expire();
    }

    public override void RefreshDuration()
    {
        float timeOfRefresh = Time.time; //set current time

        if (lastTickTime > 0)
        {
            timeBetweenRefreshAndLastTick = lastTickTime - timeOfRefresh;
        }
        else //if this modifier just started and there is no first tick yet
        {
            timeBetweenRefreshAndLastTick = startTime - timeOfRefresh;
        }

        //add the time between the tick and refresh time so that small portion is still included
        startTime = Time.time + timeBetweenRefreshAndLastTick;
    }
}
