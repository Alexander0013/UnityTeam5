using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("進到RoalingState");
        if (boss.animator != null)
        {
            boss.animator.SetTrigger("Roaling");
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        // 不在這裡切換狀態，完全依靠 AnimationEvent
    }

    public override void ExitState(BossFSM boss)
    {
        boss.animator.ResetTrigger("Roaling");
    }
}

