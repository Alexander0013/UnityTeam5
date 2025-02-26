using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    private int comboCount;
    private float timer = 0f;
    private const float comboWindowStart = 0.2f;
    private const float comboWindowEnd = 1.2f;
    private const int maxCombo = 3;
    private float[] attackDurations = new float[4] { 1.2f, 1.2f, 1.2f, 1.2f };
    private float totalAttackDuration;

    public AttackData attackData;

    public AttackState(int comboCount = 1)
    {
        this.comboCount = comboCount;
        totalAttackDuration = CalculateTotalDuration(comboCount);
    }

    private float CalculateTotalDuration(int count)
    {
        float sum = 0f;
        for (int i = 0; i < count; i++)
            sum += attackDurations[i];
        return sum;
    }

    public override void EnterState(PlayerStateManager player)
    {
        timer = 0f;
        Debug.Log("Entering Attack State, combo count: " + comboCount);

        // Smoothly blend in the Attack layer.
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeightTo(attackLayerIndex, 1f, 0.3f));

        // Switch weapons: Hide idle weapon (with dissolve fade-out) and show attack weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            // Instead of instantly hiding idle weapon, you may choose to cancel any fade-in.
            // Here we simply hide it.
            weaponController.HideIdleWeapon();
            player.HideidleWeaponfromAttack = true;
            weaponController.ShowAttackWeapon();
        }

        if (player.Animator != null)
        {
            player.Animator.SetInteger("ComboCount", comboCount);
            player.Animator.SetTrigger("AttackTrigger");
        }

        // Pass attack data if needed.
        CombatController combatController = player.GetComponent<CombatController>();
        if (combatController != null && attackData != null)
            combatController.currentAttackData = attackData;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        AnimatorStateInfo attackStateInfo = player.Animator.GetCurrentAnimatorStateInfo(player.Animator.GetLayerIndex("Attack Layer"));
        float normalizedTime = attackStateInfo.normalizedTime;
        timer += Time.deltaTime;

        // Allow combo chaining if within allowed normalized time.
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

        // When the attack sequence is over, smoothly transition back to idle.
        if (timer >= totalAttackDuration)
        {
            Debug.Log("Attack sequence finished. Transitioning to Idle.");
            player.SwitchState(new IdleState());
        }
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.Input.attack = false;
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        // Smoothly blend out the Attack layer.
        player.StartCoroutine(player.BlendAttackLayerWeight(attackLayerIndex, 0.2f));
        // Hide the attack weapon.
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideAttackWeapon();
            Debug.Log("Attack weapon hidden");
        }
    }
}
