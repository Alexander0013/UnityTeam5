using UnityEngine;
using StarterAssets;

public class WalkState : PlayerBaseState
{

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Walk State");
        WeaponController weaponController = player.GetComponent<WeaponController>();
        weaponController.HideIdleWeapon();
        if (player.Animator != null)
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0.5f);
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
