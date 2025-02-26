using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    private int comboCount;
    private float timer = 0f;

    // Allowed window (in seconds) during which the next combo input is accepted.
    private const float comboWindowStart = 0.2f;
    private const float comboWindowEnd = 1.2f;
    private const int maxCombo = 3;

    // Array of durations (in seconds) for each attack animation.
    private float[] attackDurations = new float[4] { 1.2f, 1.2f, 1.2f, 1.2f };

    // Total duration is the sum of durations for the attacks in the combo.
    private float totalAttackDuration;

    // -----------------------
    // NEW: Reference to AttackData ScriptableObject.
    public AttackData attackData;

    // -----------------------

    public AttackState(int comboCount = 1)
    {
        this.comboCount = comboCount;
        totalAttackDuration = CalculateTotalDuration(comboCount);
    }

    // Helper: calculate the total duration for the current combo sequence.
    private float CalculateTotalDuration(int count)
    {
        float sum = 0f;
        for (int i = 0; i < count; i++)
        {
            sum += attackDurations[i];
        }
        return sum;
    }

    public override void EnterState(PlayerStateManager player)
    {
        // Reset the idle weapon timer upon entering attack.
        player.idleWeaponTimer = 0f;
        Debug.Log("Entering Attack State, combo count: " + comboCount);


        // Enable the Attack layer by smoothly blending its weight from 0 to 1.
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeightTo(attackLayerIndex, 1f, 0.3f)); // 0.3 seconds blend


        // Switch weapons: hide the idle weapon and show the attack weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideIdleWeapon();
            player.HideidleWeaponfromAttack = true;
            weaponController.ShowAttackWeapon();
            Debug.Log("Attack weapon displayed.");
        }

        if (player.Animator != null)
        {
            // Set animator parameter to indicate which attack animation to play.
            player.Animator.SetInteger("ComboCount", comboCount);
            // Trigger the attack sequence.
            player.Animator.SetTrigger("AttackTrigger");
        }


        // Assign the AttackData to the CombatController.
        CombatController combatController = player.GetComponent<CombatController>();
        if (combatController != null && attackData != null)
        {
            combatController.currentAttackData = attackData;
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Get the current attack state's normalized time.
        AnimatorStateInfo attackStateInfo = player.Animator.GetCurrentAnimatorStateInfo(player.Animator.GetLayerIndex("Attack Layer"));
        float normalizedTime = attackStateInfo.normalizedTime;
        timer += Time.deltaTime;
        // Within the allowed combo window, check if the player pressed attack to chain the combo.
        if (normalizedTime >= 0.3f && normalizedTime <= 0.7f && player.Input.attack)
        {
            player.Input.attack = false;
            if (comboCount < maxCombo)
            {
                comboCount++;
                player.Animator.SetInteger("ComboCount", comboCount);
                totalAttackDuration = CalculateTotalDuration(comboCount);
            }
        }

        // When the accumulated time exceeds the total duration for the combo, exit AttackState.
        if (timer >= totalAttackDuration)
        {
            Debug.Log("Attack sequence finished. Transitioning to Idle.");
            player.SwitchState(new IdleState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        // Clear the attack input and disable the Attack layer.
        player.Input.attack = false;
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");

        // Instead of setting weight to 0 instantly, start a coroutine to blend out.
        player.StartCoroutine(player.BlendAttackLayerWeight(attackLayerIndex, 0.2f));

        // Hide the attack weapon (IdleState will show the idle weapon).
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideAttackWeapon();
            Debug.Log("Attack weapon hidden");
        }
    }
}
