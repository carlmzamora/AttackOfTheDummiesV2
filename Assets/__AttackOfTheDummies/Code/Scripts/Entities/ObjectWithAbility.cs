using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithAbility : MonoBehaviour, IProjectileSource
{
    [SerializeField] private Transform projectileSpawnPoint;

    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    private AbilitiesController abilitiesController => GetComponent<AbilitiesController>();

    public void Start()
    {
        InvokeRepeating(nameof(Shoot), 1, 1);
    }

    private void Shoot()
    {
        abilitiesController.PerformMouse01();
    }
}
