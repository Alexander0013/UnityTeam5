using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        //Debug.Log("[IdleState] Entering Idle");

        // Reset movement/anim combos
        if (player.Animator != null)
        {
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
            player.Animator.SetInteger("ComboCount", 0);
            int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
            player.Animator.SetLayerWeight(attackLayerIndex, 0f);
        }
        player.Input.attack = false; // Clear any pending attack

        // Grab the weapon controller
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            // Always re-parent the weapon to idle attach
            weaponController.AttachWeaponToIdle();

            // Check the previous state to determine if we show or hide
            if (player.previousState is JumpState || player.previousState is AttackState)
            {
                // Coming from jump or attack => show the idle weapon
                weaponController.ShowWeapon();
                // Or if you want a fade: 
                // player.StartCoroutine(weaponController.FadeInWeapon(1f, 0.5f));
                //Debug.Log("[IdleState] Showing weapon (came from Jump/Attack).");
            }
            else if (player.previousState is RunState || player.previousState is WalkState)
            {
                // Coming from run or walk => hide the idle weapon
                weaponController.HideWeapon();
                //Debug.Log("[IdleState] Hiding weapon (came from Run/Walk).");
            }
            else
            {
                // If it's the first entry or some other state,
                weaponController.HideWeapon();
                //Debug.Log("[IdleState] Default: hiding weapon (unknown previous state).");
            }
        }

        // If you want to handle "firstEntry" logic or timers, you still can
        player.firstEntry = false;
        player.idleWeaponTimer = 0f;
        player.idleWeaponHide = true;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        WeaponController wc = player.GetComponent<WeaponController>(); // or GetComponentInChildren<WeaponController>()
        if (wc != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                wc.SwitchWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                wc.SwitchWeapon();
            }

        }
        
        // Then normal idle logic (movement transitions, jump, attack, etc.)
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
            //Debug.Log("[IdleState] Attack triggered, switching to AttackState.");
            player.SwitchState(new AttackState(1));
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Nothing special on exit, unless you want to track it
    }
}
