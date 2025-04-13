using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IApplicationEffect : IAbilityEffect
{
    public AffectRule AffectRule { get; }
    public void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot);
}
