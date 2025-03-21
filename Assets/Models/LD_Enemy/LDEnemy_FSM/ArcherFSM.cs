using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcherFSM : EnemyFSM
{
    public new ArcherIdleState idleState = new ArcherIdleState();
    public ArcherShootState shootState = new ArcherShootState();
    public ArcherMeleeState meleeState = new ArcherMeleeState();
    public new ArcherDeadState deadState = new ArcherDeadState();
    public new EnemyGotHitState gotHitState = new EnemyGotHitState();

    [Header("Archer Specific Settings")]
    public Transform arrowSpawnPoint;
    public GameObject muzzleFlashPrefab;
    public GameObject arrowImpactEffect;
    public GameObject meleeImpactEffect;
    public GameObject tracerPrefab;

    protected override void Start()
    {
        playerTarget = FindActiveLivingPlayer();
        TransitionToState(idleState);
    }

    protected override void Update()
    {
        if (playerTarget != null)
        {
            if (!playerTarget.gameObject.activeInHierarchy || !IsPlayerAlive(playerTarget))
            {
                playerTarget = null;
            }
        }
        else
        {
            Transform newTarget = FindActiveLivingPlayer();
            if (newTarget != null)
            {
                playerTarget = newTarget;
                TransitionToState(idleState);
            }
        }
        currentState.UpdateState(this);
    }
    public void OnShootHitEvent()
    {
        if (currentState is ArcherShootState)
        {
            if (playerTarget != null && arrowSpawnPoint != null)
            {
                GetComponent<ShooterAudio>()?.PlayAttackSound();
                Vector3 origin = arrowSpawnPoint.position;
                Vector3 direction = (playerTarget.position - origin).normalized;
                if (muzzleFlashPrefab != null)
                {
                    Quaternion correctRotation = arrowSpawnPoint.rotation * Quaternion.Euler(0, 180, 0);
                    GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, origin, correctRotation);
                    Destroy(muzzleFlash, 2f); // Destroy after 0.5 seconds
                }
                float sphereRadius = 0.5f; // Increase or decrease based on your needs
                RaycastHit hit;
                // SphereCast with a max distance of 20 units.
                if (Physics.SphereCast(origin, sphereRadius, direction, out hit, 20f, npcData.playerLayers))
                {
                    // For debugging: visualize the spherecast.
                    Debug.DrawRay(origin, direction * 20f, Color.red, 1f);

                    IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                    if (dmg != null)
                    {
                        float damage = npcData.baseDamage * npcData.comboMultiplier;
                        dmg.TakeDamage(damage);
                    }
                    if (arrowImpactEffect != null)
                    {
                        Instantiate(arrowImpactEffect, hit.point, Quaternion.identity);
                    }
                }
                else
                {
                    // For debugging if the spherecast misses.
                    Debug.DrawRay(origin, direction * 20f, Color.blue, 1f);
                }
            }
        }
    }
    // Called via Animation Event at the end of the Shoot animation.
    public void OnShootEndEvent()
    {
        if (currentState is ArcherShootState)
        {
            TransitionToState(idleState);
        }
    }

    // Called via Animation Event during the MeleeKick animation hit frame.
    public void OnMeleeHitEvent()
    {
        if (currentState is ArcherMeleeState)
        {
            GetComponent<ShooterAudio>()?.PlayLegKickSound();
            float damage = npcData.baseDamage * npcData.comboMultiplier;
            float meleeRadius = npcData.hitRadius;
            Vector3 center = attackHitPoint.position;
            Collider[] hits = Physics.OverlapSphere(center, meleeRadius, npcData.playerLayers);
            foreach (Collider c in hits)
            {
                IDamageable dmg = c.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }
                if (meleeImpactEffect != null)
                {
                    Instantiate(meleeImpactEffect, center, Quaternion.identity);
                }
            }
        }
    }

    // Called via Animation Event at the end of the MeleeKick animation.
    public void OnMeleeEndEvent()
    {
        if (currentState is ArcherMeleeState)
        {
            TransitionToState(idleState);
        }
    }
}
