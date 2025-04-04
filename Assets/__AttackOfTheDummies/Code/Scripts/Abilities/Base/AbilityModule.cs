using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityModule : IAbilityModule
{
    protected IAbilitiesHolder owner;
    public Ability rootAbility { get; set; }
    public bool debugMode { get; set; }

    public virtual void Setup(IAbilitiesHolder owner)
    { 
        this.owner = owner;
    }

    public void ToggleDebugMode(bool value)
    {
        debugMode = value;
    }
}