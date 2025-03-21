using UnityEngine;
using System.Collections;

public class BossChargeIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 ChargeIdle 狀態");
        // 播放 ChargeIdle 動畫，記得在 Animator 中建立對應的 Trigger 或狀態
        boss.animator.SetTrigger("ChargeIdle");

        // 等待短暫時間後切換到 ChargeState
        boss.StartCoroutine(WaitAndTransition(boss));
    }

    private IEnumerator WaitAndTransition(BossFSM boss)
    {
        // 這裡等待的時間可以依據動畫長度調整
        yield return new WaitForSeconds(0.5f);
        boss.TransitionToState(boss.chargeState);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 無需持續更新
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 ChargeIdle 狀態");
    }
}

