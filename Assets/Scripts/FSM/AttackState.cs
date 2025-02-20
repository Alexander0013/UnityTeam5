using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    private bool comboQueued = false;
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Attack State");
        if (player.Animator != null){
            player.Animator.SetTrigger("AttackTrigger");
        }
            
        comboQueued = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Get current animation state info.
        var stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        
        if (stateInfo.normalizedTime >= 0.3f && stateInfo.normalizedTime <= 0.7f)
        {
            // Check if the player pressed attack during this window.
            if (player.Input.attack)
            {
                comboQueued = true;
                // Consume the input so it doesn't trigger repeatedly.
                //player.Input.attack = false;
                player.SwitchState(new AttackState());
                Debug.Log("Combo queued");
            }
        }
        /*
        
        // When the animation is about to end...
        if (stateInfo.normalizedTime >= 1f)
        {
            if (comboQueued)
            {
                Debug.Log("Combo attack triggered");
                // Re-enter the AttackState to chain the next attack.
                player.SwitchState(new AttackState());
            }
            else
            {
                Debug.Log("Attack animation finished. Switching to Idle State.");
                player.SwitchState(new IdleState());
            }
        }
        */
        
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Ensure the attack flag is cleared so subsequent clicks work.
        player.Input.attack = false;
    }
}

