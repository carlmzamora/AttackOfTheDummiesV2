using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class UnitTargetModule : AbilityModule, IUnitTargetCastModule
{
    public bool hasGlobalCastRange;
    public float castRange;

    private AffectRule? compiledAffectRule = null;

    private AffectRule CompiledAffectRule
    {
        get
        {
            if (compiledAffectRule == null)
            {
                AffectRule combined = AffectRule.None;

                foreach (IAbilityEffect effect in effectsOnTargetOnCast)
                {
                    combined |= effect.AffectRule;
                }

                compiledAffectRule = combined;
            }

            return compiledAffectRule.Value;
        }
    }

    [Space(10)]
    [SerializeReference] public List<IAbilityEffect> effectsOnTargetOnCast = new();

    public bool CanTarget(GameObject candidate)
    {
        return FactionManager.Instance.CanAffect(owner, candidate, CompiledAffectRule);
    }

    public void CastOnTarget(GameObject target)
    {
        //trouble with custom affect rule conflicting with effect affect rule
        foreach (IAbilityEffect effect in effectsOnTargetOnCast)
        {
            effect.ApplyEffect(target, owner, rootAbility);
        }
    }

    public void UpdateWaitForInputDisplay(Vector2 worldPos)
    {
        
    }

    public void EndWaitForInput(Vector2 worldPos)
    {
        
    }

    public void OnInvalidTarget()
    {
        Debug.Log("You can't do that.");
    }
}
