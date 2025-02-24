using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    private float idleTimer = 0f;

    public override void EnterState(PlayerStateManager player)
    {
        idleTimer = 0f;
        Debug.Log("Entering Idle State");

        if (player.Animator != null)
        {
            // Reset movement and combo parameters.
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
            player.Animator.SetInteger("ComboCount", 0);
            int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
            player.Animator.SetLayerWeight(attackLayerIndex, 0f);
        }
        
        // Clear any pending attack input.
        player.Input.attack = false;

        // Ensure the idle weapon is active and the attack weapon is hidden.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.ShowIdleWeapon();
            Debug.Log("Idle weapon displayed.");
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        idleTimer += Time.deltaTime;
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if(idleTimer >= 2.0f)
        {
            weaponController.HideIdleWeapon();
            Debug.Log("Idle weapon hided.");
        }

        // Transition based on player input.
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
            Debug.Log("Attack triggered from IdleState.");
            player.SwitchState(new AttackState(1));
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Additional exit logic if needed.
    }
}
