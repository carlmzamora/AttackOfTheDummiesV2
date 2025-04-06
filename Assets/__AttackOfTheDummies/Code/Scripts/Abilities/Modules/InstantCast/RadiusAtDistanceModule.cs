using System.Collections.Generic;
using UnityEngine;

public class RadiusAtDistanceModule : AbilityModule, IInstantCastModule
{
    public float radius;
    public float castDistance;
    public float radiusDamage;

    [Space(10)]
    [SerializeReference, HideAffectRule] public List<IAbilityEffect> effectsOnSelfOnCast = new();
    [Space(10)]
    [SerializeReference] public List<IAbilityEffect> effectsInRadiusOnCast = new();

    public void InstantCast()
    {
        foreach (IAbilityEffect effect in effectsOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }

        Vector3 targetPoint = owner.transform.position + (owner.transform.forward * castDistance);

        Collider[] collidersInRadius = Physics.OverlapSphere(targetPoint, radius, ~LayerMask.GetMask("Player", "Environment")); //means do not include player

        foreach (IAbilityEffect effect in effectsInRadiusOnCast)
        {
            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                effect.ApplyEffect(collidersInRadius[i].gameObject, owner, rootAbility);
            }
        }
    }
}