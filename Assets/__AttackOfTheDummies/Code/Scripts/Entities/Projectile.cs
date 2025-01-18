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

    [HideInInspector] public Action releaseSelf;
    private Rigidbody rb => GetComponent<Rigidbody>();

    public void OnEnable()
    {
        rb.AddForce(transform.forward * travelSpeed);
        Invoke(nameof(ReleaseSelfToObjectPooler), lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(damage);
        }

        CancelInvoke(nameof(ReleaseSelfToObjectPooler));
        ReleaseSelfToObjectPooler();
    }

    private void ReleaseSelfToObjectPooler()
    {
        rb.velocity = Vector3.zero;
        releaseSelf();
    }
}
