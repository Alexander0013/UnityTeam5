using UnityEngine;
using System.Collections;

public class BossChargeIdleState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 ChargeIdle 狀態");
        // 先平滑轉向玩家
        if (boss.playerTarget != null)
        {
            boss.StartCoroutine(SmoothLookAt(boss, boss.playerTarget.position, 0.3f));
        }
        // 播放 ChargeIdle 動畫，請在 Animator 中設定對應 Trigger
        boss.animator.SetTrigger("ChargeIdle");
    }



    public override void UpdateState(BossFSM boss)
    {
        // 無需持續更新
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 ChargeIdle 狀態");
    }
  
    public void OnChargeIdleAnimationEnd(BossFSM boss)
    {
        Debug.Log("ChargeIdle 動畫結束，進入 ChargeState");
        boss.TransitionToState(boss.chargeState);
    }
    private IEnumerator SmoothLookAt(BossFSM boss, Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Quaternion startRot = boss.transform.rotation;
        Vector3 direction = (targetPos - boss.transform.position).normalized;
        direction.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        while (elapsed < duration)
        {
            boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.transform.rotation = targetRot;
    }
}

