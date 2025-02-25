using UnityEngine;

public class CombatController : MonoBehaviour
{
    public AttackData currentAttackData;
    public Transform attackHitPoint; // Set this in the Inspector.

    // Called via an Animation Event at the hit frame.
    public void PerformHitDetection()
    {
        Debug.Log("PerformHitDetection");
        if (currentAttackData == null)
        {
            Debug.LogWarning("AttackData is not assigned.");
            return;
        }

        float damage = currentAttackData.baseDamage * currentAttackData.comboMultiplier;
        Collider[] hitColliders = Physics.OverlapSphere(attackHitPoint.position, currentAttackData.hitRadius, currentAttackData.enemyLayers);
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
