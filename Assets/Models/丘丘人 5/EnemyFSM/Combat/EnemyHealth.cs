using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public NPCStateData npcStateData;
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;
    public float fadeDuration = 1.0f;
    public Material deathMaterial;

    //private float currentHealth;
    private Animator animator;
    private bool isDead = false;

    public float currentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;
            OnHealthChange?.Invoke();
        }
    }
    public event System.Action OnHealthChange;

    // Reference to your FSM if you want it:
    private EnemyFSM fsm;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<EnemyFSM>(); // If you want to notify the FSM of death
    }

    private void Start()
    {
        if (npcStateData != null)
        {
            currentHealth = npcStateData.maxHealth;
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
    }

    IEnumerator GetHitRoutine()
    {
        // Optional: spawn a hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitFx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitFx, 0.5f);
        }

        // Trigger the GetHit animation
        animator.SetTrigger("GotHit");

        // Wait a fraction of a second to let the flinch play, if you like
        yield return new WaitForSeconds(0.5f);

        // If you want the FSM to do something special (like not chase for a moment),
        // you can do so here, or rely on the AnyState->GotHit animator transitions
    }

    IEnumerator DieRoutine()
    {
        if (isDead) yield break;
        isDead = true;

        //Debug.Log($"{gameObject.name} has died.");
        animator.SetTrigger("Die");

        // (Optional) Let the FSM know we're dead so it can stop AI logic
        if (fsm != null)
        {
            fsm.isDead = true; 
            // or do fsm.TransitionToState(fsm.deadState), if you want
        }

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
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = deathMaterial;
            }
            r.materials = mats;
        }

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

        // Finally, destroy the object
        Destroy(gameObject);
    }
}
