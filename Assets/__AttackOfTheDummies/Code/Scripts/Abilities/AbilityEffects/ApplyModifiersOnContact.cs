using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApplyModifiersOnContact : IContactEffect
{
    [SerializeReference] public List<Modifier> modifiers = new();

    public void OnContact(GameObject hitObject, Projectile projectile)
    {
        if (hitObject.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in modifiers)
            {
                modController.ApplyModifier(mod, projectile.projectileOwner, projectile.abilityRoot);
            }
        }
    }
}
