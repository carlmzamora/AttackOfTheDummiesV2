using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEntity : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }
}
