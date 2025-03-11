using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Attack Data")]
    public AttackData currentAttackData;
    public Transform attackHitPoint; // Set this in the Inspector.

    [Header("Auto-Target Settings")]
    public float autoTargetRadius = 5f;
    public float autoTargetAngle = 90f;
    public LayerMask enemyLayer;

    [Header("VFX")]
    public GameObject electroSlashVFX; // Assign your slash VFX prefab here.

    // Track the current dash coroutine to avoid multiple dashes overlapping.
    private Coroutine dashCoroutine;

    [HideInInspector]
    public Transform currentTarget;

    /// <summary>
    /// Called via an Animation Event at the hit frame of your attack animation.
    /// It performs hit detection and instantiates the slash VFX.
    /// </summary>
    public void PerformHitDetection()
    {
        Debug.Log("PerformHitDetection");
        if (currentAttackData == null)
        {
            Debug.LogWarning("AttackData is not assigned.");
            return;
        }

        // Get the WeaponController from the player (assume it's on the same object or parent).
        WeaponController wc = GetComponentInParent<WeaponController>();
        Quaternion spawnRotation = attackHitPoint.rotation;
        if (wc != null && wc.attackAttach != null)
        {
            // Use the attackAttach rotation so the VFX follows the weapon's attack direction.
            spawnRotation = wc.attackAttach.rotation;
        }

        // Instantiate the slash VFX at the attack hit point, using the desired rotation.
        if (electroSlashVFX != null)
        {
            // Use the dedicated slash spawn transform
            Quaternion SpawnRotation = wc.slashSpawn.rotation;
            Vector3 spawnPosition = wc.slashSpawn.position;
            GameObject vfxInstance = Instantiate(electroSlashVFX, spawnPosition, SpawnRotation);
            // Optionally, parent the instance so it follows for a brief moment:
            // vfxInstance.transform.SetParent(wc.slashSpawn);

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

        // Damage calculation remains unchanged.
        float damage = currentAttackData.baseDamage * currentAttackData.comboMultiplier;
        Collider[] hitColliders = Physics.OverlapSphere(
            attackHitPoint.position,
            currentAttackData.hitRadius,
            currentAttackData.enemyLayers
        );

        foreach (Collider hit in hitColliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Debug.DrawRay(attackHitPoint.position, Vector3.one * currentAttackData.hitRadius, Color.red, 1f);
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
            Debug.Log($"[CombatController] Auto-target locked on {bestTarget.name}");
        }
        else
        {
            currentTarget = null;
            Debug.Log("[CombatController] No valid target in range.");
        }
    }
    
    public void DashToTarget(float dashStep)
{
    // Use this.transform because CombatController is on the player.
    Transform playerTransform = transform;

    if (currentTarget != null && currentAttackData != null)
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
        Debug.Log("[DashToTarget] Computed dashDirection: " + dashDirection);

        // Calculate the distance to the target.
        float dist = Vector3.Distance(playerTransform.position, currentTarget.position);
        Debug.Log($"[DashToTarget] Distance to target: {dist}, HitRadius: {currentAttackData.hitRadius}");

        // If the target is farther than the hit radius, dash toward it.
        if (dist > currentAttackData.hitRadius )
        {
            // Move using the computed dashDirection. Multiply by Time.deltaTime for frame-rate independent movement.
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(dashDirection * dashStep * Time.deltaTime * 15f);
                Debug.Log("[DashToTarget] Dashing toward target in direction: " + dashDirection);
            }
        }
    }
}
    
    /*
    /// <summary>
    /// Starts dashing toward the current target. Uses a coroutine to update movement over frames.
    /// </summary>
    /// <param name="dashSpeed">The movement speed for the dash.</param>
    public void DashToTarget(float dashSpeed)
    {
        // Stop any existing dash coroutine before starting a new one.
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
        dashCoroutine = StartCoroutine(DashToTargetCoroutine(dashSpeed));
    }

    /// <summary>
    /// Coroutine that dashes the player toward the target over multiple frames.
    /// </summary>
    /// <param name="dashSpeed">The speed of the dash.</param>
    private IEnumerator DashToTargetCoroutine(float dashSpeed)
    {
        if (currentTarget == null || currentAttackData == null)
            yield break;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc == null)
            yield break;

        // Define a maximum duration for the dash to prevent overshooting.
        float maxDashTime = 0.3f;
        float dashTimer = 0f;

        while (dashTimer < maxDashTime)
        {
            Vector3 dashDirection = currentTarget.position - transform.position;
            dashDirection.y = 0f;

            if (dashDirection.sqrMagnitude < 0.01f)
                break;
            dashDirection.Normalize();

            // Rotate the player to face the target each frame.
            Quaternion targetRotation = Quaternion.LookRotation(dashDirection, Vector3.up);
            transform.rotation = targetRotation;

            float dist = Vector3.Distance(transform.position, currentTarget.position);
            // Stop dashing if the player is within the desired distance.
            if (dist <= currentAttackData.hitRadius)
                break;

            cc.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer += Time.deltaTime;

            yield return null;
        }
    }
    */


}
