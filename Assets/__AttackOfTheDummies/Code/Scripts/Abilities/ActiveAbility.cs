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
    public IAbilityModule abilityModule;

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);

        if (abilityModule != null)
        {
            abilityModule.rootAbility = this;
            abilityModule.Setup(owner);
        }
    }

    //Turn these into ActiveAbility?
    public void Activate()
    {
        if(abilityModule is IInstantCastModule instantAbility)
        {
            instantAbility.InstantCast();
        }
    }

    public void UpdateInputHandling(Vector2 worldPos)
    {
        if(abilityModule is IRequireInputModule requireInputAbility)
        {
            requireInputAbility.UpdateWaitForInputDisplay(worldPos);
        }
    }

    public void ConfirmInput(Vector2 worldPos)
    {
        if (abilityModule is IRequireInputModule requireInputAbility)
        {
            requireInputAbility.EndWaitForInput(worldPos);
        }
    }
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