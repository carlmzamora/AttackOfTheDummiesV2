using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : HealthEntity//, IProjectileSource
{
    [Header("EnemyAI")]
    public float baseMoveSpeed;

    [SerializeField] private Transform projectileSpawnPoint;
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    [SerializeField] private Transform destination1;
    [SerializeField] private Transform destination2;
    private NavMeshAgent agent;
    private Transform currentDestination;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = baseMoveSpeed;

        if(destination1)
        {
            agent.SetDestination(destination1.position);
            currentDestination = destination1;
        }
    }

    public void Update()
    {
        if (agent.remainingDistance < 1 && destination1 && destination2)
        {
            if (currentDestination == destination1)
            {
                agent.SetDestination(destination2.position);
                currentDestination = destination2;
            }
            else
            {
                agent.SetDestination(destination1.position);
                currentDestination = destination1;
            }
        }
    }
}
