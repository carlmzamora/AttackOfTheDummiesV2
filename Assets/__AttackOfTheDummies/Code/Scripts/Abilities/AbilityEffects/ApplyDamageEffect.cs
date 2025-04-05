using System;
using UnityEngine;

[Serializable]
public class ApplyDamageEffect : IAbilityEffect
{
    [Tooltip("What factions should we damage?")]
    public AffectRule affectRule;
    public float damage;

    public void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, affectRule)) return;

        float finalDamage = damage;

        finalDamage *= source.outgoingDamageMultiplier;

        if (target.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(finalDamage);
        }
    }
}
