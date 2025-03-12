using UnityEngine;

public class BossIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        //Debug.Log("Enter Idle State");
        boss.animator.SetBool("isWalking", false);
        boss.animator.SetBool("isAttacking", false);

        // We pick a random idle time each time we enter Idle
        //idleTime = Random.Range(minIdle, maxIdle);
        //deciding = false;
    }

    public override void UpdateState(BossFSM boss)
    {
        Debug.Log("idelUpdate");
        if (boss.isDead || boss.playerTarget == null) return;

        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // Directly switch states according to distance：
        if (distance <= boss.attackRadius)
        {
            Debug.Log("idel狀態，玩家在怪物攻擊範圍內進行攻擊");
            // When the player is within the attack range, switch to attack state
            boss.TransitionToState(boss.attackState);
        }
        else
        {
            Debug.Log("idel狀態，怪物進到chase狀態");
            // When the player is out of attack range (assuming still within detection range), switch to chase mode
            boss.TransitionToState(boss.chaseState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        // Not needed here
        //Debug.Log("Exit boss idle State");
    }
}
