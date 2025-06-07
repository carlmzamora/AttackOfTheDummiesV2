using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveSpeedSlowModifier : Modifier
{
    [FoldingHeader("MoveSpeedSlow")]
    public float slowPercent;
    public float baseDuration;

    [HideInInspector, Manipulable] public float slowPercentAdditive;
    [HideInInspector, Manipulable] public float slowDurationAdditive;

    private float currentSlowPercent;
    private float currentDuration;

    private NavMeshAgent aiAgent;
    private EnemyAI aiEntity;
    private PlayerController playerController;

    private float slowPerStack = 0;

    public override Modifier Clone()
    {
        Modifier copy = (MoveSpeedSlowModifier)MemberwiseClone();
        return copy;
    }

    public override void Instantiate()
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        aiEntity = affected.GetComponent<EnemyAI>();
        playerController = affected.GetComponent<PlayerController>();

        currentSlowPercent = slowPercent + GetFloatParameter(nameof(slowPercentAdditive));
        currentDuration = baseDuration + GetFloatParameter(nameof(slowDurationAdditive));

        if (aiAgent)
            slowPerStack = aiEntity.baseMoveSpeed * currentSlowPercent * 0.01f;

        if (playerController)
            slowPerStack = playerController.baseMoveSpeed * currentSlowPercent * 0.01f;

        base.Instantiate();
    }

    public override void AddStack()
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

        base.AddStack();
    }

    protected override void Expire()
    {
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

        base.Expire();
    }
}