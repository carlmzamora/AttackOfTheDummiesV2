using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Shooter")]
public class ShootAbility : Ability
{
    public Projectile projectilePrefab;

    [Header("Physics")]
    public float projectileSpeed;
    public float lifetime;

    [Header("Damage")]
    public float damage;

    private Transform spawnPoint;

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);
        ObjectPooler.Instance.CreatePool(projectilePrefab.gameObject);

        if(owner.TryGetComponent(out IProjectileSource projectileSource))
        {
            spawnPoint = projectileSource.ProjectileSpawnPoint;
        }
    }

    public override void Activate()
    {
        Projectile projectile = ObjectPooler.Instance.GetPooledObject(projectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.releaseSelf = () => ObjectPooler.Instance.ReleasePooledObject(projectilePrefab.gameObject, projectile.gameObject);

        projectile.transform.SetLocalPositionAndRotation(spawnPoint.position + spawnPoint.forward, spawnPoint.rotation);
        projectile.travelSpeed = projectileSpeed;
        projectile.lifetime = lifetime;
        projectile.damage = damage;

        projectile.gameObject.SetActive(true);
    }
}
