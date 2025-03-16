using UnityEngine;

public class ArcherIdleState : EnemyBaseState
{
    // Overall guard range in which the archer will react.
    private float guardRange = 10f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherIdleState] Entering Idle");
        enemy.animator.SetBool("isAttacking", false);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null)
            return;
        
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        
        // Use npcData.hitRadius for close-range melee.
        if (distance <= enemy.npcData.hitRadius)
        {
            enemy.TransitionToState(((ArcherFSM)enemy).meleeState);
        }
        // If within guard range but not too close, shoot.
        else if (distance <= guardRange)
        {
            enemy.TransitionToState(((ArcherFSM)enemy).shootState);
        }
        // Otherwise, remain idle.
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Nothing to clean up.
    }
}
