using UnityEngine;
using System.Collections;

public class BossSwipingState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Swiping 狀態");
        if (boss.playerTarget != null)
        {
            boss.StartCoroutine(SmoothRotateToTarget(boss, boss.playerTarget.position, 0.3f));
        }
        // 先清除或重置不需要的參數
        boss.animator.ResetTrigger("Walk");
        boss.animator.SetBool("Walk", false);
        boss.animator.SetBool("isSwiping", false);
        boss.animator.ResetTrigger("Dance");
        // 平滑過渡到 Swiping 動畫
        boss.animator.CrossFade("Swiping", 0.1f, 0, 0f);

        // 設置 swiping 參數（確保在過渡完成後）
        boss.animator.SetBool("isSwiping", true);
    }
    private IEnumerator SmoothRotateToTarget(BossFSM boss, Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Quaternion startRot = boss.transform.rotation;
        // 僅在水平面旋轉
        Vector3 direction = (targetPos - boss.transform.position).normalized;
        direction.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        while (elapsed < duration)
        {
            boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.transform.rotation = targetRot;
    }
    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("玩家離開偵測範圍，切換回 Walk 狀態");
            boss.TransitionToState(boss.walkState);
            
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Swiping 狀態");
        boss.animator.SetBool("isSwiping", false);
    }

    // 由動畫事件呼叫，檢查是否命中玩家
    public void OnAttackHit(BossFSM boss)
    {
        boss.ApplyAttackDamage();
  
    }

    // 攻擊動畫結束後，回到 StandBy 狀態讓 Boss 決定下一步動作
    public void OnAttackAnimationFinished(BossFSM boss)
    {
        boss.StartCoroutine(DelayedSwipingTransition(boss));
    }
    private IEnumerator DelayedSwipingTransition(BossFSM boss)
    {
        // 延遲0.2秒，確保動畫完全結束且 Animator 參數穩定
        yield return new WaitForSeconds(0.2f);
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);
        if (distance <= boss.attackRadius)
        {
            Debug.Log("玩家仍在 swiping 範圍，持續 swiping");
            boss.TransitionToState(boss.swipingState);
        }
        else
        {
            Debug.Log("玩家已離開 swiping 範圍，返回 WalkState");
            boss.TransitionToState(boss.walkState);
        }
    }
  
}