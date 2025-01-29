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

    private NavMeshAgent aiAgent;
    private PlayerController playerController;

    private float slowPerStack = 0;

    public override void Instantiate(bool timedStacks)
    {
        aiAgent = affected.GetComponent<NavMeshAgent>();
        playerController = affected.GetComponent<PlayerController>();

        if (aiAgent)
            slowPerStack = aiAgent.speed * slowPercent * 0.01f;

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

        base.AddStack(timedStacks);
    }

    private IEnumerator DurationCoroutine()
    {
        startTime = Time.time;
        while (Time.time - startTime < duration) //if duration lapsed is more than totalDuration
        {
            yield return new WaitForEndOfFrame();
        }

        if (aiAgent)
        {
            aiAgent.speed += slowPerStack * currentStacks;
        }

        Expire();
    }
}
