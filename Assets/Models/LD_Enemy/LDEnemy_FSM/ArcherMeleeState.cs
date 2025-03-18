using UnityEngine;

public class ArcherMeleeState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherMeleeState] Entering Melee Attack");
        // Face the player immediately.
        if(enemy.playerTarget != null)
        {
            Vector3 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
            direction.y = 0f;
            enemy.transform.rotation = Quaternion.LookRotation(direction);
        }
        enemy.animator.SetBool("isAttacking", true);
        enemy.animator.SetTrigger("MeleeKick");
    }

    public override void UpdateState(EnemyFSM enemy)
    {

    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }
}
