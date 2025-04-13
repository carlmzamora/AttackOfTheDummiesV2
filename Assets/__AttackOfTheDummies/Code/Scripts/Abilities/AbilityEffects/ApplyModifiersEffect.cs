using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApplyModifiersEffect : AbilityApplicationEffectBase
{
    [SerializeReference] public List<Modifier> modifiers = new();    

    public override void ApplyEffect(GameObject target, IAbilitiesHolder source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, target, AffectRule)) return;

        if (target.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in modifiers)
            {
                Modifier modInstance = mod.Clone();
                modController.ApplyModifier(modInstance, source, abilityRoot);
            }
        }
    }
}
