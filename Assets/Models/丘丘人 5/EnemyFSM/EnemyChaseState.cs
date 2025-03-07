using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private float speed = 2f;
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
        if (enemy.playerTarget == null || IsPlayerDead())
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
        Vector3 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation, 
                Time.deltaTime * 5f
            );
        }

        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            enemy.playerTarget.position,
            speed * Time.deltaTime
        );
    }

    private bool IsPlayerDead()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return true;
        PlayerHealth ph = playerObj.GetComponent<PlayerHealth>();
        return (ph != null && ph.CurrentHealth <= 0);
    }
}
