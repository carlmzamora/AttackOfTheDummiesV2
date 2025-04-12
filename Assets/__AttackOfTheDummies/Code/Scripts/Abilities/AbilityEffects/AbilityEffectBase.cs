using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityEffectBase : IAbilityEffect
{
    [Tooltip("What factions should this affect?")]
    public AffectRule affectRule;
    public AffectRule AffectRule => affectRule;

    public abstract void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot);
}