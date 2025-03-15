using UnityEngine;

public class ArcherShootState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherShootState] Entering Ranged Attack");
        enemy.animator.SetBool("isAttacking", true);
        enemy.animator.SetTrigger("Shoot");
    }

    public override void UpdateState(EnemyFSM enemy)
    {

    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }
}
