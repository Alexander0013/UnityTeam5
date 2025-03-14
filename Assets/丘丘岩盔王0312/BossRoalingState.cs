using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Roaling 狀態");

        if (boss.animator != null)
        {
            boss.animator.applyRootMotion = false; // 確保動畫不會影響移動
            boss.animator.SetTrigger("Roaling");  // 觸發 Roaling 動畫
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.animator == null) return;

        int layerIndex = boss.animator.GetLayerIndex("Base Layer"); // 或 "moveLayer"
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(layerIndex);

        // 等待 Roaling 播放完畢
        if (stateInfo.IsName("Roaling") && stateInfo.normalizedTime < 1f)
        {
            return;  // 動畫還沒播完，不切換狀態
        }

        // Roaling 播放完畢，轉換至 Chase
        Debug.Log("Roaling 動畫播放完畢，轉換至 Chase 狀態");
        boss.TransitionToState(boss.chaseState);
    }


    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Roaling 狀態");
        boss.animator.ResetTrigger("Roaling");  // 重置 Trigger，避免影響下一次播放
    }
}

