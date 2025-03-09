using UnityEngine;
using System.Collections;

public class EnemyAttackState : EnemyBaseState
{
    private bool isAttacking;
    private float attackCooldown = 1f; 
    private float cooldownTimer;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter Attack State");
        enemy.animator.SetBool("isAttacking", true);
        enemy.animator.SetBool("isWalking", false);
        isAttacking = false;
        cooldownTimer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {

        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);

        // If out of attack range, do something else
        if (distance > enemy.attackRadius)
        {
            enemy.TransitionToState(enemy.chaseState);
            return;
        }

        // Attack logic
        cooldownTimer += Time.deltaTime;
        if (!isAttacking && cooldownTimer >= attackCooldown)
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
        // Trigger animation
        enemy.animator.SetTrigger("Attack");

        // Delay to line up with the animation's impact frame
        yield return new WaitForSeconds(0.3f);  

        // OverlapSphere or direct check for your "old logic" of dealing damage
        float damage = enemy.npcData.baseDamage * enemy.npcData.comboMultiplier;
        float radius = enemy.npcData.hitRadius; // or enemy.attackRadius, whichever you used
        Vector3 attackCenter = enemy.transform.position + enemy.transform.forward * 1f; 
        // Adjust as needed

        // Example OverlapSphere
        Collider[] hits = Physics.OverlapSphere(attackCenter, radius, enemy.npcData.playerLayers);
        foreach (Collider c in hits)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }

        // Wait a bit, then transition (or do probability again)
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        // Could go directly back to idle or do random logic again
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
