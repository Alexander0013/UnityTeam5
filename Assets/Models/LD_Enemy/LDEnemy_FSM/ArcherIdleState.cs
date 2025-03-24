using UnityEngine;

public class ArcherIdleState : EnemyBaseState
{
    private float guardRange = 10f;
    private float minIdle = 2f;  // random idle range
    private float maxIdle = 4f;
    private float idleTime;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherIdleState] Entering Idle");
        enemy.animator.SetBool("isAttacking", false);
        idleTime = Random.Range(minIdle, maxIdle);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;
        idleTime -= Time.deltaTime;
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        if (idleTime <= 0f)
        {
            // Use npcData.hitRadius for close-range melee.
            if (distance <= enemy.npcData.hitRadius)
            {
                enemy.TransitionToState(((ArcherFSM)enemy).meleeState);
            }
            // If within guard range but not too close, shoot.
            else if (distance <= guardRange)
            {
                enemy.TransitionToState(((ArcherFSM)enemy).shootState);
            }
            else
            {
                idleTime = Random.Range(minIdle, maxIdle);
            }
        }
        // Otherwise, remain idle.
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Nothing to clean up.
    }
}
