using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Attack Data")]
    public AttackData currentAttackData;
    public Transform attackHitPoint; // Set this in the Inspector.

    [Header("Auto-Target Settings")]
    public float autoTargetRadius = 5f;     // How far to search for enemies
    public float autoTargetAngle = 60f;     // Must be within this angle from forward
    public LayerMask enemyLayer;            // The layer used by enemy colliders

    [HideInInspector]
    public Transform currentTarget;         // The target we selected (if any)

    /// <summary>
    /// Call this from an "Attack" button press or AttackState, before you actually trigger your attack animation.
    /// This method attempts to find the "best" enemy in front of you within autoTargetRadius/autoTargetAngle.
    /// If found, it orients you toward that enemy (soft lock-on), and sets currentTarget to that transform.
    /// </summary>
    public void TryAutoTarget()
    {
        // 1) Find all enemies in a sphere around the player
        Collider[] colliders = Physics.OverlapSphere(transform.position, autoTargetRadius, enemyLayer);

        if (colliders.Length == 0)
        {
            currentTarget = null; // no enemy found
            return;
        }

        // 2) Filter by angle and pick the closest
        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider c in colliders)
        {
            Vector3 dirToEnemy = c.transform.position - transform.position;
            float dist = dirToEnemy.magnitude;
            float angle = Vector3.Angle(transform.forward, dirToEnemy.normalized);

            // Check if within our angle cone
            if (angle <= autoTargetAngle)
            {
                // Check if it's the closest
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = c.transform;
                }
            }
        }

        // 3) If we found a target, rotate toward it
        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            FaceTarget(bestTarget);
            Debug.Log($"[CombatController] Auto-target locked on {bestTarget.name}");
        }
        else
        {
            currentTarget = null;
            Debug.Log("[CombatController] No valid target in angle range.");
        }
    }

    /// <summary>
    /// Rotate the player horizontally to face the target.
    /// If your game allows vertical aiming, remove the 'y = 0f' line.
    /// </summary>
    private void FaceTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f; // Keep the rotation horizontal
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    /// <summary>
    /// This is called via an Animation Event at the hit frame of your attack animation.
    /// It uses OverlapSphere to see which enemies were hit. 
    /// </summary>
    public void PerformHitDetection()
    {
        Debug.Log("PerformHitDetection");
        if (currentAttackData == null)
        {
            Debug.LogWarning("AttackData is not assigned.");
            return;
        }

        float damage = currentAttackData.baseDamage * currentAttackData.comboMultiplier;

        // Perform a sphere overlap around 'attackHitPoint'
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
}
