using UnityEngine;

public interface IAbilityModule
{
    Ability rootAbility { get; set; }
    bool debugMode { get; set; }
    void Setup(IAbilitiesHolder owner) { }
    void ToggleDebugMode(bool value) { }
}