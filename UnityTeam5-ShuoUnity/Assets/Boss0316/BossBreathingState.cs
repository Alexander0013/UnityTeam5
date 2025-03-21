using UnityEngine;
using System.Collections;

public class BossBreathingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Breath 狀態");
        // 播放 Breathing 動畫，請在 Animator 中建立對應 Trigger 或狀態
        boss.animator.SetTrigger("Breath");
        boss.animator.SetBool("Walk", false);

        // 等待一段時間後返回 StandByState
        boss.StartCoroutine(WaitAndTransition(boss));
    }

    private IEnumerator WaitAndTransition(BossFSM boss)
    {
        // 根據動畫長度調整等待時間
        yield return new WaitForSeconds(1f);
        boss.TransitionToState(boss.standByState);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 無需持續更新
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Breat 狀態");
    }
}

