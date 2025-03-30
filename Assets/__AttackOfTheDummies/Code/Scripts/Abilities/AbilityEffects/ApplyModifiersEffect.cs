using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApplyModifiersEffect : IAbilityEffect
{
    [SerializeReference] public List<Modifier> modifiers = new();

    public void ApplyEffect(GameObject target, GameObject source, Ability abilityRoot)
    {
        if (target.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in modifiers)
            {
                modController.ApplyModifier(mod, source, abilityRoot);
            }
        }
    }
}
