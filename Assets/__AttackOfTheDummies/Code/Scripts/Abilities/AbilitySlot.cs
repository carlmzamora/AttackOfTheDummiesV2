using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilitySlot
{
    public Ability ability;
    [HideInInspector] public Ability abilityInstance;
    [HideInInspector] public AbilityState slotState;

    [HideInInspector] public float cooldownProgress;

    public void Perform()
    {
        if (abilityInstance == null)
        {
            Debug.LogError("Attempting to cast with an unassigned ability slot!");
            return;
        }

        if(abilityInstance is ActiveAbility activeAbilityInstance)
        {
            switch (slotState)
            {
                case AbilityState.READY:

                    if (activeAbilityInstance.abilityModule is IInstantCastModule)
                    {
                        activeAbilityInstance.Activate();
                        slotState = AbilityState.COOLDOWN;
                        cooldownProgress = abilityInstance.cooldown;
                    }
                    else if(activeAbilityInstance.abilityModule is IRequireInputModule)
                    {
                        slotState = AbilityState.WAITING_FOR_INPUT;
                    }
                    break;
            }
        }
    }

    public void Update(Vector2 worldPos, bool mouse1Pressed)
    {
        if (abilityInstance is ActiveAbility activeAbilityInstance)
        {
            switch (slotState)
            {
                case AbilityState.WAITING_FOR_INPUT:
                    activeAbilityInstance.UpdateInputHandling(worldPos);
                    if (mouse1Pressed)
                    {
                        activeAbilityInstance.ConfirmInput(worldPos);
                        slotState = AbilityState.COOLDOWN;
                        //add definitions for types of targeting here?
                    }
                    break;

                case AbilityState.COOLDOWN:
                    if (cooldownProgress > 0)
                    {
                        cooldownProgress -= Time.deltaTime;
                    }
                    else
                    {
                        slotState = AbilityState.READY;
                    }
                    break;
            }
        }
    }
}

public enum AbilityState
{
    READY,
    WAITING_FOR_INPUT,
    COOLDOWN
}