using UnityEngine;
using System.Collections; // 引入協程的命名空間

public class BossDieState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        boss.animator.SetTrigger("Die");

        // 禁用碰撞，防止玩家與屍體交互
        Collider col = boss.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 開始等待死亡動畫播放完畢
        boss.StartCoroutine(WaitForDieAnimation(boss));
    }

    private IEnumerator WaitForDieAnimation(BossFSM boss)
    {
        // 取得 Die 動畫的長度
        AnimatorStateInfo animInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
        float dieAnimationLength = animInfo.length;

        // 等待動畫播放完畢
        yield return new WaitForSeconds(dieAnimationLength);

        // 銷毀 Boss 物件
        Object.Destroy(boss.gameObject);
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

