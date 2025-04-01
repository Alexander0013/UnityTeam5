using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTime = 0.5f;
    private bool deciding;
    private Vector3 targetPos;
    public Transform treasure;

    public override void EnterState(EnemyFSM enemy)
    {
        //Debug.Log("Enter Idle State");
        enemy.animator.SetBool("isWalking", false);
        // Get the player's position, but keep the enemy's y position
        //if (enemy.playerTarget != null)
        //{
        //    targetPos = enemy.playerTarget.position;
        //    targetPos.y = enemy.transform.position.y;
        //}

        // Calculate direction ignoring vertical difference
        //Vector3 direction = (targetPos - enemy.transform.position).normalized;
        //if (direction != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(direction);
        //    enemy.transform.rotation = Quaternion.Slerp(
        //        enemy.transform.rotation,
        //        targetRotation,
        //        Time.deltaTime * 5f
        //    );
        //}
        treasure = enemy.transform.parent;
        // We pick a random idle time each time we enter Idle
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        idleTime -= Time.deltaTime;
        if (idleTime > 0) return;
        if (enemy.isDead || enemy.playerTarget == null) return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        if (distance > enemy.detectionRadius)
        {
            enemy.TransitionToState(enemy.idleState);
        }
        if (enemy.IsPlayerInSight())
        {
            if (distance <= enemy.attackRadius)
            {
                enemy.TransitionToState(enemy.attackState);
            }
            else if (distance <= enemy.detectionRadius)
            {
                enemy.TransitionToState(enemy.chaseState);
            }

        }
        else if (enemy.chaseMemoryTimer > 0f && distance < enemy.detectionRadius)
        {
            enemy.TransitionToState(enemy.chaseState);
        }
        else
        {
            enemy.TransitionToState(enemy.idleState);
            enemy.chaseMemoryTimer = 0f;
        }

            
    }

    public override void ExitState(EnemyFSM enemy)
    {
        idleTime = 0.5f;
        //Debug.Log("Exit enemy idle State");
    }

}
