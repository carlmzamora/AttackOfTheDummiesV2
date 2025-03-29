using UnityEngine;

public interface IContactEffect : IAbilityEffect
{
    public void OnContact(GameObject hitObject, Projectile projectile);
}