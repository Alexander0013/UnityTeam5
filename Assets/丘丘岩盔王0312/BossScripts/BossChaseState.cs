using UnityEngine;

public class BossChaseState : BossBaseState
{
    private float speed = 2f;
    private float lostPlayerTimer;
    private float lostPlayerThreshold = 1.5f;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Enter boss Chase State");
        boss.animator.SetBool("isWalking", true);
        boss.animator.SetBool("isAttacking", false);

    }

    public override void UpdateState(BossFSM boss)
    {
        // If the player target does not exist, return directly
        if (boss.playerTarget == null) return;

        // Calculate the distance between the boss and the player
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // When the player is within the attack range, it will directly switch to the attack state
        if (distance <= boss.attackRadius)
        {
            boss.TransitionToState(boss.attackState);
            //return;
        }
        else
        {
            // Keep chasing players
            ChasePlayer(boss);
        }

    }

    public override void ExitState(BossFSM boss)
    {
        // When leaving chase, stop walking
        boss.animator.SetBool("isWalking", false);
    }

    private void ChasePlayer(BossFSM boss)
    {
        Vector3 direction = (boss.playerTarget.position - boss.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            boss.transform.rotation = Quaternion.Slerp(
                boss.transform.rotation,
                targetRotation, 
                Time.deltaTime * 5f
            );
        }

        boss.transform.position = Vector3.MoveTowards(
            boss.transform.position,
            boss.playerTarget.position,
            speed * Time.deltaTime
        );
    }
    
}
