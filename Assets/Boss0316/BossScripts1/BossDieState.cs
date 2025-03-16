using UnityEngine;

public class BossDieState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        boss.animator.SetTrigger("Die");

        // 禁用碰撞，防止玩家與屍體交互
        Collider col = boss.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 一段時間後摧毀 Boss
        Object.Destroy(boss.gameObject, 3f);
    }

    public override void UpdateState(BossFSM boss)
    {
        // 死亡狀態不需要更新
    }

    public override void ExitState(BossFSM boss)
    {
        // 死亡狀態不可退出，因此這裡不需要任何操作
    }
}
