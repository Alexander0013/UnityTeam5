using UnityEngine;
using System.Collections;

public class BossRangedAttackState : EnemyBaseState
{
    private bool isAttacking;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[BossRangedAttackState] Entering Ranged Attack");
        enemy.animator.SetBool("isAttacking", true);
        // Trigger the roar attack (ensure you have a "Roar" trigger in your Animator)
        enemy.animator.SetTrigger("Roar");
        isAttacking = false;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;
        if (!isAttacking)
        {
            isAttacking = true;
            enemy.StartCoroutine(PerformRangedAttack(enemy));
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }

    private IEnumerator PerformRangedAttack(EnemyFSM enemy)
    {
        // Wait until the roar animation reaches the damage frame.
        yield return new WaitForSeconds(1.2f);
        if (enemy.currentState is BossRangedAttackState)
        {
            enemy.ApplyAttackDamage();
        }
        // Wait for the remainder of the animation.
        yield return new WaitForSeconds(0.8f);
        enemy.TransitionToState(((FinalBossFSM)enemy).idleState);
    }
}
