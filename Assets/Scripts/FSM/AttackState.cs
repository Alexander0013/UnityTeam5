using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    private bool comboQueued = false;
    private bool weaponAppeared = false;
    private float timer = 0f;
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Attack State");
        if (player.Animator != null){
            // Force the attack animation to start from the beginning.
            player.Animator.Play("Attack", 0, 0f);
            player.Animator.SetTrigger("AttackTrigger");
        }
            
        comboQueued = false;
        weaponAppeared = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        timer += Time.deltaTime;
        // Get current animation state info.
        var stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        // Check for the proper window to display the weapon
        if (!weaponAppeared && timer >= 0.1f)
        {
            Debug.Log("weapon appear");
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.ShowWeapon();
                weaponAppeared = true;
            }
        }
        // Check if we are in the combo window for chaining attacks
        if (timer >= 0.3f && timer <= 0.7f)
        {
            if (player.Input.attack)
            {
                comboQueued = true;
                player.SwitchState(new AttackState());
                Debug.Log("Combo queued");
            }
        }
        // When the animation finishes, switch state (weapon hide logic might be in IdleState)
        if ( timer >= 1f)
        {
            if (comboQueued)
            {
                Debug.Log("Combo attack triggered");
                player.SwitchState(new AttackState());
            }
            else
            {
                Debug.Log("Attack animation finished. Switching to Idle State.");
                player.SwitchState(new IdleState());
            }
        }
        
        
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Ensure the attack flag is cleared so subsequent clicks work.
        player.Input.attack = false;
    }
}

