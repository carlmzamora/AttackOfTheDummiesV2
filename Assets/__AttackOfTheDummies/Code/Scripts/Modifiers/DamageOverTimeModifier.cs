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
    private float startTime;
    private float lastTickTime;

    public override void Instantiate()
    {
        currentTickDamage = tickDamage;
        currentTickCount = tickCount;
        currentTickInterval = tickInterval;

        totalDuration = currentTickInterval * currentTickCount;
        healthEntity = affected.GetComponent<HealthEntity>();

        affected.StartCoroutine(DurationCoroutine());
    }

    private IEnumerator DurationCoroutine()
    {
        startTime = Time.time;

        while (Time.time - startTime < totalDuration) // If duration elapsed is more than totalDuration
        {
            yield return new WaitForSeconds(currentTickInterval);

            healthEntity.TakeDamage(currentTickDamage);

            //lastTickTime = Time.time;
        }
    }
}
