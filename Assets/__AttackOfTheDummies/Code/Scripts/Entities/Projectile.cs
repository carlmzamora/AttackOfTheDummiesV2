using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public float travelSpeed;
    [HideInInspector] public float lifetime;
    [HideInInspector] public float damage;

    [HideInInspector] public Modifier[] modifiersOnContact;

    [HideInInspector] public Action poolerReleaseFunction;
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
                modController.ApplyModifier(mod, gameObject);
            }
        }

        CancelInvoke(nameof(Deactivate));
        Deactivate();
    }

    private void Deactivate()
    {
        rb.velocity = Vector3.zero;
        poolerReleaseFunction();
    }
}
