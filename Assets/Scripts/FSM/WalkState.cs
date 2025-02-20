using UnityEngine;
using StarterAssets;

public class WalkState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Walk State");
        if (player.Animator != null)
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0.5f);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.Input.move == Vector2.zero)
        {
            player.SwitchState(new IdleState());
        }
        else if (player.Input.sprint)
        {
            player.SwitchState(new RunState());
        }
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        else if (player.Input.attack)
        {
            Debug.Log("Switching to Attack State from Walk");
            player.SwitchState(new AttackState());
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Optional cleanup.
    }
}
