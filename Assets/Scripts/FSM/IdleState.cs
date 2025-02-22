using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    private float idleTimer = 0f;
    private bool weaponHidden = false;

    public override void EnterState(PlayerStateManager player)
    {
        idleTimer = 0f;
        weaponHidden = false;
        Debug.Log("Entering Idle State");
        if (player.Animator != null)
        {
            // Set movement parameters to zero.
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
            // Reset the combo parameter.
            player.Animator.SetInteger("ComboCount", 0);
            // Ensure the Attack layer is disabled.
            int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
            player.Animator.SetLayerWeight(attackLayerIndex, 0f);
        }
        // Clear any pending attack input.
        player.Input.attack = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        idleTimer += Time.deltaTime;

        // After a short idle period, hide the weapon.
        if (!weaponHidden && idleTimer >= 2.0f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideWeapon();
                Debug.Log("Weapon hidden after idle period.");
                weaponHidden = true;
            }
        }

        // Transition based on input.
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
        // Optionally, when leaving Idle, you can show the weapon immediately.
    }
}
