using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float idleTime;
    private bool deciding;
    private float minIdle = 0.5f;  // random idle range
    private float maxIdle = 1f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter Idle State");
        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isAttacking", false);

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

        // If the time is up, we do the next decision
        if (!deciding && idleTime <= 0f)
        {
            deciding = true; // so we only do one decision

            // CASE 1: If distance > attackRadius && distance < detectionRadius
            // we do a probability: remain idle or chase
            if (distance > enemy.attackRadius && distance < enemy.detectionRadius)
            {
                DecideIdleOrChase(enemy);
                return;
            }
            // CASE 2: If distance <= attackRadius
            // we do a probability: idle, chase, or attack
            else if (distance <= enemy.attackRadius)
            {
                DecideIdleChaseOrAttack(enemy);
                return;
            }
            else
            {
                // If we're still outside detection range (distance >= detectionRadius),
                // just reset idleTime for another cycle
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
