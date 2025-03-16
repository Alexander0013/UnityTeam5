using UnityEngine;

public class BossWalkState : BossBaseState
{
    private float moveSpeed = 2.0f; // Boss 移動速度

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Walk 狀態");
        boss.animator.SetBool("Walk", true);  // 啟動 Walk 動畫
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("玩家已離開偵測範圍，回到 Idle");
            boss.TransitionToState(boss.idleState);
            return;
        }

        // 計算距離
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // **新增 JumpAttack 條件**
        if (distance > boss.attackRadius && distance < boss.detectionRadius && boss.IsJumpAttackReady())
        {
            Debug.Log("玩家在 JumpAttack 範圍內，進行跳躍攻擊");
            boss.TransitionToState(boss.jumpAttackState);
            return;
        }

        // 進入攻擊範圍時切換到戰鬥狀態
        if (distance <= boss.attackRadius)
        {
            Debug.Log("Boss 進入攻擊範圍，切換至 Combat 狀態");
            boss.TransitionToState(boss.combatState);
            return;
        }

        // 讓 Boss 面向玩家
        Vector3 targetPosition = boss.playerTarget.position;
        targetPosition.y = boss.transform.position.y; // 確保不會朝上或朝下
        Vector3 direction = (targetPosition - boss.transform.position).normalized;

        if (direction != Vector3.zero)
        {
            boss.transform.forward = Vector3.Lerp(boss.transform.forward, direction, Time.deltaTime * 5f);
        }

        // 移動 Boss
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Walk 狀態");
        boss.animator.SetBool("Walk", false);  // 停止 Walk 動畫

    }
}




