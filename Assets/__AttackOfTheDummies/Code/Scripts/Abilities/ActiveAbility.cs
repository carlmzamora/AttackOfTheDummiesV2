using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/ActiveAbility")]
public class ActiveAbility : Ability
{
    [HideInInspector] public bool instantCast = true;

    [SerializeReference]
    private IAbilityModule abilityModule;

    public void Initialize()
    {
        if (abilityModule != null)
        {
            abilityModule.rootAbility = this;
        }
    }

    //Turn these into ActiveAbility?
    public virtual void InstantCast() { }
    public virtual void ShowWaitingForInputDisplay() { }
    public virtual void UpdateWaitForInput(Vector2 worldPos, bool mouse1Pressed) { }
    public virtual void EndWaitForInput() { }
}

public enum InstantCastType
{
    SELF,
    RADIUS
}

public enum TargetedCastType
{
    UNIT,
    POINT,
    RADIUS
    //VECTOR
}