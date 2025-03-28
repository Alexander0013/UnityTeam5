using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHitState : PlayerBaseState
{
    private float hitDuration = 0.6f;
    private float timer = 0f;

    public override void EnterState(PlayerStateManager player)
    {
        timer = 0f;
        if (player.Animator != null)
        {
            player.Animator.SetBool("getHit", true);
        }

        // 可以加上受擊時強制停止角色移動速度
        if (player.Controller != null)
        {
            player.Controller.Move(Vector3.zero);
            Debug.Log("stop move in gethit");
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        timer += Time.deltaTime;

        if (timer >= hitDuration)
        {
            if (player.Animator != null)
            {
                player.Animator.SetBool("getHit", false);
            }

            // 回到 Idle 狀態（或根據輸入切換其他狀態）
            player.SwitchState(new IdleState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (player.Animator != null)
        {
            player.Animator.SetBool("getHit", false);
        }
    }
}