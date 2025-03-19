using UnityEngine;

public class BossIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Idle 狀態");
        boss.animator.SetBool("isIdle", true);
        boss.animator.CrossFade("Idle", 0.1f);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 利用 OverlapSphere 檢測玩家
        Collider[] hits = Physics.OverlapSphere(boss.transform.position, boss.detectionRadius, boss.playerLayer);
        if (hits.Length > 0)
        {
            // 儲存玩家目標
            boss.playerTarget = hits[0].transform;
            Debug.Log("玩家進入偵測範圍，Boss 進入 Roaling 狀態");
            boss.TransitionToState(boss.roalingState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Idle 狀態");
        boss.animator.SetBool("isIdle", false);
    }
}
