using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityApplicationEffectBase : IApplicationEffect
{
    [Tooltip("What factions should this affect?")]
    public AffectRule affectRule;
    public AffectRule AffectRule => affectRule;

    public abstract void ApplyEffect(GameObject target, IAbilitiesHolder source = null, Ability abilityRoot = null);
}