using UnityEngine;
using StarterAssets;

public class IdleState : PlayerBaseState
{
    private float idleTimer = 0f;
    private bool weaponHidden = false;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Idle State");
        idleTimer = 0f;       // Reset the idle timer when entering the state.
        weaponHidden = false; // Reset the weapon hide flag.

        if (player.Animator != null)
        {
            player.Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
        }
        // Reset the attack input flag.
        player.Input.attack = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Increment the timer.
        idleTimer += Time.deltaTime;

        // After 3 seconds of idle, hide the weapon if it hasn't been hidden yet.
        if (!weaponHidden && idleTimer >= 2.0f)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.HideWeapon();
                Debug.Log("Weapon hidden after 3 seconds idle.");
                weaponHidden = true;
            }
        }
        // Only allow weapon switching if the weapon is not hidden.
        if (!weaponHidden && Input.GetKeyDown(KeyCode.Tab))
        {
            WeaponSwitcher weaponSwitcher = player.GetComponent<WeaponSwitcher>();
            if (weaponSwitcher != null)
            {
                weaponSwitcher.SwitchWeapon();
                Debug.Log("Weapon switched using Tab key.");
            }
        }

        // Check for movement input to transition to Walk or Run.
        if (player.Input.move != Vector2.zero)
        {
            if (player.Input.sprint)
                player.SwitchState(new RunState());
            else
                player.SwitchState(new WalkState());
        }
        // Check for jump input.
        else if (player.Input.jump)
        {
            player.SwitchState(new JumpState());
        }
        // Check for attack input.
        else if (player.Input.attack)
        {
            Debug.Log("Switching to Attack State");
            player.SwitchState(new AttackState());
            player.Input.attack = false;
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Optionally, you might want to show the weapon again when exiting idle.
    }
}
