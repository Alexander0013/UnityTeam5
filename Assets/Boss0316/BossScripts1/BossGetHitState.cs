using UnityEngine;

public class BossGetHitState : BossBaseState
{
    private float gotHitDuration = 0.8f; // 受擊動畫時間
    private float timer;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入受擊狀態");
        boss.animator.SetTrigger("getHit");
        timer = 0f;
    }

    public override void UpdateState(BossFSM boss)
    {
        //timer += Time.deltaTime;
        //if (timer >= gotHitDuration)
        //{
        //    // 根據當前情境決定要回到哪個狀態
        //    if (boss.playerTarget != null)
        //    {
        //        boss.TransitionToState(boss.combatState);
        //    }
        //    else
        //    {
        //        boss.TransitionToState(boss.idleState);
        //    }
        //}
    }

    public override void ExitState(BossFSM boss)
    {
        // 可選：如果有需要重置動畫或變數，這裡可以處理
    }
}
