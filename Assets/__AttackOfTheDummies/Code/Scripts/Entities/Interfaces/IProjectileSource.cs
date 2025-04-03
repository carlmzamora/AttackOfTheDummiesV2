using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileSource
{
    public Transform ProjectileSpawnPoint { get; }
}
