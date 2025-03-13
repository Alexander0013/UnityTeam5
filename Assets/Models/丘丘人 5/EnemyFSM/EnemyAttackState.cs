using UnityEngine;
using System.Collections;

public class EnemyAttackState : EnemyBaseState
{
    private bool isAttacking;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter Attack State");
        enemy.animator.SetBool("isWalking", false);
        //enemy.animator.SetTrigger("Attack");
        isAttacking = false;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;

        // Start the attack coroutine only if not already attacking.
        if (!isAttacking)
        {
            isAttacking = true;
            enemy.StartCoroutine(PerformAttack(enemy));

        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }

    private IEnumerator PerformAttack(EnemyFSM enemy)
    {
        // Trigger the attack animation.
        enemy.animator.SetTrigger("Attack");

        // Wait for the attack animation to reach its hit frame.
        yield return new WaitForSeconds(0.4f);

        // Check if the enemy's current state is still AttackState.
        // If the enemy was hit during its attack, it might have transitioned
        // to a "GotHit" or "Idle" state. In that case, cancel the attack.
        if (enemy.currentState != this)
        {
            // Attack was interrupted; do not apply damage.
            yield break;
        }
        enemy.ApplyAttackDamage();

        // Transition back to idle after the attack.
        enemy.TransitionToState(enemy.idleState);
    }
}
