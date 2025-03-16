using UnityEngine;
using StarterAssets;

public class AttackState : PlayerBaseState
{
    private int comboCount;
    private float stateTimer = 0f;
    private float comboTimer = 0f;
    private const float comboInputWindow = 0.5f;
    private const float normalizedEndThreshold = 1.6f;
    private const float maxStateDuration = 30f;
    private bool chainActive = true;
    private const float dashStep = 1f;

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
        //Debug.Log("[AttackState] Entering Attack. Combo start: " + comboCount);

        // Blend in Attack layer
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeightTo(attackLayerIndex, 1f, 0.3f));

        // Re-parent weapon to attack
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.AttachWeaponToAttack();
            weaponController.ShowWeapon();
        }

        // Auto-target the nearest enemy in front
        CombatController combat = player.GetComponent<CombatController>();
        if (combat != null)
        {
            combat.TryAutoTarget();
            combat.DashToTarget(dashStep);
        }

        // Set animator params
        if (player.Animator != null)
        {
            player.Animator.SetInteger("ComboCount", comboCount);
            player.Animator.SetTrigger("AttackTrigger");
        }

        // Pass attack data
        var combatController = player.GetComponent<CombatController>();
        if (combatController != null && attackData != null)
        {
            combatController.playerAttackData = attackData;
        }
        player.Animator.applyRootMotion = false;
        //dash to target

    }

    public override void UpdateState(PlayerStateManager player)
    {
        stateTimer += Time.deltaTime;

        // Interrupt if jump
        if (player.Input.jump)
        {
            //Debug.Log("[AttackState] Interrupted by jump");
            player.Input.jump = false;
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");
            player.SwitchState(new JumpState());
            return;
        }

        // Interrupt if movement
        if (player.Input.move != Vector2.zero)
        {
            //Debug.Log("[AttackState] Interrupted by movement");
            player.Input.attack = false;
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");

            if (player.Input.sprint)
                player.SwitchState(new RunState());
            else
                player.SwitchState(new WalkState());
            return;
        }

        // End after maxStateDuration
        if (stateTimer >= maxStateDuration)
        {
            //Debug.Log("[AttackState] Duration exceeded, forced end.");
            if (player.Animator != null)
                player.Animator.SetTrigger("AttackEnd");
            player.SwitchState(new IdleState());
            return;
        }

        // Check current animation normalized time
        AnimatorStateInfo attackStateInfo = player.Animator.GetCurrentAnimatorStateInfo(player.Animator.GetLayerIndex("Attack Layer"));
        float normalizedTime = attackStateInfo.normalizedTime;

        // Access CombatController to check for current target.
        CombatController combat = player.GetComponent<CombatController>();

        if (chainActive)
        {
            // If new attack input, cycle combo
            if (player.Input.attack)
            {
                player.Input.attack = false;
                comboTimer = 0f;
                combat.DashToTarget(dashStep);
                // Attempt to dash toward target while still performing the attack combo.
                // Update combo count
                comboCount = (comboCount % 3) + 1;
                //Debug.Log("[AttackState] Combo input received. comboCount=" + comboCount);
                if (player.Animator != null)
                {
                    player.Animator.SetInteger("ComboCount", comboCount);
                    // Optionally re-trigger the attack
                    // player.Animator.SetTrigger("AttackTrigger");
                }
            }
            else
            {
                comboTimer += Time.deltaTime;
                // If no input within window OR animation almost done, end combo
                if (comboTimer > comboInputWindow || normalizedTime >= normalizedEndThreshold)
                {
                    chainActive = false;
                    //Debug.Log("[AttackState] Combo chain ended, going to 'sword idle' in Attack layer.");
                    if (player.Animator != null)
                        player.Animator.SetTrigger("AttackEnd");
                }
            }
        }
        else
        {
            // The chain is finished. Wait for next input to start a new chain
            if (player.Input.attack)
            {
                player.Input.attack = false;
                //Debug.Log("[AttackState] New chain started from sword idle.");
                comboCount = 1;
                chainActive = true;
                comboTimer = 0f;
                if (player.Animator != null)
                {
                    player.Animator.SetInteger("ComboCount", comboCount);
                    player.Animator.SetTrigger("AttackTrigger");
                }
                player.Animator.applyRootMotion = false;
                //dash to target
                combat.DashToTarget(dashStep);
            }
        }
    }
    
    public override void ExitState(PlayerStateManager player)
    {
        // Clear attack input
        player.Input.attack = false;

        // Blend out Attack layer
        int attackLayerIndex = player.Animator.GetLayerIndex("Attack Layer");
        player.StartCoroutine(player.BlendAttackLayerWeight(attackLayerIndex, 0.2f));

        // Hide the weapon
        WeaponController weaponController = player.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.HideWeapon();
            //Debug.Log("[AttackState] Attack weapon hidden.");
        }
    }
    
}