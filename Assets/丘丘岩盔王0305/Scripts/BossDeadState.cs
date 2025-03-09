using UnityEngine;

public class BossDeadState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log(boss.name + " is Dead");
        boss.animator.SetTrigger("Die");

        // 可選：停用 Collider 等元件，避免碰撞干擾
        Collider col = boss.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // 若需要，也可以在死亡動畫結束後刪除 Boss 物件
        Object.Destroy(boss.gameObject, 2f);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 死亡狀態下通常不需要 Update 邏輯
    }

    public override void ExitState(BossFSM boss)
    {
        // 死亡狀態一般不會退出，此處可留空
    }
}
