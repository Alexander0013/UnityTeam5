using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcherFSM : EnemyFSM
{
    // Replace melee states with archer-specific ones.
    public new ArcherIdleState idleState = new ArcherIdleState();
    public ArcherShootState shootState = new ArcherShootState();
    public new ArcherReturnState returnState = new ArcherReturnState();
    public new ArcherDeadState deadState = new ArcherDeadState();

    [Header("Archer Specific Settings")]
    public Transform arrowSpawnPoint;      // Where arrows spawn
    public GameObject arrowPrefab;         // The arrow projectile prefab
    public float arrowSpeed = 25f;         // Projectile speed

    // For shooting timing (cooldown between shots)
    [HideInInspector] public float shootCooldown = 2f;
    [HideInInspector] public float shootTimer = 0f;

    // Override Start so that we use archer states.
    protected override void Start()
    {
        // Instead of using EnemyFSM’s Start, we re-implement it:
        playerTarget = FindActiveLivingPlayer();
        if (playerTarget == null)
            TransitionToState(returnState);
        else
            TransitionToState(idleState);
    }

    /// <summary>
    /// Called via Animation Event in the shoot animation.
    /// </summary>
    public void SpawnArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = arrowSpawnPoint.forward * arrowSpeed;
        }
        // You can also add damage or lifetime logic to the arrow here.
    }
}
