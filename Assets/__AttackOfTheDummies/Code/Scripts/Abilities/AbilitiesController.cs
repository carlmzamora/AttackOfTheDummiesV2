using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySlot> abilitySlots;

    [HideInInspector] public Vector2 mouseWorldPos;
    [HideInInspector] public bool mouse0WasPressed = false;

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
            active.UpdateInputHandling(mouseWorldPos);

            if (justStartedTargeting)
            {
                justStartedTargeting = false;
                return;
            }

            if (mouse0WasPressed)
            {
                Debug.Log("Mouse 0 pressed!");
                if (active.abilityModule is IUnitTargetCastModule unitTargetModule)
                {
                    GameObject selectedUnit = TryFindTargetableUnit(mouseWorldPos, unitTargetModule);
                    if (selectedUnit != null)
                    {
                        unitTargetModule.CastOnTarget(selectedUnit);
                        currentTargetingSlot.slotState = AbilityState.COOLDOWN;
                        currentTargetingSlot.cooldownProgress = active.cooldown;
                        currentTargetingSlot = null;
                        return;
                    }
                    else
                    {
                        unitTargetModule.OnInvalidTarget(); // Show message if invalid
                        return; // Don't cast ability or reset cooldown
                    }
                }

                active.ConfirmInput(mouseWorldPos);
                currentTargetingSlot.slotState = AbilityState.COOLDOWN;
                currentTargetingSlot.cooldownProgress = active.cooldown;
                currentTargetingSlot = null;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                // Cancel targeting
                currentTargetingSlot.slotState = AbilityState.READY;
                currentTargetingSlot = null;
            }
        }
    }

    public void Perform(int slotNumber)
    {
        AbilitySlot slot = abilitySlots[slotNumber];

        if (abilitySlots.Any(slot => slot.slotState == AbilityState.WAITING_FOR_INPUT)) return;

        if (slot.abilityInstance is ActiveAbility active && slot.slotState == AbilityState.READY)
        {
            if (active.abilityModule is IInstantCastModule)
            {
                active.Activate();
                slot.slotState = AbilityState.COOLDOWN;
                slot.cooldownProgress = active.cooldown;
            }
            else if (active.abilityModule is ITargetedCastModule)
            {
                active.StartInputHandling(mouseWorldPos);
                currentTargetingSlot = slot;
                slot.slotState = AbilityState.WAITING_FOR_INPUT;

                justStartedTargeting = true;
            }
        }
    }

    private GameObject TryFindTargetableUnit(Vector2 worldPos, IUnitTargetCastModule module)
    {
        float searchRadius = 1.2f;
        Vector3 center = new Vector3(worldPos.x, 0, worldPos.y);

        Collider[] hits = Physics.OverlapSphere(center, searchRadius, ~LayerMask.GetMask("Environment"));

        GameObject closest = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            GameObject go = hit.gameObject;
            if (!module.CanTarget(go)) continue;

            float distanceSqr = (go.transform.position - center).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closest = go;
                closestDistanceSqr = distanceSqr;
            }
        }

        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(new Vector3(mouseWorldPos.x, 0, mouseWorldPos.y), 1.2f);
    }
}