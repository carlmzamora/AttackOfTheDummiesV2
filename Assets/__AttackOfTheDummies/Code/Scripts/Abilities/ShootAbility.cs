using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tomadle/Shooter")]
public class ShootAbility : Ability
{
    public Projectile projectilePrefab;
    public float projectileSpeed;
    public float lifetime;

    public override void Setup(GameObject owner, int level = -1)
    {
        base.Setup(owner, level);
        ObjectPooler.Instance.CreatePool(projectilePrefab.gameObject);
    }

    public override void Activate()
    {
        Projectile projectile = ObjectPooler.Instance.GetPooledObject(projectilePrefab.gameObject).GetComponent<Projectile>();

        projectile.transform.SetLocalPositionAndRotation(owner.transform.position + owner.transform.forward, owner.transform.rotation);
        projectile.travelSpeed = projectileSpeed;
        projectile.lifetime = lifetime;
        projectile.releaseSelf = () => ObjectPooler.Instance.ReleasePooledObject(projectilePrefab.gameObject, projectile.gameObject);
        projectile.gameObject.SetActive(true);
    }
}
