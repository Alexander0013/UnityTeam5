using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    // Assign your NPCStateData asset in the Inspector.
    public NPCStateData npcStateData;

    // Optional visual effects and material for death/dissolve.
    public GameObject hitEffectPrefab;      // Particle prefab to spawn when hit.
    public GameObject deathEffectPrefab;    // Particle prefab to spawn when dying.
    public float fadeDuration = 1.0f;         // Duration for the dissolve fade effect.
    public Material deathMaterial;            // Material that uses your dissolve shader.

    private float currentHealth;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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
    /// Call this method to apply damage to the enemy.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        // Trigger the get hit reaction.
        StartCoroutine(GetHit());

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// Plays hit effects and triggers the GetHit animation.
    /// </summary>
    IEnumerator GetHit()
    {
        if (hitEffectPrefab != null)
        {
            // Instantiate hit effect at the enemy's position.
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 0.5f);
        }
        animator.SetTrigger("GetHit");
        yield return new WaitForSeconds(0.53f); // Adjust if needed.
    }

    /// <summary>
    /// Plays the death animation and dissolves the enemy over time before destroying it.
    /// </summary>
    IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        animator.SetTrigger("Die");

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait briefly to allow the death animation to start.
        yield return new WaitForSeconds(0.5f);

        // Swap all renderer materials to the death material (assumed to use a dissolve shader).
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = deathMaterial;
            }
            r.materials = mats;
        }

        // Fade out enemy by updating the dissolve value.
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_DissolveAmount"))
                    {
                        mat.SetFloat("_DissolveAmount", dissolveValue);
                    }
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
