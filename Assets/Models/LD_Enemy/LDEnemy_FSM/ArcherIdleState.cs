using UnityEngine;

public class ArcherIdleState : EnemyBaseState
{
    private float guardRange = 10f;
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherIdleState] Entering Idle");
        enemy.animator.SetBool("isAttacking", false);
        enemy.animator.SetBool("isWalking", false);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;
        
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        if (distance < 2f)
        {
            enemy.TransitionToState(((ArcherFSM)enemy).meleeState);
        }
        else if (distance <= guardRange)
        {
            enemy.TransitionToState(((ArcherFSM)enemy).shootState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        
    }
}
