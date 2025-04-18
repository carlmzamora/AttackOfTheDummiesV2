using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySlot> abilitySlots;

    [HideInInspector] public Vector3 mouseWorldPos;
    [HideInInspector] public bool confirmButtonWasPressed = false;

    private bool justStartedTargeting = false;

    private IAbilitiesHolder owner => GetComponent<IAbilitiesHolder>();

    private AbilitySlot currentTargetingSlot = null;

    private void Start()
    {
        foreach(AbilitySlot slot in abilitySlots)
        {
            slot.abilityInstance = Instantiate(slot.ability);
            slot.abilityInstance.Setup(owner);
        }
    }

    public void Trigger(int slotNumber)
    {
        AbilitySlot slot = abilitySlots[slotNumber];

        if (slot.abilityInstance is not ActiveAbility) return;

        if (slot.slotState != AbilityState.READY) return;

        if (abilitySlots.Any(slot => slot.slotState == AbilityState.WAITING_FOR_INPUT)) return;

        ActiveAbility active = slot.abilityInstance as ActiveAbility;

        if (active.abilityModule is IInstantCastModule)
        {
            active.Activate();
            slot.slotState = AbilityState.COOLDOWN;
            slot.cooldownProgress = active.cooldown;
        }
        
        if (active.abilityModule is ITargetedCastModule targeted)
        {
            Debug.Log($"{active.abilityModule} wait for input started.");
            targeted.StartWaitForInput(mouseWorldPos);
            currentTargetingSlot = slot;
            slot.slotState = AbilityState.WAITING_FOR_INPUT;

            justStartedTargeting = true;
        }
    }

    private void Update()
    {
        foreach (AbilitySlot slot in abilitySlots)
        {
            if (slot.slotState == AbilityState.COOLDOWN)
            {
                slot.cooldownProgress -= Time.deltaTime;
                if (slot.cooldownProgress <= 0)
                {
                    slot.slotState = AbilityState.READY;
                }
            }
        }

        if (currentTargetingSlot != null && currentTargetingSlot.slotState == AbilityState.WAITING_FOR_INPUT)
        {
            ActiveAbility active = currentTargetingSlot.abilityInstance as ActiveAbility;

            //prevents auto-ending input if trigger and confirm buttons are the same (eg. mouse0)
            if (justStartedTargeting)
            {
                justStartedTargeting = false;
                return;
            }

            if (active.abilityModule is ITargetedCastModule targeted)
                targeted.UpdateWaitForInput(mouseWorldPos, confirmButtonWasPressed);
            else
                return;

            if (confirmButtonWasPressed)
            {
                Debug.Log("Mouse 0 pressed!");

                targeted.ConcludeWaitForInput(mouseWorldPos);
                currentTargetingSlot.slotState = AbilityState.COOLDOWN;
                currentTargetingSlot.cooldownProgress = active.cooldown;
                currentTargetingSlot = null;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                // Cancel targeting
                targeted.CancelWaitForInput();
                currentTargetingSlot.slotState = AbilityState.READY;
                currentTargetingSlot = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(mouseWorldPos, 1.2f);
    }
}