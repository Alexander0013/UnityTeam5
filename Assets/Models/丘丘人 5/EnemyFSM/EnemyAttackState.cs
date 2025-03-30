using UnityEngine;
public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        //Debug.Log("Enter EnemyAttack State");
        enemy.animator.SetBool("isWalking", false);
        // Start the attack animation.
        enemy.animator.SetTrigger("Attack");
    }
    public override void UpdateState(EnemyFSM enemy)
    {
        // No polling needed here�Xattack timing is controlled by animation events.
        // Get the player's position, but keep the enemy's y position
        Vector3 targetPos = enemy.playerTarget.position;
        targetPos.y = enemy.transform.position.y;

        // Calculate direction ignoring vertical difference
        Vector3 direction = (targetPos - enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }
    public override void ExitState(EnemyFSM enemy)
    {
        //enemy.animator.SetBool("isAttacking", false);
    }
    // Called by an Animation Event at the exact moment the attack should hit.
    public void OnAttackHit(EnemyFSM enemy)
    {
        // Apply damage only if still in attack state
        enemy.ApplyAttackDamage();
    }
    // Called by an Animation Event at the end of the attack animation.
    public void OnAttackAnimationFinished(EnemyFSM enemy)
    {
        enemy.chaseMemoryTimer = enemy.chaseMemoryTime;
        enemy.TransitionToState(enemy.idleState);
    }
    
}

