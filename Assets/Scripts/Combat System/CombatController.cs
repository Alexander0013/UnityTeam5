using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public Transform attackHitPoint;

    [Header("Auto-Target Settings")]
    public float autoTargetRadius = 6f;
    public float autoTargetAngle = 90f;
    public LayerMask enemyLayer;

    [Header("VFX")]
    public GameObject electroSlashVFX; // Assign your slash VFX prefab here.

    // Track the current dash coroutine to avoid multiple dashes overlapping.
    private Coroutine dashCoroutine;

    [HideInInspector]
    public Transform currentTarget;
    [HideInInspector]
    public AttackData playerAttackData;
    void Awake() 
    {
        PlayerHealth ph = GetComponentInParent<PlayerHealth>();
        if(ph != null)
            playerAttackData = ph.playerAttackData;
    }
    /// <summary>
    /// Called via an Animation Event at the hit frame of your attack animation.
    /// It performs hit detection and instantiates the slash VFX.
    /// </summary>
    public void PerformHitDetection()
{
    // Play the attack sound using the player's common audio component.
    GetComponent<PlayerAudio>()?.PlayAttackSound();

    // Get the WeaponController from the player.
    WeaponController wc = GetComponentInParent<WeaponController>();

    // Determine spawn position and rotation for the attack effects.
    Quaternion spawnRotation = Quaternion.identity;
    Vector3 spawnPosition = Vector3.zero;

    // First try to use the attackHitPoint (if it hasn’t been destroyed).
    if (attackHitPoint != null)
    {
        spawnRotation = attackHitPoint.rotation;
        spawnPosition = attackHitPoint.position;
    }
    // If attackHitPoint is missing, fall back to the attackAttach from the WeaponController.
    else if (wc != null && wc.attackAttach != null)
    {
        spawnRotation = wc.attackAttach.rotation;
        spawnPosition = wc.attackAttach.position;
    }
    else
    {
        Debug.LogWarning("No valid attack point found!");
        return;
    }

    // For the VFX, prefer the dedicated slashSpawn from the WeaponController if available.
    if (wc != null && wc.slashSpawn != null)
    {
        spawnRotation = wc.slashSpawn.rotation;
        spawnPosition = wc.slashSpawn.position;
    }

    // Instantiate the slash VFX at the chosen spawn position and rotation.
    if (electroSlashVFX != null)
    {
        GameObject vfxInstance = Instantiate(electroSlashVFX, spawnPosition, spawnRotation);
        ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            float destroyDelay = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(vfxInstance, destroyDelay);
        }
        else
        {
            Destroy(vfxInstance, 1.5f);
        }
    }

    // Damage calculation: use the spawnPosition as the origin.
    float damage = playerAttackData.baseDamage * playerAttackData.comboMultiplier;
    Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, playerAttackData.hitRadius, playerAttackData.enemyLayers);
    foreach (Collider hit in hitColliders)
    {
        IDamageable damageable = hit.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            // Apply element effect if applicable.
            ElementalStatus targetStatus = hit.GetComponent<ElementalStatus>();
            if (targetStatus != null)
            {
                targetStatus.ApplyElement(playerAttackData.element, 15f);
            }
        }
    }
}



    /// <summary>
    /// Rotates the player to face the target enemy.
    /// </summary>
    public void FaceTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f; // Only rotate horizontally.
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    /// <summary>
    /// Tries to auto-target the closest enemy within range and angle.
    /// </summary>
    public void TryAutoTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, autoTargetRadius, enemyLayer);

        if (colliders.Length == 0)
        {
            currentTarget = null;
            return;
        }

        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider c in colliders)
        {
            Vector3 dirToEnemy = c.transform.position - transform.position;
            float dist = dirToEnemy.magnitude;
            float angle = Vector3.Angle(transform.forward, dirToEnemy.normalized);
            if (angle <= autoTargetAngle)
            {
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = c.transform;
                }
            }
        }

        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            //Debug.Log($"[CombatController] Auto-target locked on {bestTarget.name}");
        }
        else
        {
            currentTarget = null;
            //Debug.Log("[CombatController] No valid target in range.");
        }
    }
    
    public void DashToTarget(float dashStep)
{
    // Use this.transform because CombatController is on the player.
    Transform playerTransform = transform;

    if (currentTarget != null && playerAttackData != null)
    {
        // Calculate the horizontal direction from player to target.
        Vector3 dashDirection = currentTarget.position - playerTransform.position;
        dashDirection.y = 0f;
        if (dashDirection.sqrMagnitude < 0.01f)
            return;
        dashDirection.Normalize();

        // Rotate the player to face the target.
        Quaternion targetRotation = Quaternion.LookRotation(dashDirection, Vector3.up);
        playerTransform.rotation = targetRotation;

        // Log computed dashDirection for debugging.
        //Debug.Log("[DashToTarget] Computed dashDirection: " + dashDirection);

        // Calculate the distance to the target.
        float dist = Vector3.Distance(playerTransform.position, currentTarget.position);
        //Debug.Log($"[DashToTarget] Distance to target: {dist}, HitRadius: {currentAttackData.hitRadius}");

        // If the target is farther than the hit radius, dash toward it.
        if (dist > playerAttackData.hitRadius )
        {
            // Move using the computed dashDirection. Multiply by Time.deltaTime for frame-rate independent movement.
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(dashDirection * dashStep * Time.deltaTime * 15f);
                //Debug.Log("[DashToTarget] Dashing toward target in direction: " + dashDirection);
            }
        }
    }
}
    


}
