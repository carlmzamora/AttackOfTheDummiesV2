using UnityEngine;

public interface IAbilityModule
{
    Ability rootAbility { get; set; }
    bool debugMode { get; set; }
    void Setup(GameObject owner) { }
    void ToggleDebugMode(bool value) { }
}