using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class UnitTargetModule : AbilityModule, IUnitTargetCastModule
{
    public bool hasGlobalCastRange;
    public float castRange;
    public AffectRule affectRule;

    [Space(10)]
    [SerializeReference, HideAffectRule] public List<IAbilityEffect> effectsOnTargetOnCast = new();

    public bool CanTarget(GameObject candidate)
    {
        Debug.Log($"{candidate}: {FactionManager.Instance.CanAffect(owner, candidate, affectRule)}");
        return FactionManager.Instance.CanAffect(owner, candidate, affectRule);
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
