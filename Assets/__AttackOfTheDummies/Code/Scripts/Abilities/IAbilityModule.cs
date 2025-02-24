using UnityEngine;

public interface IAbilityModule
{
    Ability rootAbility { get; set; }
    void Setup(GameObject owner) { }
}