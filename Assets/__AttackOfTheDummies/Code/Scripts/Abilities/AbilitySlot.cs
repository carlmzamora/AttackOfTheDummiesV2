using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilitySlot
{
    public Ability ability;
    [HideInInspector] public Ability abilityInstance;
    [HideInInspector] public AbilityState state;

    [HideInInspector] public float cooldownProgress;

    public void Perform()
    {
        if (abilityInstance == null)
        {
            Debug.LogError("Attempting to cast with an unassigned ability slot!");
            return;
        }

        switch (state)
        {
            case AbilityState.READY:
                if (abilityInstance.instantCast)
                {
                    abilityInstance.InstantCast();
                    state = AbilityState.COOLDOWN;
                    cooldownProgress = abilityInstance.cooldown;
                }
                else //has targeting
                {
                    abilityInstance.ShowWaitingForInputDisplay();

                    state = AbilityState.WAITING_FOR_INPUT;
                }
                break;
        }
    }

    public void Update(Vector2 worldPos, bool mouse1Pressed)
    {
        switch (state)
        {
            case AbilityState.WAITING_FOR_INPUT:
                abilityInstance.UpdateWaitForInput(worldPos, mouse1Pressed);
                if (mouse1Pressed)
                {
                    abilityInstance.EndWaitForInput();
                    state = AbilityState.COOLDOWN;
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
                    state = AbilityState.READY;
                }
                break;
        }
    }
}

public enum AbilityState
{
    READY,
    WAITING_FOR_INPUT,
    COOLDOWN
}