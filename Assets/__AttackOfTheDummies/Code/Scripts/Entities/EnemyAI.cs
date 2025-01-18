using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : HealthEntity, IProjectileSource
{
    [SerializeField] private Transform projectileSpawnPoint;
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;
}
