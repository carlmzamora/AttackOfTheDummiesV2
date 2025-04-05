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

    public float incomingDamageMultiplier = 1f;
    public float incomingHealMultiplier = 1f;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage;

        finalDamage *= incomingDamageMultiplier;

        CurrentHealth -= finalDamage;
    }

    public void ApplyHeal(float heal)
    {
        float finalHeal = heal;

        finalHeal *= incomingHealMultiplier;

        CurrentHealth += finalHeal;
    }
}
