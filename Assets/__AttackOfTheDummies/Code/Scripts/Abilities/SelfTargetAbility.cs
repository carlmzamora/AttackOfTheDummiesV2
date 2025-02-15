using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Abilities/Active/SelfTarget_Instant")]
public class SelfTargetAbility : ActiveAbility
{
    [Header("SelfTargetAbility")]
    public float selfDamage;

    [Space(10)]
    [SerializeReference, HideFactionMask] public List<Modifier> selfModifiersOnCast;

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);
        instantCast = true;
    }

    public override void InstantCast()
    {
        if (owner.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(selfDamage);
        }

        if (owner.TryGetComponent(out ModifiersController modController))
        {
            foreach (Modifier mod in selfModifiersOnCast)
            {
                modController.ApplyModifier(mod, owner.gameObject, this);
            }
        }
    }
}
