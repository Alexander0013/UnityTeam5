using UnityEngine;
using StarterAssets;

public class WalkState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        //Debug.Log("[WalkState] Enter");

        // Immediately hide the weapon when entering WalkState
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideWeapon();
        }

        if (player.Animator != null)
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0.5f);
    }

    public override void UpdateState(PlayerStateManager player)
    {

        // Transitions
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
        //else if (player.Input.attack)
        //{
        //    //Debug.Log("[WalkState] Attack triggered");
        //    player.SwitchState(new AttackState());
        //    player.Input.attack = false;
        //}
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
