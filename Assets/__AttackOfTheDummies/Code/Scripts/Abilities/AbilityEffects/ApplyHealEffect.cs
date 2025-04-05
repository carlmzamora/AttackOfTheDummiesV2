using System;
using UnityEngine;

[Serializable]
public class ApplyHealEffect : IAbilityEffect
{
    [Tooltip("What factions should we heal?")]
    public AffectRule affectRule;
    public float heal;

    public void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, affectRule)) return;

        float finalHeal = heal;

        finalHeal *= source.outgoingHealMultiplier;

        if (target.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.ApplyHeal(finalHeal);
        }
    }
}
