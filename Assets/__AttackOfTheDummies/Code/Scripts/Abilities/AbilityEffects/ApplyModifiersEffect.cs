using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApplyModifiersEffect : IAbilityEffect
{
    [Tooltip("What factions do these modifier affect?")]
    public AffectRule affectRule;
    [SerializeReference] public List<Modifier> modifiers = new();    

    public void ApplyEffect(GameObject target, GameObject source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, affectRule)) return;

        if (target.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in modifiers)
            {
                modController.ApplyModifier(mod, source, abilityRoot);
            }
        }
    }
}
