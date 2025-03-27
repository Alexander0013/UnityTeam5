using UnityEngine;

public class BossDanceState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("進入Dance 狀態");
        boss.animator.SetTrigger("Dance");
        // 此狀態下不需額外 Update 行為，由動畫事件控制結束時機
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget != null)
        {
            Debug.Log("玩家離開偵測範圍，Boss 回到 Idle");
            boss.TransitionToState(boss.walkState);

        }
    }

    public override void ExitState(BossFSM boss)
    {

    }
}

