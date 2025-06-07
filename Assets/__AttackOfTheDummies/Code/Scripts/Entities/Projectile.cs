using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolReleasable
{
    [HideInInspector] public float travelSpeed;
    [HideInInspector] public float lifetime;

    [HideInInspector] public IAbilitiesHolder projectileOwner;
    [HideInInspector] public Ability abilityRoot;
    [HideInInspector] public Action releaseFunction { get; set; }

    [SerializeReference] public List<IApplicationEffect> effectsToApplyOnTargetsOnContact = new List<IApplicationEffect>();

    private Rigidbody rb => GetComponent<Rigidbody>();

    public void OnEnable()
    {
        rb.AddForce(transform.forward * travelSpeed);
        Invoke(nameof(Deactivate), lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        foreach (IApplicationEffect effect in effectsToApplyOnTargetsOnContact)
        {
            effect.ApplyEffect(other.gameObject, projectileOwner, abilityRoot);
        }

        CancelInvoke(nameof(Deactivate));
        Deactivate();
    }

    private void Deactivate()
    {
        rb.linearVelocity = Vector3.zero;
        releaseFunction();
    }
}
