using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetModule : AbilityModule, IInstantCastModule
{
    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;

    public void InstantCast()
    {
        if (owner.gameObject.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner, rootAbility);
            }
        }
    }
}