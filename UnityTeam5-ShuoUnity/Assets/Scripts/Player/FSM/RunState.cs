using UnityEngine;
using StarterAssets;

public class RunState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        //Debug.Log("[RunState] Enter");
        if (player.Animator != null)
            player.Animator.SetFloat("Speed", 1f);
        /// Immediately hide the weapon when entering
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideWeapon();
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Transitions
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
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        /// Immediately hide the weapon when exiting
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideWeapon();
        }
    }
}
