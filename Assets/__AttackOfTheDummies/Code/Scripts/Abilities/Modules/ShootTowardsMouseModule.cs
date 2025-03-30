using System.Collections.Generic;
using UnityEngine;

public class ShootTowardsMouseModule : AbilityModule, IInstantCastModule
{
    public Projectile projectilePrefab;

    [Header("Physics")]
    public float projectileSpeed;
    public float lifetime;

    [Space(10)]
    [SerializeReference] public List<IAbilityEffect> effectsOnContact = new();

    private Transform spawnPoint;

    public override void Setup(GameObject owner)
    {
        base.Setup(owner);

        ObjectPooler.Instance.CreatePool(projectilePrefab.gameObject);

        if(owner.TryGetComponent(out IProjectileSource projectileSource))
        {
            spawnPoint = projectileSource.ProjectileSpawnPoint;
        }
    }

    public void InstantCast()
    {
        Projectile projectile = ObjectPooler.Instance.GetPooledObject(projectilePrefab.gameObject).GetComponent<Projectile>();

        projectile.projectileOwner = owner;
        projectile.abilityRoot = rootAbility;

        projectile.transform.SetLocalPositionAndRotation(spawnPoint.position + spawnPoint.forward, spawnPoint.rotation);
        projectile.gameObject.layer = LayerMask.NameToLayer(owner.gameObject.layer == LayerMask.NameToLayer("Enemy") ? "EnemyProjectiles" : "PlayerProjectiles");

        projectile.travelSpeed = projectileSpeed;
        projectile.lifetime = lifetime;
        projectile.effectsOnContact = new(effectsOnContact);

        projectile.gameObject.SetActive(true);
    }
}
