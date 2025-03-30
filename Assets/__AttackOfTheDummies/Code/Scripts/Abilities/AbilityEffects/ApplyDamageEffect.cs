using System;
using UnityEngine;

[Serializable]
public class ApplyDamageEffect : IAbilityEffect
{
    public float damage;

    public void ApplyEffect(GameObject target, GameObject source, Ability abilityRoot)
    {
        if (target.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(damage);
        }
    }
}
