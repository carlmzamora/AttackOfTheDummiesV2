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

    public void StartWaitForInput(Vector2 mouseWorldPos)
    {

    }

    public void UpdateWaitForInputDisplay(Vector2 mouseWorldPos)
    {
        
    }

    public void EndWaitForInput(Vector2 mouseWorldPos)
    {
        
    }

    public void OnInvalidTarget()
    {
        Debug.Log("You can't do that.");
    }
}
