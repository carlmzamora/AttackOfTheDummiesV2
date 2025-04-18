using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIWithAbility : EnemyAI, IAbilitiesHolder
{
    [SerializeField] private Transform projectileSpawnPoint;

    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    private AbilitiesController abilitiesController => GetComponent<AbilitiesController>();
    public AbilitiesController AbilitiesController => abilitiesController;

    public MonoBehaviour mono => GetComponent<MonoBehaviour>();

    public float outgoingDamageMultiplier { get; set; }
    public float outgoingHealMultiplier { get; set; }

    protected override void Start()
    {
        InvokeRepeating(nameof(Shoot), 1, 1);
    }

    protected override void Update()
    {
        
    }

    private void Shoot()
    {
        abilitiesController.Trigger(0);
    }
}
