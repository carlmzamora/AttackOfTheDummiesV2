using UnityEngine;

public interface IContactResult
{
    public void OnContact(GameObject hitObject, Projectile projectile);
}