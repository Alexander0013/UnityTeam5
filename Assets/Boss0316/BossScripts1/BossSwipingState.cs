using UnityEngine;

public class BossSwipingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Swiping 狀態");
        // 設置布林值為 true，觸發 Swiping 動畫
        boss.animator.SetBool("isSwiping", true);
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("玩家已離開偵測範圍，切換回 Walk 狀態");
            boss.TransitionToState(boss.walkState);
            return;
        }

        // 檢查 Boss 與 Player 的距離，若超過攻擊範圍則切換至 WalkState
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);
        if (distance > boss.attackRadius)
        {
            Debug.Log("Boss 與玩家距離過遠，切換回 Walk 狀態");
            boss.TransitionToState(boss.walkState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        // 結束 Swiping 動畫，將布林值設為 false
        boss.animator.SetBool("isSwiping", false);
    }
    public void OnAttackHit(BossFSM boss)
    {
        // Apply damage only if still in attack state
        boss.ApplyAttackDamage();
    }
    // Called by an Animation Event at the end of the attack animation.
    public void OnAttackAnimationFinished(BossFSM boss)
    {
        boss.TransitionToState(boss.walkState);
    }
}
