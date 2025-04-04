using UnityEngine;

public interface IAbilityEffect
{
    public void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot );
}