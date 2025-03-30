using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherGotHitState : EnemyBaseState
{
    private float timer = 0f;
    private float gotHitDuration = 0.8f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherGotHitState] Archer got hit");
        enemy.animator.SetTrigger("GotHit");
        timer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        timer += Time.deltaTime;
        if (timer >= gotHitDuration)
        {
            enemy.TransitionToState(((ArcherFSM)enemy).idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
    }
}

