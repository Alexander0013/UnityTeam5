using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
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

        if (player.firstEntry)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            // On the very first scene entry, hide both weapons.
            if (weaponController != null)
            {
                weaponController.HideBothWeapons();
                Debug.Log("Initial entry: Both weapons hidden."); 
                player.firstEntry = false;
            }

        }
        
        if (player.HideidleWeaponfromAttack)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            // after scene entry, show the idle weapon.
            if (weaponController != null)
            {
                player.idleWeaponHide = false;
                weaponController.ShowIdleWeapon();
                weaponController.HideAttackWeapon();   
            }
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Increment the shared timer.
        player.idleWeaponTimer += Time.deltaTime;
        if (!player.idleWeaponHide && player.idleWeaponTimer >= 5.0f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideIdleWeapon();
                player.idleWeaponHide = true;
                Debug.Log("Idle weapon hidden");
            }
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
