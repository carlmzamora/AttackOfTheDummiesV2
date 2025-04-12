using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySlot> abilitySlots;

    [HideInInspector] public Vector2 worldPosFromMousePos;
    [HideInInspector] public bool mouse1WasPressed = false;

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

        if (currentTargetingSlot != null)
        {
            ActiveAbility active = currentTargetingSlot.abilityInstance as ActiveAbility;
            active.UpdateInputHandling(worldPosFromMousePos);

            if (mouse1WasPressed)
            {
                Debug.Log("Mouse 1 pressed!");
                if (active.abilityModule is IUnitTargetCastModule unitTargetModule)
                {
                    GameObject selectedUnit = TryFindTargetableUnit(worldPosFromMousePos, unitTargetModule);
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

                active.ConfirmInput(worldPosFromMousePos);
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

    public void PerformMouse01()
    {
        AbilitySlot slot = abilitySlots[0];

        if (slot.abilityInstance is ActiveAbility active && slot.slotState == AbilityState.READY)
        {
            if (active.abilityModule is IInstantCastModule instant)
            {
                active.Activate();
                slot.slotState = AbilityState.COOLDOWN;
                slot.cooldownProgress = active.cooldown;
            }
            else if (active.abilityModule is ITargetedCastModule)
            {
                currentTargetingSlot = slot;
                slot.slotState = AbilityState.WAITING_FOR_INPUT;
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
        Gizmos.DrawWireSphere(new Vector3(worldPosFromMousePos.x, 0, worldPosFromMousePos.y), 1.2f);
    }
}