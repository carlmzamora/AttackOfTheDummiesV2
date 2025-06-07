using System.Collections.Generic;
using UnityEngine;

public class RadiusAroundSelfModule : AbilityModule, IInstantCastModule
{
    public float radius;

    [Space(10)]
    [SerializeReference, HideAffectRule] public List<IApplicationEffect> effectsToApplyOnSelfOnCast = new();
    [Space(10)]
    [SerializeReference] public List<IApplicationEffect> effectsToApplyToTargetsInRadiusOnCast = new();

    public void InstantCast()
    {
        foreach (IApplicationEffect effect in effectsToApplyOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }

        Collider[] collidersInRadius = Physics.OverlapSphere(owner.transform.position, radius, ~LayerMask.GetMask("Environment", "Ground")); //means do not include player

        foreach (IApplicationEffect effect in effectsToApplyToTargetsInRadiusOnCast)
        {
            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                effect.ApplyEffect(collidersInRadius[i].gameObject, owner, rootAbility);
            }
        }
    }
}