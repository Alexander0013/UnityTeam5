using UnityEngine;
using StarterAssets;

public class JumpState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Jump State");
        if (player.Animator != null)
            player.Animator.SetBool(Animator.StringToHash("Jump"), true);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Transition back to Idle once grounded.
        if (player.IsGrounded())
        {
            player.SwitchState(new IdleState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        if (player.Animator != null)
            player.Animator.SetBool(Animator.StringToHash("Jump"), false);
    }
}
