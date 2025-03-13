using UnityEngine;
using System.Collections;

public class EnemyGotHitState : EnemyBaseState
{
    private float gotHitDuration = 0.8f;  // Duration of the hit reaction
    private float timer;

    public override void EnterState(EnemyFSM enemy)
    {
        // Trigger the got hit animation
        enemy.animator.SetTrigger("GotHit");
        timer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        timer += Time.deltaTime;
        if (timer >= gotHitDuration)
        {
            // After reacting to the hit, transition back to Idle
            enemy.TransitionToState(enemy.idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Optional: reset any hit flags here if needed.
    }
}
