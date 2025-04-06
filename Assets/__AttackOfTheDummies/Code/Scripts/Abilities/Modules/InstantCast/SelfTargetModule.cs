using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetModule : AbilityModule, IInstantCastModule
{
    [SerializeReference, HideAffectRule] public List<IAbilityEffect> effectsOnSelfOnCast = new();

    public void InstantCast()
    {
        foreach (IAbilityEffect effect in effectsOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }
    }
}