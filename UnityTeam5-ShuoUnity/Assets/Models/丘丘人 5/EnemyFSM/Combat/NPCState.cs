using UnityEngine;

public class NPCState : MonoBehaviour
{
    // Assign your NPCStateData asset in the Inspector.
    public NPCStateData npcStateData;

    // Current health is initialized based on the data asset.
    private float currentHealth;

    private void Start()
    {
        if (npcStateData != null)
        {
            currentHealth = npcStateData.maxHealth;
        }
        else
        {
            Debug.LogWarning("NPCStateData not assigned. Defaulting health to 100.");
            currentHealth = 100f;
        }
    }

    /// <summary>
    /// Apply damage to this NPC.
    /// </summary>
    /// <param name="amount">Damage amount to subtract from current health.</param>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Returns the damage value that this NPC can deal, factoring in a combo multiplier.
    /// </summary>
    /// <returns>Calculated attack damage.</returns>
    public float GetAttackDamage()
    {
        if (npcStateData != null)
        {
            return npcStateData.baseDamage * npcStateData.comboMultiplier;
        }
        return 10f; // Fallback value.
    }

    /// <summary>
    /// Returns the hit radius used for this NPC's attack detection.
    /// </summary>
    public float GetHitRadius()
    {
        if (npcStateData != null)
        {
            return npcStateData.hitRadius;
        }
        return 1.5f;
    }

    /// <summary>
    /// Handles the NPC's death.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Optionally, play a death animation or spawn particle effects here.
        Destroy(gameObject);
    }
}
