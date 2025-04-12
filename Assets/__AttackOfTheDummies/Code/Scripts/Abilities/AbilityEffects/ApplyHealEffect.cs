using System;
using UnityEngine;

[Serializable]
public class ApplyHealEffect : AbilityEffectBase
{
    public float heal;

    public override void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, AffectRule)) return;

        float finalHeal = heal;

        finalHeal *= source.outgoingHealMultiplier;

        if (target.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.ApplyHeal(finalHeal);
        }
    }
}
