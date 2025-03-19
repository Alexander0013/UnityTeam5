using UnityEngine;

public class BossStandByState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 StandBy 狀態");
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // 若距離小於 2f，使用 Swiping
        if (distance <= 2f)
        {
            Debug.Log("距離小於1.5f，使用 Swiping");
            boss.TransitionToState(boss.swipingState);
        }
        // 若 Charge 已解鎖且冷卻完畢，並且玩家距離在 3f ~ 7f，則進行 Charge
        else if (boss.chargeUnlockTimer == 0f && boss.chargeCooldownTimer <= 0f && distance >= 3f && distance <= 7f)
        {
            Debug.Log("Charge 可用，進入 ChargeState");
            boss.TransitionToState(boss.chargeState);
        }
        else
        {
            Debug.Log("Charge 冷卻中或距離不符，返回 WalkState");
            boss.TransitionToState(boss.walkState);
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        // StandBy 狀態只在 Enter 時進行判斷，無需持續更新
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 StandBy 狀態");
    }
}

