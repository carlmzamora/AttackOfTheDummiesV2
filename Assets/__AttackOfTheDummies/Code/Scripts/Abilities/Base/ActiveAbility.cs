using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/ActiveAbility")]
public class ActiveAbility : Ability
{
    [HideInInspector] public bool instantCast = true;
    public MonoScript moduleScript;

    [SerializeReference] public IAbilityModule abilityModule;

    public void InstantiateModule()
    {
        if (moduleScript == null) return;

        Type moduleType = moduleScript.GetClass();
        if (moduleType != null && typeof(IAbilityModule).IsAssignableFrom(moduleType))
        {
            abilityModule = (IAbilityModule)Activator.CreateInstance(moduleType);
            Debug.Log($"Instantiated: {abilityModule}");
        }
    }

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);

        if (abilityModule != null)
        {
            abilityModule.rootAbility = this;
            abilityModule.Setup(owner);
        }
    }

    public void Activate()
    {
        if(abilityModule is IInstantCastModule instantCastAbility)
        {
            instantCastAbility.InstantCast();
        }
    }

    public void UpdateInputHandling(Vector2 worldPos)
    {
        if(abilityModule is ITargetedCastModule targetedCastAbility)
        {
            targetedCastAbility.UpdateWaitForInputDisplay(worldPos);
        }
    }

    public void ConfirmInput(Vector2 worldPos)
    {
        if (abilityModule is ITargetedCastModule targetedCastAbility)
        {
            targetedCastAbility.EndWaitForInput(worldPos);
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