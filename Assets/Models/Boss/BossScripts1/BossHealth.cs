using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class BossHealth : MonoBehaviour, IDamageable
{
    public BossNPCStateData bossnpcStateData;
    public Material[] bossDeathMaterials;
    public GameObject deathEffectPrefab;
    public float fadeDuration = 1.0f;

    //public GameObject deathEffectPrefab;
    public GameObject hitEffectPrefab;

    // �s�W�G���w�S�ĥͦ��I�� Transform�]�Ҧp��b Boss �l���󤤡^
    public Transform hitEffectPoint;

    public float currentHealth;
    private Animator animator;
    private bool isDead = false;

    // Reference to your FSM if you want it:
    private BossFSM fsm;

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    public GameObject floatingTextPrefab;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<BossFSM>(); // If you want to notify the FSM of death
    }

    private void Start()
    {
        if (bossnpcStateData != null)
        {
            currentHealth = bossnpcStateData.maxHealth;
        }
        else
        {
            //Debug.LogWarning("NPCStateData not assigned. Defaulting health to 100.");
            currentHealth = 100f;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnHealthChanged.Invoke(currentHealth);
        //Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        // Trigger the get-hit reaction & animation if still alive
        if (currentHealth > 0)
        {
            StartCoroutine(GetHitRoutine());
        }
        else
        {
            // HP is zero or below
            StartCoroutine(DieRoutine());
        }
        if (floatingTextPrefab)
        {
            ShowFloatingText(amount);
        }
    }

    IEnumerator GetHitRoutine()
    {
        // �p�G hitEffectPrefab �P hitEffectPoint �����]�w�A�h�b hitEffectPoint ��m�ͦ��S��
        if (hitEffectPrefab != null && hitEffectPoint != null)
        {
            GameObject hitFx = Instantiate(hitEffectPrefab, hitEffectPoint.position, Quaternion.identity);
            Destroy(hitFx, 0.5f);
        }
        fsm.PLayGetHitSound();
        // Wait a fraction of a second to let the flinch play, if you like
        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator DieRoutine()
    {
        OnDeath.Invoke();
        if (isDead) yield break;
        isDead = true;

       

        //Debug.Log($"{gameObject.name} has died.");
        animator.SetTrigger("Die");

        // Spawn a death effect, if any
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Wait a short moment for the death animation
        yield return new WaitForSeconds(0.5f);

        // Swap in dissolve material
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            Material[] newMats;
            if (bossDeathMaterials != null && bossDeathMaterials.Length == r.materials.Length)
            {
                newMats = new Material[r.materials.Length];
                for (int i = 0; i < r.materials.Length; i++)
                {
                    newMats[i] = bossDeathMaterials[i];
                }
            }
            else
            {
                // Otherwise, use the current materials
                newMats = r.materials;
            }
            r.materials = newMats;
        }

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
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
        // Finally, destroy the object
        Destroy(gameObject);
    }

    public void ShowFloatingText(float damage)
    {
        GameObject floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        floatingText.GetComponent<TextMeshPro>().text = damage.ToString();

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            floatingText.transform.LookAt(mainCamera.transform.position);
            floatingText.transform.Rotate(0f, 180f, 0f);
        }
    }
}
