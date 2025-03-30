using System.Collections.Generic;
using UnityEngine;

public class RadiusAroundSelfModule : AbilityModule, IInstantCastModule
{
    public float radius;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;
    [Space(10)]
    [SerializeReference] public List<IAbilityEffect> effectsInRadiusOnCast = new();

    public void InstantCast()
    {
        if (owner.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner.gameObject, rootAbility);
            }
        }

        foreach (IAbilityEffect effect in effectsInRadiusOnCast)
        {
            Collider[] collidersInRadius = Physics.OverlapSphere(owner.transform.position, radius, ~LayerMask.GetMask("Player", "Environment")); //means do not include player
            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                effect.ApplyEffect(collidersInRadius[i].gameObject, owner, rootAbility);
            }
        }
    }
}