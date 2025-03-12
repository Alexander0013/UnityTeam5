using UnityEngine;
using System.Collections;

public class EnemyAttackState : EnemyBaseState
{
    private bool isAttacking;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter Attack State");
        enemy.animator.SetBool("isWalking", false);
        isAttacking = false;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if(enemy.playerTarget == null) return;
        // Removed the distance check so the attack animation cannot be interrupted.
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
        // Trigger the attack animation
        enemy.animator.SetTrigger("Attack");
        
        // Wait for the remainder of the attack animation (total length 1.333 seconds)
        yield return new WaitForSeconds(1f);
        enemy.ApplyAttackDamage();

        // Transition to chase state once the attack animation is complete
        enemy.TransitionToState(enemy.idleState);
    }


    /// <summary>
    /// Finds any GameObject(s) tagged "Player" that is activeInHierarchy and has PlayerHealth > 0.
    /// Returns the first valid player's transform, or null if none found.
    /// </summary>
    private Transform FindActiveLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                PlayerHealth ph = p.GetComponent<PlayerHealth>();
                if (ph != null && ph.CurrentHealth > 0)
                {
                    return p.transform;
                }
            }
        }
        return null;
    }
}
