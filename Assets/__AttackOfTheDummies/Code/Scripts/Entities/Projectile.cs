using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolReleasable
{
    [HideInInspector] public float travelSpeed;
    [HideInInspector] public float lifetime;
    [HideInInspector] public float damage;

    [HideInInspector] public List<Modifier> modifiersOnContact;

    [HideInInspector] public GameObject projectileOwner;
    [HideInInspector] public Ability abilityRoot;
    [HideInInspector] public Action releaseFunction { get; set; }

    private Rigidbody rb => GetComponent<Rigidbody>();

    public void OnEnable()
    {
        rb.AddForce(transform.forward * travelSpeed);
        Invoke(nameof(Deactivate), lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(damage);
        }

        if(other.TryGetComponent(out ModifiersController modController))
        {
            foreach(Modifier mod in modifiersOnContact)
            {
                modController.ApplyModifier(mod, projectileOwner, abilityRoot);
            }
        }

        CancelInvoke(nameof(Deactivate));
        Deactivate();
    }

    private void Deactivate()
    {
        rb.velocity = Vector3.zero;
        releaseFunction();
    }
}
