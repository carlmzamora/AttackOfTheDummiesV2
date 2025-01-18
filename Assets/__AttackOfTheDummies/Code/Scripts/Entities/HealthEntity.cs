using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEntity : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float currentHealth = 100;
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set { currentHealth = value; }
    }

    [SerializeField] private float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        private set { maxHealth = value; }
    }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }
}
