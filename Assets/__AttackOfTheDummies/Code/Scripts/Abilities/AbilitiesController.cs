using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySlot> abilitySlots;

    [HideInInspector] public Vector2 worldPosFromMousePos = Vector2.zero;
    [HideInInspector] public bool mouse1WasPressed = false;

    private IAbilitiesHolder owner => GetComponent<IAbilitiesHolder>();

    private void Start()
    {
        foreach(AbilitySlot slot in abilitySlots)
        {
            slot.abilityInstance = Instantiate(slot.ability);
            slot.abilityInstance.Setup(owner);
        }
    }

    private void Update()
    {
        foreach (AbilitySlot slot in abilitySlots)
        {
            slot.Update(worldPosFromMousePos, mouse1WasPressed);
        }
    }

    public void PerformMouse01()
    {
        abilitySlots[0].Perform();
    }
}