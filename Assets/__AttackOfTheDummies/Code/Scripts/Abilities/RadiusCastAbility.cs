using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusCastAbility : AbilityModule, ITargetedCastModule
{
    [Header("Radius Cast Ability")]
    public float radius;
    public float castDistance;

    [Header("Damage")]
    public float damage;
    //modifier on affected

    //Header("Effects")]
    //cast effect

    public void UpdateWaitForInputDisplay(Vector2 worldPos)
    {

    }

    public void EndWaitForInput(Vector2 worldPos)
    {
        Collider[] allAffected = Physics.OverlapSphere(worldPos, radius);

        if (allAffected.Length <= 0) return;

        foreach (Collider collider in allAffected)
        {
            if (collider.TryGetComponent(out HealthEntity healthEntity))
            {
                healthEntity.TakeDamage(damage);
            }
        }
    }
}
