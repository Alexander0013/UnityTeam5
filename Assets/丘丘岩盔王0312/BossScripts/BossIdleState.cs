using UnityEngine;

public class BossIdleState : BossBaseState
{
    //This variable is used to record whether the Idle state has been entered for the first time.
    private bool firstTime = true;
    private bool isRoalingPlaying = false;
    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Enter Idle State");
        boss.animator.SetBool("isWalking", false);
        boss.animator.SetBool("isAttacking", false);

        //Enable Roaling animation when entering Idle for the first time
        if (firstTime)
        {
            Debug.Log("first time idele");
            boss.animator.SetTrigger("Roaling");
            isRoalingPlaying = true;
            firstTime = false;
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        Debug.Log("IdleUpdate");
        if (boss.isDead || boss.playerTarget == null) return;

        // 如果正在播放 Roaling 動畫，檢查動畫是否已經播放完畢
        if (isRoalingPlaying)
        {
            AnimatorStateInfo stateInfo = boss.animator.GetCurrentAnimatorStateInfo(0);
            // 假設動畫狀態名稱為 "Roaling"
            if (stateInfo.IsName("Roaling") && stateInfo.normalizedTime < 1.0f)
            {
                // 還沒播完，直接 return，不進行狀態切換
                return;
            }
            else
            {
                // Roaling 播放完畢，解除鎖定
                isRoalingPlaying = false;
            }
        }

        // 當 Roaling 完成後，再根據玩家距離決定下一步動作
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        if (distance <= boss.attackRadius)
        {
            Debug.Log("Idle狀態，玩家在攻擊範圍內，切換到攻擊狀態");
            boss.TransitionToState(boss.attackState);
        }
        else
        {
            Debug.Log("Idle狀態，玩家不在攻擊範圍內，切換到追逐狀態");
            boss.TransitionToState(boss.chaseState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        // Not needed here
        //Debug.Log("Exit boss idle State");
    }
}
