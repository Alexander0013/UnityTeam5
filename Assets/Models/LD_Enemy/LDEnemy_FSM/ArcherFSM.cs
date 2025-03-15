using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcherFSM : EnemyFSM
{
    public new ArcherIdleState idleState = new ArcherIdleState();
    public ArcherShootState shootState = new ArcherShootState();
    public ArcherMeleeState meleeState = new ArcherMeleeState();
    public new EnemyDeadState deadState = new EnemyDeadState();
    public new EnemyGotHitState gotHitState = new EnemyGotHitState();

    [Header("Archer Specific Settings")]
    public Transform arrowSpawnPoint;      
    public GameObject arrowImpactEffect;    
    public float shootCooldown = 2f;        

    protected override void Start()
    {
        playerTarget = FindActiveLivingPlayer();
        TransitionToState(idleState);
    }


    public void OnShootHitEvent()
    {
        if (currentState is ArcherShootState)
        {
            // Use raycast for ranged damage.
            if (playerTarget != null && arrowSpawnPoint != null)
            {
                Vector3 origin = arrowSpawnPoint.position;
                Vector3 direction = (playerTarget.position - origin).normalized;
                RaycastHit hit;
                // For example, a maximum distance of 20 units.
                if (Physics.Raycast(origin, direction, out hit, 20f, npcData.playerLayers))
                {
                    // If we hit a damageable target, apply damage.
                    IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                    if (dmg != null)
                    {
                        float damage = npcData.baseDamage * npcData.comboMultiplier;
                        dmg.TakeDamage(damage);
                    }
                    // Instantiate special effect at hit point.
                    if (arrowImpactEffect != null)
                    {
                        Instantiate(arrowImpactEffect, hit.point, Quaternion.identity);
                    }
                }
            }
        }
    }
    public void OnShootEndEvent()
    {
        if (currentState is ArcherShootState)
        {
            TransitionToState(idleState);
        }
    }
    public void OnMeleeHitEvent()
    {
        if (currentState is ArcherMeleeState)
        {
            // Use a sphere check for melee damage.
            float damage = npcData.baseDamage * npcData.comboMultiplier;
            float meleeRadius = npcData.hitRadius; // or set a custom value
            Vector3 center = attackHitPoint.position; // reuse the same point
            Collider[] hits = Physics.OverlapSphere(center, meleeRadius, npcData.playerLayers);
            foreach (Collider c in hits)
            {
                IDamageable dmg = c.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }
            }
        }
    }
    public void OnMeleeEndEvent()
    {
        if (currentState is ArcherMeleeState)
        {
            TransitionToState(idleState);
        }
    }
}
