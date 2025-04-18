using System.Collections.Generic;
using UnityEngine;

public class UnitTargetModule : AbilityModule, IUnitTargetCastModule
{
    public bool hasGlobalCastRange;
    public float castRange;

    [SerializeField] public RadiusReticle customRadiusReticle;
    public RadiusReticle CustomRadiusReticle => customRadiusReticle;

    [Space(10)]
    [SerializeReference] public List<IApplicationEffect> effectsToApplyOnTargetOnCast = new();

    private AffectRule? compiledAffectRule = null;

    private AffectRule CompiledAffectRule
    {
        get
        {
            if (compiledAffectRule == null)
            {
                AffectRule combined = AffectRule.None;

                foreach (IApplicationEffect effect in effectsToApplyOnTargetOnCast)
                {
                    combined |= effect.AffectRule;
                }

                compiledAffectRule = combined;
            }

            return compiledAffectRule.Value;
        }
    }

    public bool CanTarget(GameObject candidate)
    {
        return FactionManager.Instance.CanAffect(owner, candidate, CompiledAffectRule);
    }

    public void CastOnTarget(GameObject target)
    {
        foreach (IApplicationEffect effect in effectsToApplyOnTargetOnCast)
        {
            effect.ApplyEffect(target, owner, rootAbility);
        }
    }

    public void StartWaitForInput(Vector3 mouseWorldPos)
    {

    }

    public void UpdateWaitForInput(Vector3 mouseWorldPos, bool confirmButtonPressed)
    {
        if (!confirmButtonPressed) return;

        GameObject selectedUnit = TryFindTargetableUnit(mouseWorldPos);
        if (selectedUnit != null)
        {
            CastOnTarget(selectedUnit);
        }
        else
        {
            OnInvalidTarget(); // Show message if invalid
            return; // Don't cast ability or reset cooldown
        }
    }

    public void ConcludeWaitForInput(Vector3 mouseWorldPos)
    {
        
    }

    public void CancelWaitForInput()
    {
        
    }

    public void OnInvalidTarget()
    {
        Debug.Log("You can't do that.");
    }

    private GameObject TryFindTargetableUnit(Vector3 mouseWorldPos)
    {
        float searchRadius = 1.2f;
        Vector3 center = mouseWorldPos;

        Collider[] hits = Physics.OverlapSphere(center, searchRadius, ~LayerMask.GetMask("Environment", "Ground"));

        GameObject closest = null;
        float closestDistanceSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            GameObject go = hit.gameObject;
            if (!CanTarget(go)) continue;

            float distanceSqr = (go.transform.position - center).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closest = go;
                closestDistanceSqr = distanceSqr;
            }
        }

        return closest;
    }
}
