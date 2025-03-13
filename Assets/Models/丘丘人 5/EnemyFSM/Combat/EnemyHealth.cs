using UnityEngine;
using System.Collections;
using System;
using TMPro;
using System.Xml;

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

    private float _currentHealth;
    public float currentHealth
    {
        get { return _currentHealth; }
        private set
        {
            _currentHealth = value;
            OnHealthChanged?.Invoke(_currentHealth);
        }
    }

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public bool isInitialized=false;

    // Reference to your FSM if you want it:
    private EnemyFSM fsm;

    //For floating damage text
    public GameObject floatingTextPrefab;

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
            currentHealth = 100f;
        }
        isInitialized= true;
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
        
        
        //For floating damage text
        if (floatingTextPrefab)
        {
            ShowFloatingText(amount);
        }
    }

    IEnumerator GetHitRoutine()
{
    // Optionally spawn hit effect
    if (hitEffectPrefab != null)
    {
        GameObject hitFx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        Destroy(hitFx, 0.5f);
    }

    // Trigger the GotHit animation
    animator.SetTrigger("GotHit");

    // If the enemy is currently attacking, interrupt that attack
    if (fsm != null && fsm.currentState is EnemyAttackState)
    {
        fsm.TransitionToState(fsm.gotHitState);
    }

    // Wait briefly for the hit reaction to play
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
        OnDeath?.Invoke();
    }

    //For floating damage text
    public void ShowFloatingText(float damage)
    {
        GameObject floatingText =Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        floatingText.GetComponent<TextMeshPro>().text = damage.ToString();

        // 讓浮動文字朝向攝影機
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            floatingText.transform.LookAt(mainCamera.transform.position);  // 朝向攝影機
            floatingText.transform.Rotate(0f, 180f, 0f); // 避免文字反向（有時候 LookAt 會導致文字反轉）
        }

        //// 可選：加上動畫或效果
        //floatingText.transform.DOMoveY(floatingText.transform.position.y + 1f, 1f);
        //tmpText.DOFade(0f, 1f).OnComplete(() => Destroy(floatingText));
    }
}
