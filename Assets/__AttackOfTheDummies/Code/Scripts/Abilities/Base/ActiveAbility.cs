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
    [HideInInspector] public bool hasChosenModule = false;
    [HideInInspector] public string moduleDataJson;

    public override void Setup(IAbilitiesHolder owner, int level = -1)
    {
        base.Setup(owner, level);

        if (abilityModule != null)
        {
            InGameRefs.Instance.OnDebugModeChanged += abilityModule.ToggleDebugMode;
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

    public void StartInputHandling(Vector2 mouseWorldPos)
    {
        if (abilityModule is ITargetedCastModule targetedCastAbility)
        {
            Debug.Log("Wait for input started.");

            targetedCastAbility.StartWaitForInput(mouseWorldPos);
        }
    }

    public void UpdateInputHandling(Vector2 mouseWorldPos)
    {
        if(abilityModule is ITargetedCastModule targetedCastAbility)
        {
            targetedCastAbility.UpdateWaitForInputDisplay(mouseWorldPos);
        }
    }

    public void ConfirmInput(Vector2 mouseWorldPos)
    {
        if (abilityModule is ITargetedCastModule targetedCastAbility)
        {
            targetedCastAbility.EndWaitForInput(mouseWorldPos);
        }
    }

    #region EDITOR
    public void InstantiateModule()
    {
        if (moduleScript == null) return;

        Type moduleType = moduleScript.GetClass();
        if (moduleType != null && typeof(IAbilityModule).IsAssignableFrom(moduleType))
        {
            abilityModule = (IAbilityModule)Activator.CreateInstance(moduleType);
            hasChosenModule = true;
        }
    }
    #endregion
}