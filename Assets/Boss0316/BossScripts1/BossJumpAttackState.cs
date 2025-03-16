using UnityEngine;

public class BossJumpAttackState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 執行 JumpAttack");
        boss.animator.SetTrigger("JumpAttack");
        boss.ResetJumpAttackCooldown(); // 設定冷卻時間
    }

    public override void UpdateState(BossFSM boss)
    {
        AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpAttack") && stateInfo.normalizedTime >= 1f)
        {
            Debug.Log("JumpAttack 動畫結束，回到 WalkState");
            boss.TransitionToState(boss.walkState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("離開 JumpAttack 狀態");
    }
    public void OnJumpHit(BossFSM boss)
    {
        // Apply damage only if still in attack state
        boss.ApplyJumpDamage();
    }
    // Called by an Animation Event at the end of the attack animation.
    public void OnJumpAnimationFinished(BossFSM boss)
    {

        Debug.Log("Leaving EnemyAttack State via animation event");
        boss.TransitionToState(boss.walkState);
    }
}   




