using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveSpeedBoostModifier : Modifier
{
    [FoldingHeader("MoveSpeedBoost")]
    public float boostPercent;
    public float baseDuration;

    [HideInInspector, Manipulable] public float boostPercentAdditive;
    [HideInInspector, Manipulable] public float boostDurationAdditive;

    private float currentBoostPercent;
    private float currentDuration;

    private NavMeshAgent aiAgent;
    private EnemyAI aiEntity;
    private PlayerController playerController;

    private float boostPerStack = 0;

    public override Modifier Clone()
    {
        Modifier copy = (MoveSpeedBoostModifier)MemberwiseClone();
        return copy;
    }

    public override void Instantiate()
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        aiEntity = affected.GetComponent<EnemyAI>();
        playerController = affected.GetComponent<PlayerController>();

        currentBoostPercent = boostPercent + GetFloatParameter(nameof(boostPercentAdditive));
        currentDuration = totalDuration + GetFloatParameter(nameof(boostDurationAdditive));

        if (aiAgent)
            boostPerStack = aiEntity.baseMoveSpeed * currentBoostPercent * 0.01f;

        if (playerController)
            boostPerStack = playerController.baseMoveSpeed * currentBoostPercent * 0.01f;

        base.Instantiate();
    }

    public override void AddStack()
    {
        if (aiAgent)
        {
            aiAgent.speed += boostPerStack;
        }

        if (playerController)
        {
            playerController.currentMoveSpeed += boostPerStack;
        }

        base.AddStack();
    }

    protected override void Expire()
    {
        if (aiAgent)
        {
            aiAgent.speed -= boostPerStack * currentStacks;
        }

        if (playerController)
        {
            playerController.currentMoveSpeed -= boostPerStack * currentStacks;
        }

        base.Expire();
    }
}
