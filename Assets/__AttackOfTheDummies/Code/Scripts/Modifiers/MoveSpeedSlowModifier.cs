using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Tomadle/Modifiers/Slow")]
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
    private PlayerController playerController;

    private float slowPerStack = 0;

    public override void Instantiate(bool timedStacks)
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        playerController = affected.GetComponent<PlayerController>();

        currentSlowPercent = slowPercent + abilityRoot.floatParameters[nameof(slowPercentAdditive)];
        currentDuration = duration + abilityRoot.floatParameters[nameof(durationAdditive)];

        if (aiAgent)
            slowPerStack = aiAgent.speed * currentSlowPercent * 0.01f;

        if (playerController)
            slowPerStack = playerController.currentMoveSpeed * currentSlowPercent * 0.01f;

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
        }

        if(playerController)
        {
            playerController.currentMoveSpeed -= slowPerStack;
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
        }

        if(playerController)
        {
            playerController.currentMoveSpeed += slowPerStack * currentStacks;
        }

        Expire();
    }
}
