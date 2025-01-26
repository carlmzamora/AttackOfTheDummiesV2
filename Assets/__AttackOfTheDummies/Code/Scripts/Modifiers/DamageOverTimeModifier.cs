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

    private float currentTickDamage;
    private int currentTickCount;
    private float currentTickInterval;

    private HealthEntity healthEntity;
    private float totalDuration;
    private float lastTickTime;
    private float timeBetweenRefreshAndLastTick;

    public override void Instantiate(bool timedStacks)
    {
        currentTickDamage = tickDamage;
        currentTickCount = tickCount;
        currentTickInterval = tickInterval;

        totalDuration = currentTickInterval * currentTickCount;
        healthEntity = affected.GetComponent<HealthEntity>();

        affected.StartCoroutine(DurationCoroutine());

        base.AddStack(timedStacks);
    }

    private IEnumerator DurationCoroutine()
    {
        startTime = Time.time;

        while (Time.time - startTime < totalDuration) // If duration elapsed is more than totalDuration
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
