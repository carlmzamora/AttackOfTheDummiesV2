using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveSpeedBoostModifier : Modifier
{
    [Header("MoveSpeedBoost")]
    public float boostPercent;
    public float duration;

    [HideInInspector, Manipulable] public float boostPercentAdditive;
    [HideInInspector, Manipulable] public float boostDurationAdditive;

    private float currentBoostPercent;
    private float currentDuration;

    private NavMeshAgent aiAgent;
    private EnemyAI aiEntity;
    private PlayerController playerController;

    private float boostPerStack = 0;

    public override void Instantiate(bool timedStacks)
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        aiEntity = affected.GetComponent<EnemyAI>();
        playerController = affected.GetComponent<PlayerController>();

        currentBoostPercent = boostPercent + abilityRoot.floatParameters[$"{abilityRoot.name}_{GetType()}_{nameof(boostPercentAdditive)}"];
        currentDuration = duration + abilityRoot.floatParameters[$"{abilityRoot.name}_{GetType()}_{nameof(boostDurationAdditive)}"];

        if (aiAgent)
            boostPerStack = aiEntity.baseMoveSpeed * currentBoostPercent * 0.01f;

        if (playerController)
            boostPerStack = playerController.baseMoveSpeed * currentBoostPercent * 0.01f;

        //we don't check currentDuration but duration,
        //so that accidental upgrading doesn't render infinite duration into limited duration
        if (duration > 0)
            affected.StartCoroutine(DurationCoroutine());

        base.Instantiate(timedStacks);
    }

    public override void AddStack(bool timedStacks)
    {
        if (aiAgent)
        {
            aiAgent.speed += boostPerStack;
        }

        if (playerController)
        {
            playerController.currentMoveSpeed += boostPerStack;
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
            aiAgent.speed -= boostPerStack * currentStacks;
        }

        if (playerController)
        {
            playerController.currentMoveSpeed -= boostPerStack * currentStacks;
        }

        Expire();
    }
}
