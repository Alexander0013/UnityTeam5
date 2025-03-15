using UnityEngine;
using System.Collections;

public class BossMeleeAttackState : EnemyBaseState
{
    private bool isAttacking;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[BossMeleeAttackState] Entering Melee Attack");
        enemy.animator.SetBool("isAttacking", true);
        // Trigger the swipe animation. Ensure your Animator has a "Swipe" trigger.
        enemy.animator.SetTrigger("Swipe");
        isAttacking = false;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null)
            return;
        
        if (!isAttacking)
        {
            isAttacking = true;
            enemy.StartCoroutine(PerformSwipe(enemy));
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }

    private IEnumerator PerformSwipe(EnemyFSM enemy)
    {
        // Wait until the swipe animation reaches its hit frame.
        yield return new WaitForSeconds(2.0f);
        if (enemy.currentState is BossMeleeAttackState)
        {
            enemy.ApplyAttackDamage();
        }
        
        // Now smoothly rotate the boss to face the player.
        if (enemy.playerTarget != null)
        {
            yield return enemy.StartCoroutine(SmoothRotateToPlayer(enemy, 0.5f));
        }
        
        // Wait for the remainder of the animation if needed.
        yield return new WaitForSeconds(0.5f);
        enemy.TransitionToState(((FinalBossFSM)enemy).idleState);
    }

    private IEnumerator SmoothRotateToPlayer(EnemyFSM enemy, float duration)
    {
        Quaternion initialRotation = enemy.transform.rotation;
        // Compute direction to player, ignoring vertical differences.
        Vector3 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            enemy.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        enemy.transform.rotation = targetRotation;
    }
}
