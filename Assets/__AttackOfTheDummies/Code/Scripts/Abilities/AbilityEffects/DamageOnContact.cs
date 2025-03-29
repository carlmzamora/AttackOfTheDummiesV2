using System;
using UnityEngine;

[Serializable]
public class DamageOnContact : IContactEffect
{
    public float damage;

    public void OnContact(GameObject hitObject, Projectile projectile)
    {
        if (hitObject.TryGetComponent(out HealthEntity healthEntity))
        {
            healthEntity.TakeDamage(damage);
        }
    }
}
