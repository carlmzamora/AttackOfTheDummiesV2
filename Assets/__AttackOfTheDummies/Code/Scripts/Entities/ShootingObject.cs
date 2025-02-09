using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingObject : MonoBehaviour, IProjectileSource
{
    [SerializeField] private ShootAbility shootAbility;
    [SerializeField] private Transform projectileSpawnPoint;

    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    public void Start()
    {
        shootAbility.Setup(gameObject);
        InvokeRepeating(nameof(Shoot), 1, 1);
    }

    private void Shoot()
    {
        shootAbility.Activate();
    }
}
