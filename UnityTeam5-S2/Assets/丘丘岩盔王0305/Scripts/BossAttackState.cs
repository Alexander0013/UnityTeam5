using UnityEngine;

public class BossAttackState : BossBaseState
{
    private float attackTimer = 0f;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Attack (Swiping) 狀態");
        attackTimer = 0f;
        // 停止移動：將速度參數清零
        if (boss.animator != null)
        {
            boss.animator.SetFloat("Speed", 0f);
            boss.animator.SetTrigger("Swiping");
        }
        // 若使用 NavMeshAgent 或其他移動控制，則可以在此禁用它們
        // e.g., boss.navMeshAgent.enabled = false;
    }

    public override void UpdateState(BossFSM boss)
    {
        attackTimer += Time.deltaTime;

        // 在攻擊狀態中不更新位置，確保 Boss 保持原地不動

        // 檢查目前攻擊動畫的播放狀態
        int layerIndex = boss.animator.GetLayerIndex("moveLayer");
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(layerIndex);

        // 如果「Swiping」動畫還沒播完，則等待（即使玩家移動也不切換狀態）
        if (stateInfo.IsName("Swiping") && stateInfo.normalizedTime < 1f)
        {
            return;
        }

        // 攻擊動畫播完後，再檢查玩家與 Boss 之間的距離
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);
        if (distance > boss.attackRadius)
        {
            Debug.Log("攻擊動畫播放完畢且玩家離開攻擊範圍，轉換至 Chase 狀態");
            boss.TransitionToState(boss.chaseState);
        }
        else
        {
            Debug.Log("攻擊動畫播放完畢，玩家仍在攻擊範圍，重新攻擊");
            // 重置計時器，重新觸發攻擊動畫
            attackTimer = 0f;
            if (boss.animator != null)
            {
                boss.animator.SetTrigger("Swiping");
            }
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Attack (Swiping) 狀態");
        // 若有禁用其他移動控制元件，則可以在這裡重新啟用它們
        // e.g., boss.navMeshAgent.enabled = true;
    }
}
