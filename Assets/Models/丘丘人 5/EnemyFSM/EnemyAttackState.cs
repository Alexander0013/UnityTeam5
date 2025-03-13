using UnityEngine;
public class EnemyAttackState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter EnemyAttack State");
        enemy.animator.SetBool("isWalking", false);
        // Start the attack animation.
        enemy.animator.SetTrigger("Attack");
    }
    public override void UpdateState(EnemyFSM enemy)
    {
        // No polling needed here¡Xattack timing is controlled by animation events.
    }
    public override void ExitState(EnemyFSM enemy)
    {
        // Any cleanup if needed.
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

        Debug.Log("Leaving EnemyAttack State via animation event");
        enemy.TransitionToState(enemy.idleState);
    }
    
}

