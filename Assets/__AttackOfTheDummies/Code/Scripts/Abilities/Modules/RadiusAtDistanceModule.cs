using System.Collections.Generic;
using UnityEngine;

public class RadiusAtDistanceModule : AbilityModule, IInstantCastModule
{
    public float radius;
    public float castDistance;
    public float radiusDamage;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;
    [Space(10)]
    [SerializeReference] public List<Modifier> modifiersAppliedInRadiusOnCast;

    public void InstantCast()
    {
        if (owner.gameObject.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner, rootAbility);
            }
        }

        Vector3 targetPoint = owner.transform.position + (owner.transform.forward * castDistance);

        Collider[] collidersInRadius = Physics.OverlapSphere(targetPoint, radius, ~LayerMask.GetMask("Player")); //means do not include player
        for (int i = 0; i < collidersInRadius.Length; i++)
        {
            if (collidersInRadius[i].TryGetComponent(out ModifiersController otherModController))
            {
                for (int j = 0; j < modifiersAppliedInRadiusOnCast.Count; j++)
                {
                    otherModController.ApplyModifier(modifiersAppliedInRadiusOnCast[j], owner, rootAbility);
                }
            }
        }
    }
}