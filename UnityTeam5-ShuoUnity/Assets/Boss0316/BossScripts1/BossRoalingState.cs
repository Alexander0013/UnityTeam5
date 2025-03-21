using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("進入 Roaling 狀態");
        boss.animator.SetTrigger("Roaling");
        // 此狀態下不需額外 Update 行為，由動畫事件控制結束時機
    }

    public override void UpdateState(BossFSM boss)
    {
        // 等待動畫事件觸發 OnRoalingAnimationEnd()
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Roaling 狀態，開始 30 秒 Charge 計時");
        boss.StartCoroutine(boss.ChargeUnlockCountdown());
    }
}
