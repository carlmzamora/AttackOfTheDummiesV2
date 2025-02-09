using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Abilities/RadiusCast")]
public class RadiusCastAbility : ActiveAbility
{
    [Header("Radius Cast Ability")]
    public float radius;
    public float castDistance;

    [Header("Damage")]
    public float damage;
    //modifier on affected

    //Header("Effects")]
    //cast effect

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);
    }

    public override void InstantCast()
    {
        Collider[] allAffected = Physics.OverlapSphere(owner.transform.position + (owner.transform.forward * castDistance), radius);

        if (allAffected.Length <= 0) return;

        foreach(Collider collider in allAffected)
        {
            if(collider.TryGetComponent(out HealthEntity healthEntity))
            {
                healthEntity.TakeDamage(damage);
            }
        }
    }

    public override void UpdateWaitForInput(Vector2 worldPos, bool mouse1Pressed)
    {
        //get world position via mouse position
        if(mouse1Pressed)
        {
            Collider[] allAffected = Physics.OverlapSphere(worldPos, radius);

            if (allAffected.Length <= 0) return;

            foreach (Collider collider in allAffected)
            {
                if (collider.TryGetComponent(out HealthEntity healthEntity))
                {
                    healthEntity.TakeDamage(damage);
                }
            }
        }
    }
}
