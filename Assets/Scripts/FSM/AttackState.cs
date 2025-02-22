using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    // The current combo count (e.g., 1 for Attack1, 2 for Attack2, etc.)
    private int comboCount;
    private float timer = 0f;

    // Combo window (in seconds) during which additional input is accepted.
    private const float comboWindowStart = 0.2f;
    private const float comboWindowEnd = 0.5f;

    // Total duration of the attack animation (adjust to your clip's length).
    private float attackDuration = 1f;

    // Maximum combo count (3 in this example).
    private const int maxCombo = 3;

    // Constructor: default to comboCount 1 if not provided.
    public AttackState(int comboCount = 1)
    {
        this.comboCount = comboCount;
    }

    public override void EnterState(PlayerStateManager player)
    {
        timer = 0f;
        Debug.Log("Entering Attack State, combo count: " + comboCount);
        
        // Enable the Attack layer so its animations override the base layer.
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.Animator.SetLayerWeight(attackLayerIndex, 1f);

        if (player.Animator != null)
        {
            // Set the combo parameter so the Animator plays the correct attack animation.
            player.Animator.SetInteger("ComboCount", comboCount);
            // Fire the attack trigger.
            player.Animator.SetTrigger("AttackTrigger");
        }

        // Immediately show the weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.ShowWeapon();
            Debug.Log("Weapon shown in AttackState");
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        timer += Time.deltaTime;
        
        // During the combo window, if additional input is detected, increase the combo count.
        if (timer >= comboWindowStart && timer <= comboWindowEnd && player.Input.attack)
        {
            player.Input.attack = false; // Consume the input.
            if (comboCount < maxCombo)
            {
                comboCount++;
                Debug.Log("Combo input detected. New combo count: " + comboCount);
                // Optionally, you might want to immediately switch state here or simply let the Animator
                // handle the transition via its conditions.
                // For this example, we simply update the parameter:
                player.Animator.SetInteger("ComboCount", comboCount);
            }
            if (comboCount == 2 ){
                attackDuration += 1.0f;
            }
            if (comboCount == 3 ){
                attackDuration += 2.0f;
            }
        }
        // When the attack animation has finished, return to Idle.
        if (timer >= attackDuration)
        {
            Debug.Log("Attack animation finished. Returning to Idle.");
            player.SwitchState(new IdleState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Clear the attack input flag.
        player.Input.attack = false;
        // Disable the Attack layer so the base (Idle) animations resume.
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.Animator.SetLayerWeight(attackLayerIndex, 0f);
    }
}
