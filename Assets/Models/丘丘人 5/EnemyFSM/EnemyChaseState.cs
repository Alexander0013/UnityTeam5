using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private float speed = 2.5f;
    private float lostPlayerTimer;
    private float lostPlayerThreshold = 1.5f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter enemy Chase State");
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isAttacking", false);
        lostPlayerTimer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // If there is no player target, transition to return state.
        if (enemy.playerTarget == null)
        {
            enemy.TransitionToState(enemy.returnState);
            return;
        }
        // Check if blocked by another enemy before chasing
        if (IsBlockedByOtherEnemy(enemy))
        {
            enemy.TransitionToState(enemy.returnState);
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);

        // If within attack range, transition
        if (distance <= enemy.attackRadius)
        {
            enemy.TransitionToState(enemy.attackState);
            return;
        }

        // If within detection range, chase
        if (distance <= enemy.detectionRadius)
        {
            lostPlayerTimer = 0f;
            ChasePlayer(enemy);
        }
        else
        {
            // If the player is out of detection range, start a timer
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer > lostPlayerThreshold)
            {
                enemy.TransitionToState(enemy.returnState);
            }
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // When leaving chase, stop walking
        enemy.animator.SetBool("isWalking", false);
    }

    private void ChasePlayer(EnemyFSM enemy)
    {
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

        // Move toward the target position only in the xz plane
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }

    // Checks if another enemy is blocking the path toward the player.
    private bool IsBlockedByOtherEnemy(EnemyFSM enemy)
    {
        // If there is no player target, don't block.
        if (enemy.playerTarget == null)
            return false;
        Vector3 directionToPlayer = (enemy.playerTarget.position - enemy.transform.position).normalized;
        foreach (EnemyFSM other in EnemyFSM.AllEnemies)
        {
            if (other == enemy) continue;
            Vector3 toOther = other.transform.position - enemy.transform.position;
            // Check if the other enemy is nearly in the same direction and close enough (within 1 unit)
            if (Vector3.Angle(toOther, directionToPlayer) < 30f && toOther.magnitude < 1f)
            {
                return true;
            }
        }
        return false;
    }

}
