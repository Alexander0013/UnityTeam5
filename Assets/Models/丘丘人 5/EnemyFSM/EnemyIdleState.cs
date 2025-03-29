using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTime;
    private bool deciding;
    private float minIdle = 0.5f;  // random idle range
    private float maxIdle = 1f;
    private Vector3 targetPos;
    public Transform treasure;

    public override void EnterState(EnemyFSM enemy)
    {
        //Debug.Log("Enter Idle State");
        enemy.animator.SetBool("isWalking", false);
        // Get the player's position, but keep the enemy's y position
        if (enemy.playerTarget != null)
        {
            targetPos = enemy.playerTarget.position;
            targetPos.y = enemy.transform.position.y;
        }

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
        treasure = enemy.transform.parent;
        // We pick a random idle time each time we enter Idle
        idleTime = Random.Range(minIdle, maxIdle);
        deciding = false;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.isDead) return;

        if (enemy.playerTarget == null) return;
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);

        // Decrement the idleTime while we're out of detection range
        // or we haven't chosen a new action yet
        idleTime -= Time.deltaTime;
        if (enemy.IsPlayerInSight())
        {
            enemy.TransitionToState(enemy.chaseState);
            return;
        }
        if (enemy.chaseMemoryTimer > 0f)
        {
            enemy.TransitionToState(enemy.chaseState);
            return;
        }

        // If the time is up, we do the next decision
        if (!deciding && idleTime <= 0f)
        {
            deciding = true; // so we only do one decision

            if (enemy.IsPlayerInSight())
            {
                if (distance > enemy.attackRadius)
                {
                    DecideIdleOrChase(enemy);
                    return;
                }
                else
                {
                    DecideIdleChaseOrAttack(enemy);
                    return;
                }
            }
            else
            {
                idleTime = Random.Range(minIdle, maxIdle);
                deciding = false;
            }
            }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Not needed here
        //Debug.Log("Exit enemy idle State");
    }

    private void DecideIdleOrChase(EnemyFSM enemy)
    {
        // Weighted random: e.g., 30% remain idle, 70% chase
        float rand = Random.value; // 0..1
        if (rand < 0.3f)
        {
            // remain idle: pick a new idleTime
            idleTime = Random.Range(minIdle, maxIdle);
            deciding = false;
            //Debug.Log("Decided: remain idle");
        }
        else
        {
            // transition to chase immediately
            //Debug.Log("Decided: chase");
            enemy.TransitionToState(enemy.chaseState);
            // no further code needed, the chase state sets isWalking = true
        }
    }

    private void DecideIdleChaseOrAttack(EnemyFSM enemy)
    {
        // Weighted random: 10% idle, 20% chase, 70% attack
        float rand = Random.value; // 0..1
        if (rand < 0.1f)
        {
            // remain idle
            idleTime = Random.Range(minIdle, maxIdle);
            deciding = false;
            //Debug.Log("Decided: remain idle (within attackRadius)");
        }
        else if (rand < 0.2f)
        {
            //Debug.Log("Decided: chase (within attackRadius)");
            enemy.TransitionToState(enemy.chaseState);
        }
        else
        {
            //Debug.Log("Decided: attack (within attackRadius)");
            enemy.TransitionToState(enemy.attackState);
        }
    }
}
