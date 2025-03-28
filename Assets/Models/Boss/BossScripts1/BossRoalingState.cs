using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BossRoalingState : BossBaseState
{

    public override void EnterState(BossFSM boss)
    {
        boss.PLayRoalingSound();
        Debug.Log("進入 Roaling 狀態");
        boss.animator.SetTrigger("Roaling");
        // 此狀態下不需額外 Update 行為，由動畫事件控制結束時機
    }


    public override void UpdateState(BossFSM boss)
    {
        // 等待動畫事件觸發 OnRoalingAnimationEnd()
    }

    public override void ExitState(BossFSM boss)
    {
       
    }
}
