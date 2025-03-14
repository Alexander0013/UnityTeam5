using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossFSM : EnemyFSM
{
    // Override enemy states with boss-specific ones:
    public new BossIdleState idleState = new BossIdleState();
    public BossChaseState chaseState = new BossChaseState();
    public BossMeleeAttackState meleeAttackState = new BossMeleeAttackState();
    public BossRangedAttackState rangedAttackState = new BossRangedAttackState();
    public new EnemyDeadState deadState = new EnemyDeadState();
    public new EnemyGotHitState gotHitState = new EnemyGotHitState();

    // You can reuse npcData, detectionRadius, attackRadius, etc.
    // Ensure detectionRadius and attackRadius are set appropriately (e.g., attackRadius ~2.5, detectionRadius ~8)

    protected override void Start()
    {
        playerTarget = FindActiveLivingPlayer();
        if (playerTarget == null)
            TransitionToState(idleState);
        else
            TransitionToState(idleState);
    }
    private void Update()
    {
        // Skip updates if we're flagged dead or have no current state
        if (isDead || currentState == null) return;
        // 1) Check if the currently assigned player is valid
        if (playerTarget != null)
        {
            // If that player is no longer active or HP <= 0, set it to null
            if (!playerTarget.gameObject.activeInHierarchy || !IsPlayerAlive(playerTarget))
            {
                playerTarget = null;
                // Instead of immediately switching, start the wait coroutine if not already waiting.
                if (!waitingForReturn)
                    StartCoroutine(WaitAndReturnCoroutine());
            }
        }
        else
        {
            // 2) We have no current target. Try to find one.
            Transform newTarget = FindActiveLivingPlayer();
            if (newTarget != null)
            {
                // Found a new valid player => go back to Idle so we can detect and chase them.
                playerTarget = newTarget;
                TransitionToState(idleState);
            }
            // Otherwise, remain in the current state (which might be Return or Idle).
        }
        // Let our current state perform its update logic.
        currentState.UpdateState(this);
        LockYPosition();
    }
    // Called by the Animation Event in the melee attack clip at the hit frame.
    public void MeleeAttackHitEvent()
    {
        // Check that the boss is still in the melee attack state.
        if (currentState is BossMeleeAttackState)
        {
            // Apply damage to the player.
            ApplyAttackDamage();
        }
    }

    // Called by the Animation Event at the end of the melee attack animation.
    public void MeleeAttackEndEvent()
    {
        // Only transition if still in melee attack state.
        if (currentState is BossMeleeAttackState)
        {
            TransitionToState(idleState);
        }
    }

    // Called by the Animation Event in the ranged attack clip at the hit frame.
    public void RangedAttackHitEvent()
    {
        if (currentState is BossRangedAttackState)
        {
            ApplyRangedAttackDamage();
        }
    }

    // Called by the Animation Event at the end of the ranged attack animation.
    public void RangedAttackEndEvent()
    {
        if (currentState is BossRangedAttackState)
        {
            TransitionToState(idleState);
        }
    }
    public void ApplyRangedAttackDamage()
    {
        float damage = npcData.baseDamage * npcData.comboMultiplier;
        
        // Define a local range value for the ranged roar attack.
        float range = 10f; // You can adjust this value to fit your design.
        float coneAngle = 45f; // Damage only targets within a 45° cone.
        
        // Use the attackHitPoint as the origin of the attack.
        Vector3 attackOrigin = attackHitPoint.position;
        
        // Boss's forward direction.
        Vector3 forward = transform.forward;
        
        // Get all colliders within the defined range on the player layers.
        Collider[] hits = Physics.OverlapSphere(attackOrigin, range, npcData.playerLayers);
        
        foreach (Collider c in hits)
        {
            // Compute direction from the attack origin to the target.
            Vector3 toTarget = (c.transform.position - attackOrigin).normalized;
            
            // Check if the target is within the cone defined by forward and coneAngle.
            if (Vector3.Angle(forward, toTarget) <= coneAngle)
            {
                IDamageable dmg = c.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }
            }
        }
    }
    private void LockYPosition()
    {
        Vector3 pos = transform.position;
        pos.y = 0f; // Adjust this to your ground level if not zero.
        transform.position = pos;
    }


}
