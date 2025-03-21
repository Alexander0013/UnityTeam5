using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10f;
    public GameObject deathEffectPrefab;
    public GameObject hitEffectPrefab;  // Assign your hit effect particle prefab here
    public float fadeDuration = 1.0f; // Duration for the dissolve fade effect
    public Material deathMaterial;  // Assign this in the Inspector – this material should use your dissolve shader

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");
        
        StartCoroutine(GetHit());

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator GetHit()
    {
        // Instantiate the hit effect at the enemy's position.
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            // Optionally destroy the hit effect after 0.5 second.
            Destroy(hitEffect, 0.5f);
        }
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("GetHit");
        yield return new WaitForSeconds(0.53f);
        
    }

    IEnumerator Die()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");

        // Optionally spawn a death effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait a brief moment for the death animation to start
        yield return new WaitForSeconds(0.5f);

        // Swap to the death material (which has the dissolve shader)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            // Replace each renderer's material with the death material.
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = deathMaterial;
            }
            r.materials = mats;
        }

        // Fade out the enemy using the dissolve effect.
        // Smoothly fade out by animating _DissolveAmount from 0 to 1
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

        // After the fade-out is complete, destroy the enemy.
        Destroy(gameObject);
    }
}
