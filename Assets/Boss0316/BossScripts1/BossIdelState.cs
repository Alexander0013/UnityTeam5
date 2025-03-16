using UnityEngine;

public class BossIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Idle 狀態");
        if (boss.animator != null)
        {
            boss.animator.SetBool("isIdle", true);
            boss.animator.CrossFade("Idle", 0.1f);
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        // 不直接在這裡切換狀態，等動畫結束時觸發 FSM 的 OnIdleAnimationEnd
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Idle 狀態");
        boss.animator.SetBool("isIdle", false); // 只關閉 Idle，不切換狀態
    }

}


