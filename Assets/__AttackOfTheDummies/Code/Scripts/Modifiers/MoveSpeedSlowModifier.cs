using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveSpeedSlowModifier : Modifier
{
    [Header("MoveSpeedSlow")]
    public float slowPercent;
    public float duration;

    [HideInInspector, Manipulable] public float slowPercentAdditive;
    [HideInInspector, Manipulable] public float durationAdditive;

    private float currentSlowPercent;
    private float currentDuration;

    private NavMeshAgent aiAgent;
    private EnemyAI aiEntity;
    private PlayerController playerController;

    private float slowPerStack = 0;

    public override void Instantiate(bool timedStacks)
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        aiEntity = affected.GetComponent<EnemyAI>();
        playerController = affected.GetComponent<PlayerController>();

        currentSlowPercent = slowPercent + abilityRoot.floatParameters[nameof(slowPercentAdditive)];
        currentDuration = duration + abilityRoot.floatParameters[nameof(durationAdditive)];

        if (aiAgent)
            slowPerStack = aiEntity.baseMoveSpeed * currentSlowPercent * 0.01f;

        if (playerController)
            slowPerStack = playerController.baseMoveSpeed * currentSlowPercent * 0.01f;

        //we don't check currentDuration but duration,
        //so that accidental upgrading doesn't render infinite duration into limited duration
        if (duration > 0)
            affected.StartCoroutine(DurationCoroutine());

        base.Instantiate(timedStacks);
    }

    public override void AddStack(bool timedStacks)
    {
        if(aiAgent)
        {
            aiAgent.speed -= slowPerStack;
            if(aiAgent.speed <= 0)
            {
                aiAgent.speed = 0;
            }
        }

        if(playerController)
        {
            playerController.currentMoveSpeed -= slowPerStack;
            if(playerController.currentMoveSpeed <= 0)
            {
                playerController.currentMoveSpeed = 0;
            }
        }

        base.AddStack(timedStacks);
    }

    private IEnumerator DurationCoroutine()
    {
        startTime = Time.time;
        while (Time.time - startTime < currentDuration) //if duration lapsed is more than totalDuration
        {
            yield return new WaitForEndOfFrame();
        }

        if (aiAgent)
        {
            aiAgent.speed += slowPerStack * currentStacks;
            if(aiAgent.speed > aiEntity.baseMoveSpeed)
            {
                aiAgent.speed = aiEntity.baseMoveSpeed;
            }
        }

        if(playerController)
        {
            playerController.currentMoveSpeed += slowPerStack * currentStacks;
        }

        Expire();
    }
}