using UnityEngine;

public class ArcherIdleState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        Debug.Log("[ArcherIdleState] Entering Idle (Aiming) State");
        // Set animator to idle/aiming loop.
        archer.animator.SetBool("isIdle", true);
        archer.animator.SetBool("isShooting", false);
        // Reset shoot timer.
        archer.shootTimer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null || archer.isDead) return;
        if (archer.playerTarget == null) return;
        
        float distance = Vector3.Distance(archer.transform.position, archer.playerTarget.position);
        // Remain in idle if the target is in detection range.
        if (distance <= archer.detectionRadius)
        {
            // Increment shoot timer.
            archer.shootTimer += Time.deltaTime;
            if (archer.shootTimer >= archer.shootCooldown)
            {
                // Time to shoot: transition to shoot state.
                archer.TransitionToState(archer.shootState);
            }
        }
        else
        {
            // Optionally, if the player moves out of range, transition to Return state.
            archer.TransitionToState(archer.returnState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        archer.animator.SetBool("isIdle", false);
    }
}
