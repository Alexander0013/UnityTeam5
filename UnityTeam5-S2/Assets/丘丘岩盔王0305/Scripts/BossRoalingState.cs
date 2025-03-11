using System.Collections;
using UnityEngine;

public class BossRoalingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Roaling 狀態");
        // 啟動 Coroutine 來播放 Roaling 動畫
        boss.StartCoroutine(PerformRoaling(boss));
    }

    private IEnumerator PerformRoaling(BossFSM boss)
    {
        // 觸發 Roaling 動畫
        boss.animator.SetTrigger("Roaling");
        // 等待 4.2 秒 (根據動畫剪輯的長度)
        yield return new WaitForSeconds(4.2f);
        // 動畫播放完畢後切換到 Chase 狀態
        boss.TransitionToState(boss.chaseState);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 在這個狀態下不需要額外的 Update 邏輯，狀態轉換完全由 Coroutine 控制
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Roaling 狀態");
    }
}

