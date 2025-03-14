using UnityEngine;

public class BossIdleState : EnemyBaseState
{
    private float decisionTimer;
    private float minDecisionTime = 0.5f;
    private float maxDecisionTime = 2f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[BossIdleState] Entering Idle");
        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isAttacking", false);
        decisionTimer = Random.Range(minDecisionTime, maxDecisionTime);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        decisionTimer -= Time.deltaTime;

        // If player is too far, switch to chase state immediately.
        if (distance > enemy.detectionRadius * 0.5f)
        {
            enemy.TransitionToState(((FinalBossFSM)enemy).chaseState);
            return;
        }

        if (decisionTimer <= 0)
        {
            // Decide attack type:
            if (distance <= enemy.attackRadius)
            {
                // Player is close: choose melee (swipe)
                enemy.TransitionToState(((FinalBossFSM)enemy).meleeAttackState);
            }
            else
            {
                // Otherwise, use a ranged attack (roar)
                enemy.TransitionToState(((FinalBossFSM)enemy).rangedAttackState);
            }
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Nothing special to clean up.
    }
}
