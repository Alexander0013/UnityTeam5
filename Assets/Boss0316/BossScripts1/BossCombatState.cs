using UnityEngine;

public class BossCombatState : BossBaseState
{
    private float outOfSwipingTimer = 0f;
    private const float outOfSwipingThreshold = 5f;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Combat 狀態");
        outOfSwipingTimer = 0f;
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            Debug.Log("玩家目標失效，回到 Idle");
            boss.TransitionToState(boss.idleState);
            return;
        }

        float distanceToPlayer = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        if (distanceToPlayer <= boss.swipingRange)
        {
            outOfSwipingTimer = 0f;
            if (!(boss.currentState is BossSwipingState))
            {
                Debug.Log("玩家在 Swiping 範圍，切換到 Swiping 攻擊");
                boss.TransitionToState(boss.swipingState);
            }
        }
        else if (distanceToPlayer > boss.attackRadius && distanceToPlayer <= boss.detectionRadius)
        {
            outOfSwipingTimer += Time.deltaTime;

            if (outOfSwipingTimer >= outOfSwipingThreshold)
            {
                if (boss.IsJumpAttackReady())
                {
                    Debug.Log("玩家距離超過 5 秒，執行 JumpAttack");
                    boss.TransitionToState(boss.jumpAttackState);
                    boss.ResetJumpAttackCooldown();
                }
                else
                {
                    Debug.Log("JumpAttack 冷卻中，轉為 Walk");
                    boss.TransitionToState(boss.walkState);
                }
            }
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("離開 Combat 狀態");
    }
}




