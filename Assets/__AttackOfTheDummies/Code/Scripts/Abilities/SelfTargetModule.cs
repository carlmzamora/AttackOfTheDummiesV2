using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetModule : AbilityModule, IInstantCastModule
{
    public float selfDamage;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;

    public void InstantCast()
    {
        if (owner.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(selfDamage);
        }

        if (owner.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner.gameObject, rootAbility);
            }
        }
    }
}