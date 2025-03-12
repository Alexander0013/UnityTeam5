using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Roaling 狀態");
        if (boss.animator != null)
        {
            int layerIndex = boss.animator.GetLayerIndex("moveLayer");
            boss.animator.Play("Roaling", layerIndex);
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        int layerIndex = boss.animator.GetLayerIndex("moveLayer");
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(layerIndex);
        // 如果動畫狀態是 "Roaling" 且尚未播放完畢則等待
        if (stateInfo.IsName("Roaling") && stateInfo.normalizedTime < 1f)
        {
            return;
        }
        // 播放完畢後，若有玩家則切換到 Chase；否則回 Idle（或其他處理）
        if (boss.playerTarget != null)
        {
            Debug.Log("Roaling 動畫播放完畢，轉換至 Chase 狀態");
            boss.TransitionToState(boss.chaseState);
        }
        else
        {
            boss.TransitionToState(boss.idleState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Roaling 狀態");
    }
}
