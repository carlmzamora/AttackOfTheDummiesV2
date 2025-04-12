using UnityEngine;

public interface IAbilityEffect
{
    public AffectRule AffectRule { get; }
    public void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot );
}