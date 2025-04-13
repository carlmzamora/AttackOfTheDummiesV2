using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTargetModule : AbilityModule, ITargetedCastModule
{
    public float radius;
    public bool hasGlobalCastRange;
    public float castRange;

    [Space(10)]
    [SerializeReference, HideAffectRule] public List<IApplicationEffect> effectsToApplyOnSelfOnCast = new();
    [Space(10)]
    [SerializeReference] public List<IApplicationEffect> effectsToApplyToTargetsInRadiusOnCast = new();

    public void UpdateWaitForInputDisplay(Vector2 worldPos)
    {

    }

    public void EndWaitForInput(Vector2 worldPos)
    {
        foreach (IApplicationEffect effect in effectsToApplyOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }

        Collider[] collidersInRadius = Physics.OverlapSphere(worldPos, radius);

        foreach (IApplicationEffect effect in effectsToApplyToTargetsInRadiusOnCast)
        {
            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                effect.ApplyEffect(collidersInRadius[i].gameObject, owner, rootAbility);
            }
        }
    }
}
