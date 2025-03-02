using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusAroundSelfModule : AbilityModule, IInstantCastModule
{
    public float radius;
    public float radiusDamage;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;
    [Space(10)]
    [SerializeReference] public List<Modifier> modifiersAppliedInRadiusOnCast;

    public void InstantCast()
    {
        if (owner.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner.gameObject, rootAbility);
            }
        }

        Collider[] collidersInRadius = Physics.OverlapSphere(owner.transform.position, radius, ~LayerMask.GetMask("Player")); //means do not include player
        for(int i = 0; i < collidersInRadius.Length; i++)
        {
            if (collidersInRadius[i].TryGetComponent(out ModifiersController otherModController))
            {
                for(int j = 0; j < modifiersAppliedInRadiusOnCast.Count; j++)
                {
                    otherModController.ApplyModifier(modifiersAppliedInRadiusOnCast[j], owner.gameObject, rootAbility);
                }
            }
        }
    }
}