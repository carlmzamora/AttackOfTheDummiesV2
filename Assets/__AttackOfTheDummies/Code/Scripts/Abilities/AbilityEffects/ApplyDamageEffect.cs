using System;
using UnityEngine;

[Serializable]
public class ApplyDamageEffect : AbilityApplicationEffectBase
{
    public float damage;

    public override void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, AffectRule)) return;

        float finalDamage = damage;

        finalDamage *= source.outgoingDamageMultiplier;

        if (target.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(finalDamage);
        }
    }
}
