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
}

public enum AbilityState
{
    READY,
    WAITING_FOR_INPUT,
    COOLDOWN
}