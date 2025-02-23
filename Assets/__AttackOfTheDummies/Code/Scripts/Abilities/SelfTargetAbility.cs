using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetAbility : IInstantCastAbility
{
    public ActiveAbility rootAbility { get; set; }

    public float selfDamage;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;


    public void Setup(GameObject owner, int level = -1)
    {
        
    }

    public void InstantCast(GameObject caster)
    {
        if (caster.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(selfDamage);
        }

        if (caster.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, caster.gameObject, rootAbility);
            }
        }
    }
}
