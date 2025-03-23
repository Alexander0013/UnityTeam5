using UnityEngine;

public class BossIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        boss.animator.SetBool("isIdle", true);
        boss.animator.CrossFade("Idle", 0.1f);
    }

    public override void UpdateState(BossFSM boss)
    {
        //if (boss.playerTarget == null) return;
        //float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        //boss.TransitionToState(boss.roalingState);
    }

    public override void ExitState(BossFSM boss)
    {
        boss.animator.SetBool("isIdle", false);
    }
}
