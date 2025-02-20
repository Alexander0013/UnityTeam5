using UnityEngine;
using StarterAssets;

public class JumpState : PlayerBaseState
{
    private float jumpTimer = 0f;
    private bool weaponHidden = false;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Jump State");
        jumpTimer = 0f;
        weaponHidden = false;
        WeaponController weaponController = player.GetComponent<WeaponController>();
        weaponController.HideWeapon();
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

