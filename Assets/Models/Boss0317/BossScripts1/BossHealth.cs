using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour, IDamageable
{
    public BossNPCStateData bossnpcStateData;
    public GameObject deathEffectPrefab;
    public float fadeDuration = 1.0f;

    //public GameObject deathEffectPrefab;
    public GameObject hitEffectPrefab;

    // 新增：指定特效生成點的 Transform（例如放在 Boss 子物件中）
    public Transform hitEffectPoint;

    private float currentHealth;
    private Animator animator;
    private bool isDead = false;

    // Reference to your FSM if you want it:
    private EnemyFSM fsm;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<EnemyFSM>(); // If you want to notify the FSM of death
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
        // 如果 hitEffectPrefab 與 hitEffectPoint 均有設定，則在 hitEffectPoint 位置生成特效
        if (hitEffectPrefab != null && hitEffectPoint != null)
        {
            GameObject hitFx = Instantiate(hitEffectPrefab, hitEffectPoint.position, Quaternion.identity);
            Destroy(hitFx, 0.5f);
        }
        // Wait a fraction of a second to let the flinch play, if you like
        yield return new WaitForSeconds(0.5f);

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
                //mats[i] = deathob;
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
