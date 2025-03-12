using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Roaling 狀態");

        if (boss.animator != null)
        {
            boss.animator.applyRootMotion = false; // 禁止動畫期間的 Root Motion 位移
            boss.animator.SetTrigger("Roaling");  // 改用 Trigger 觸發動畫
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.animator == null) return;

        int layerIndex = boss.animator.GetLayerIndex("moveLayer");
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(layerIndex);

        // 檢查 Roaling 是否還在播放
        if (stateInfo.IsName("Roaling") && stateInfo.normalizedTime < 1f)
        {
            return;  // 動畫未播完，等待
        }

        // 動畫播放完畢後，切換到 Idle 狀態
        Debug.Log("Roaling 動畫播放完畢，轉換至 Idle 狀態");
        boss.TransitionToState(boss.idleState);
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Roaling 狀態");
        boss.animator.ResetTrigger("Roaling");  // 確保不會影響下一次播放
    }
}
