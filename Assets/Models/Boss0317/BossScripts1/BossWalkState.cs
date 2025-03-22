using UnityEngine;
using System.Collections;

public class BossWalkState : BossBaseState
{
    private float moveSpeed = 2.0f; // 追逐速度

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("進到 WalkState");
        boss.animator.SetBool("Walk", true);
        boss.StartCoroutine(WalkDetectPlayer(boss));
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("玩家離開偵測範圍，Boss 回到 Idle");
            boss.TransitionToState(boss.idleState);
            return;
        }

        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // 如果玩家進入 swiping 範圍（≤1.5f），切換到 SwipingState
        if (distance <= 1.5f)
        {
            Debug.Log("玩家進入 swiping 攻擊範圍，Boss 進入 SwipingState");
            boss.TransitionToState(boss.swipingState);
            return;
        }

        // 如果 Charge 條件滿足（解鎖且冷卻結束，且距離在 2~6f 內），切換到 ChargeIdleState
        if (boss.chargeUnlockTimer == 0f && boss.chargeCooldownTimer <= 0f && distance >= 2f && distance <= 6f)
        {
            Debug.Log("玩家進入 charge 攻擊範圍，Boss 進入 ChargeIdleState");
            boss.TransitionToState(boss.chargeIdleState);
            return;
        }

        // 否則，繼續追逐玩家
        Vector3 targetPosition = boss.playerTarget.position;
        targetPosition.y = boss.transform.position.y;
        Vector3 direction = (targetPosition - boss.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            boss.transform.forward = Vector3.Lerp(boss.transform.forward, direction, Time.deltaTime * 5f);
        }
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private IEnumerator WalkDetectPlayer(BossFSM boss)
    {
        while (boss.currentState is BossWalkState)
        {
            boss.DetectPlayer();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Walk 狀態");
        boss.animator.SetBool("Walk", false);
    }
}