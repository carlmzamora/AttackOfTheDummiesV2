using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolReleasable
{
    [HideInInspector] public float travelSpeed;
    [HideInInspector] public float lifetime;

    [HideInInspector] public GameObject projectileOwner;
    [HideInInspector] public Ability abilityRoot;
    [HideInInspector] public Action releaseFunction { get; set; }

    [SerializeReference] public List<IContactEffect> effectsOnContact = new List<IContactEffect>();

    private Rigidbody rb => GetComponent<Rigidbody>();

    public void OnEnable()
    {
        rb.AddForce(transform.forward * travelSpeed);
        Invoke(nameof(Deactivate), lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {

        foreach (IContactEffect effect in effectsOnContact)
        {
            effect.OnContact(other.gameObject, this);
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
