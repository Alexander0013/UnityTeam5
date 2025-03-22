using UnityEngine;

public class BossChargeState : BossBaseState
{
    private float chargeSpeed = 12f;  // 衝撞速度
    private bool hasCharged = false;
    private Vector3 targetPosition;    // 目標位置：玩家後方

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Charge 狀態");
        boss.animator.SetTrigger("Charge");
        hasCharged = false;

        // 計算目標位置：玩家位置
        // 計算目標位置：玩家位置後方 1.5f
        if (boss.playerTarget != null)
        {
            targetPosition = boss.playerTarget.position - boss.playerTarget.forward * 1.5f;
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            boss.TransitionToState(boss.walkState);
            return;
        }
        // 當 Boss 與 targetPosition 的距離小於一定門檻時，認為 Charge 結束
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, chargeSpeed * Time.deltaTime);
        // 當 Boss 與 targetPosition 的距離小於一定門檻時，認為 Charge 結束
        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            OnChargeAnimationEnd(boss);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Charge 狀態");
    }
    // 當 Charge 結束後，強制清零速度並回到 WalkState
    public void OnChargeAnimationEnd(BossFSM boss)
    {
        Debug.Log("Charge 動畫結束");
        Rigidbody rb = boss.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
        boss.StartChargeCooldown();
        boss.TransitionToState(boss.walkState);
    }

    public void OnChargeCollision(BossFSM boss, Collider other)
    {
        if (other.CompareTag("Player") && !hasCharged)
        {
            IDamageable dmg = other.GetComponent<IDamageable>();
            if (dmg != null)
            {
                float damage = boss.bossnpcData.chargeDamage;
                dmg.TakeDamage(damage);
                hasCharged = true;
                Debug.Log("Charge 撞擊到玩家，造成傷害");
            }
        }
    }
}
