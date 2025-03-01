using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    // Combo counter: cycles 1 → 2 → 3 → 1 → …
    private int comboCount;
    // Global timer for the state (max duration = 30 sec)
    private float stateTimer = 0f;
    // Timer for detecting combo input within the current chain
    private float comboTimer = 0f;
    // How long the player has to press the attack key to continue the chain
    private const float comboInputWindow = 0.5f;
    // Normalized time threshold (from the current attack animation) at which we consider the current attack nearly finished
    private const float normalizedEndThreshold = 0.9f;
    // Maximum duration for AttackState
    private const float maxStateDuration = 30f;
    // Whether the current chain is active (i.e. accepting combo input)
    private bool chainActive = true;

    // Optional AttackData reference
    public AttackData attackData;

    public AttackState(int initialCombo = 1)
    {
        comboCount = initialCombo;
    }

    public override void EnterState(PlayerStateManager player)
    {
        stateTimer = 0f;
        comboTimer = 0f;
        chainActive = true;
        Debug.Log("Entering Attack State, starting combo: " + comboCount);

        // Smoothly blend in the Attack layer.
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeightTo(attackLayerIndex, 1f, 0.3f));

        // Switch weapons: hide idle weapon and show attack weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideIdleWeapon();
            player.HideidleWeaponfromAttack = true;
            weaponController.ShowAttackWeapon();
        }

        // Set animator parameters for the attack
        if (player.Animator != null)
        {
            player.Animator.SetInteger("ComboCount", comboCount);
            player.Animator.SetTrigger("AttackTrigger");
        }

        // If needed, pass attack data to the CombatController.
        CombatController combatController = player.GetComponent<CombatController>();
        if (combatController != null && attackData != null)
        {
            combatController.currentAttackData = attackData;
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        stateTimer += Time.deltaTime;

        // Interrupt attack state immediately if jump is pressed.
        if (player.Input.jump)
        {
            Debug.Log("Attack interrupted: Jump input.");
            player.Input.jump = false;
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");
            player.SwitchState(new JumpState());
            return;
        }

        // Interrupt attack state if there is movement input.
        if (player.Input.move != Vector2.zero)
        {
            Debug.Log("Attack interrupted: Movement input.");
            player.Input.attack = false;
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");
            if (player.Input.sprint)
                player.SwitchState(new RunState());
            else
                player.SwitchState(new WalkState());
            return;
        }

        // End the entire AttackState after 20 seconds.
        if (stateTimer >= maxStateDuration)
        {
            Debug.Log("Attack state duration exceeded. Ending attack chain.");
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");
            player.SwitchState(new IdleState());
            return;
        }

        // Get current attack animation normalized time.
        AnimatorStateInfo attackStateInfo = player.Animator.GetCurrentAnimatorStateInfo(player.Animator.GetLayerIndex("Attack Layer"));
        float normalizedTime = attackStateInfo.normalizedTime;

        if (chainActive)
        {
            // If attack input is detected, reset the combo timer and update the combo count.
            if (player.Input.attack)
            {
                player.Input.attack = false;
                comboTimer = 0f;
                // Increment combo count; cycle from 1 to 3 then back to 1.
                comboCount = (comboCount % 3) + 1;
                Debug.Log("Combo input received: new combo count = " + comboCount);
                if (player.Animator != null)
                {
                    player.Animator.SetInteger("ComboCount", comboCount);
                    //player.Animator.SetTrigger("AttackTrigger");
                }
            }
            else
            {
                // No new input this frame, so increment combo timer.
                comboTimer += Time.deltaTime;
                // If no input is received within the combo window OR if the current animation is nearly finished, end the current chain.
                if (comboTimer > comboInputWindow || normalizedTime >= normalizedEndThreshold)
                {
                    chainActive = false;
                    Debug.Log("Combo chain ended. Transitioning to sword idle in attack layer.");
                    if (player.Animator != null)
                        player.Animator.SetTrigger("AttackEnd");
                }
            }
        }
        else
        {
            // The chain is finished (we're in sword idle) and we're waiting for the next chain.
            if (player.Input.attack)
            {
                player.Input.attack = false;
                Debug.Log("Starting new attack chain.");
                // Reset combo count and chain timers.
                comboCount = 1;
                chainActive = true;
                comboTimer = 0f;
                if (player.Animator != null)
                {
                    player.Animator.SetInteger("ComboCount", comboCount);
                    player.Animator.SetTrigger("AttackTrigger");
                }
            }
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Clear attack input.
        player.Input.attack = false;
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeight(attackLayerIndex, 0.2f));

        // Hide the attack weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideAttackWeapon();
            Debug.Log("Attack weapon hidden.");
        }
    }
}
