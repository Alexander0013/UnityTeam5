// RunState.cs
using UnityEngine;
using StarterAssets;

public class RunState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Run State");
        if (player.Animator != null)
            player.Animator.SetFloat("Speed", 1f);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.Input.move == Vector2.zero)
        {
            player.SwitchState(new IdleState());
        }
        else if (!player.Input.sprint)
        {
            player.SwitchState(new WalkState());
        }
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        else if (player.Input.attack)
        {
            player.SwitchState(new AttackState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Optional cleanup.
    }
}
