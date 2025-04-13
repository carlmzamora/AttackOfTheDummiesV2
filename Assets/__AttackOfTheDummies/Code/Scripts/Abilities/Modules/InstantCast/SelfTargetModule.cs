using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTargetModule : AbilityModule, IInstantCastModule
{
    [SerializeReference, HideAffectRule] public List<IApplicationEffect> effectsToApplyOnSelfOnCast = new();

    public void InstantCast()
    {
        foreach (IApplicationEffect effect in effectsToApplyOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }
    }
}