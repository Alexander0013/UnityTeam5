using UnityEngine;
using System.Collections;

public class EnemyGotHitState : EnemyBaseState
{
    private float gotHitDuration = 0.5f;  // How long the enemy stays “hit” before returning to Idle
    private float timer;

    public override void EnterState(EnemyFSM enemy)
    {
        //Debug.Log("Enter GotHit State");
        
        // Trigger the “GotHit” animation in Animator
        enemy.animator.SetTrigger("GotHit"); 
        
        // You might also want to stop movement or remove navmesh here:
        // e.g., enemyNavMeshAgent.isStopped = true; (if using NavMesh)

        timer = 0f;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // If you prefer the Animator transitions itself to Idle, 
        // you could rely purely on the Animator rather than time.
        // But here's a simple timer approach:

        timer += Time.deltaTime;
        if (timer >= gotHitDuration)
        {
            // Transition back to Idle (or Chase, if the player is near).
            enemy.TransitionToState(enemy.idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // If you paused movement, you can resume it here, or in IdleState’s EnterState
        // e.g. enemyNavMeshAgent.isStopped = false;
    }
}
