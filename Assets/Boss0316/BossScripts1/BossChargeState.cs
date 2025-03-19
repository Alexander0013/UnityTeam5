using UnityEngine;

public class BossChargeState : BossBaseState
{
    private float chargeSpeed = 12f;  // 衝撞速度
    private bool hasCharged = false;
    private Vector3 targetPosition;    // 目標位置：玩家後方1f

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Charge 狀態");
        boss.animator.SetTrigger("Charge");
        hasCharged = false;

        // 計算目標位置：玩家位置後方 1.5f（假設玩家的 forward 為其朝向）
        if (boss.playerTarget != null)
        {
            targetPosition = boss.playerTarget.position - boss.playerTarget.forward * 1.5f;

            // 若 Boss 附加有 MeshCollider，則將目標位置限制在該 MeshCollider 的 Bounds 內
            MeshCollider mc = boss.GetComponent<MeshCollider>();
            if (mc != null)
            {
                Bounds bounds = mc.bounds;
                targetPosition.x = Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, bounds.min.y, bounds.max.y);
                targetPosition.z = Mathf.Clamp(targetPosition.z, bounds.min.z, bounds.max.z);
            }
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            // 若玩家丟失，則返回 StandBy 狀態
            boss.TransitionToState(boss.standByState);
            return;
        }

        // 使用 MoveTowards 使 Boss 平滑移動到目標位置
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, chargeSpeed * Time.deltaTime);

        // 當 Boss 與目標位置的距離足夠接近（例如小於 0.1f）時，認為衝撞完成
        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            OnChargeAnimationEnd(boss);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Charge 狀態");
        // 如有需要，這裡可以停用 Charge 專用的 Collider
    }

    // 此方法由 Charge 動畫事件或 UpdateState 判斷到達目標後呼叫，表示 Charge 攻擊結束
    public void OnChargeAnimationEnd(BossFSM boss)
    {
        Debug.Log("Charge 動畫結束");
        boss.StartChargeCooldown(); // 啟動 15 秒冷卻
        boss.TransitionToState(boss.standByState);
    }

    // 此方法應由 Charge Collider 的 OnTriggerEnter 呼叫，當 Boss 與玩家碰撞時造成傷害
    public void OnChargeCollision(BossFSM boss, Collider other)
    {
        if (other.CompareTag("Player") && !hasCharged)
        {
            IDamageable dmg = other.GetComponent<IDamageable>();
            if (dmg != null)
            {
                float damage = boss.bossnpcData.chargeDamage; // 請確保 BossNPCStateData 中有 chargeDamage 欄位
                dmg.TakeDamage(damage);
                hasCharged = true;
                Debug.Log("Charge 撞擊到玩家，造成傷害");
            }
        }
    }
}



