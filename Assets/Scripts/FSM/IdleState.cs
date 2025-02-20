using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Idle State");
    if (player.Animator != null)
        {
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
        }
    // Reset the attack flag so a new click will register.
    player.Input.attack = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.Input.move != Vector2.zero)
        {
            if (player.Input.sprint)
                player.SwitchState(new RunState());
            else
                player.SwitchState(new WalkState());
        }
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        else if (player.Input.attack)
        {
            Debug.Log("Switching to Attack State");
            player.SwitchState(new AttackState());
            // Consume the attack input so it can be re-triggered later.
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Cleanup if necessary.
    }
}
