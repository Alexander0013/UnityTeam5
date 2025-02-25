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
        player.idleWeaponTimer += Time.deltaTime;
        if (!player.idleWeaponHide && player.idleWeaponTimer >= 5.0f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideIdleWeapon();
                player.idleWeaponTimer = 0f;
                player.idleWeaponHide = true;
                Debug.Log("Idle weapon hidden");
            }
        }
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
