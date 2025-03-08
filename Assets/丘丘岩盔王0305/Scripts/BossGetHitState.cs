using UnityEngine;
using System.Collections;

public class BossGotHitState : BossBaseState
{
    // 受傷狀態持續時間，可依需求調整
    private float gotHitDuration = 0.5f;
    private float timer;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 GotHit 狀態");
        boss.animator.SetTrigger("GotHit");
        // 如有需要，可以在此暫停移動或其他效果
        timer = 0f;
    }

    public override void UpdateState(BossFSM boss)
    {
        timer += Time.deltaTime;
        if (timer >= gotHitDuration)
        {
            // 根據需求，受傷後可以回 Idle 或 Chase 狀態
            boss.TransitionToState(boss.idleState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        // 如有需要，可在此恢復被暫停的動作等
    }
}
