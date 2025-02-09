using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Abilities/SelfTarget_Instant")]
public class SelfTargetInstantAbility : ActiveAbility
{
    [Header("Damage")]
    public float damage;

    [Space(10)]
    [SerializeReference] public List<Modifier> selfModifiersOnCast;

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);
        instantCast = true;
    }

    public override void InstantCast()
    {
        if (owner.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(damage);
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
