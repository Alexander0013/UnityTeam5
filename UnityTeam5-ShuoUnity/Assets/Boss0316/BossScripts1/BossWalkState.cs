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

        // 若玩家距離小於 1.5f，進入 StandByState 來決定攻擊方式
        if (distance <= 1.5f)
        {
            Debug.Log("玩家進入 swiping 攻擊範圍，Boss 進入 StandBy 狀態");
            boss.TransitionToState(boss.standByState);
            return;
        }

        // 若 Charge 已解鎖 & 冷卻完成，並且玩家距離在 2f ~ 6f 內，進入 StandByState 來決定是否 Charge
        if (boss.chargeUnlockTimer == 0f && boss.chargeCooldownTimer <= 0f && distance >= 2f && distance <= 6f)
        {
            Debug.Log("玩家進入 charge 攻擊範圍，Boss 進入 StandBy 狀態");
            boss.TransitionToState(boss.standByState);
            return;
        }

        // 追逐玩家
        Vector3 targetPosition = boss.playerTarget.position;
        targetPosition.y = boss.transform.position.y; // 保持水平移動
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
