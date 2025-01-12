using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public float travelSpeed;
    [HideInInspector] public float lifetime;

    [HideInInspector] public Action releaseSelf;
    private Rigidbody rb => GetComponent<Rigidbody>();

    public void OnEnable()
    {
        rb.AddForce(transform.forward * travelSpeed);
        Invoke(nameof(ReleaseSelfToObjectPooler), lifetime);
    }

    private void ReleaseSelfToObjectPooler()
    {
        releaseSelf();
    }
}
