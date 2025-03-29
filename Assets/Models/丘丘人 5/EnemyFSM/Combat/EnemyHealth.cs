using UnityEngine;
using System.Collections;
using System;
using TMPro;
using System.Xml;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public Gender gender;
    public NPCStateData npcStateData;
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;
    public float fadeDuration = 1.0f;
    public Material deathMaterial;

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
    public bool isInitialized = false;

    // Reference to Enemy FSM
    private EnemyFSM fsm;

    // For floating damage text
    public GameObject floatingTextPrefab;

    void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<EnemyFSM>();
    }

    void Start()
    {
        if (npcStateData != null)
        {
            currentHealth = npcStateData.maxHealth;
        }
        else
        {
            currentHealth = 100f;
        }
        isInitialized = true;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth > 0)
        {
            StartCoroutine(GetHitRoutine());
        }
        else
        {
            // HP <= 0 => trigger "die" logic
            StartDieRoutine();
        }

        // Optional: Show floating damage text
        if (floatingTextPrefab)
        {
            ShowFloatingText(amount);
        }
        if (fsm != null && !fsm.isDead)
        {
            fsm.TransitionToState(fsm.gotHitState); // 讓敵人知道被打了
        }
    }

    IEnumerator GetHitRoutine()
    {
        GetComponent<PlayerAudio>()?.PlayGetHitSound();
        // Optional: spawn hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitFx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitFx, 0.4f);
        }

        // Trigger the GotHit animation
        //animator.SetTrigger("GotHit");

        // If the enemy was attacking, interrupt
        if (fsm != null && fsm.currentState is EnemyAttackState)
        {
            fsm.TransitionToState(fsm.gotHitState);
        }

        // Wait briefly for the hit reaction to play
        yield return new WaitForSeconds(0.4f);
    }

    // Instead of doing all in one DieRoutine, we do:
    private void StartDieRoutine()
    {
        if (isDead) return;
        isDead = true;
        // Notify FSM to stop AI
        if (fsm != null) fsm.isDead = true;
        GetComponent<PlayerAudio>()?.PlayDieSound();
        // Trigger "Die" animation
        animator.SetTrigger("Die");

        // Spawn a death effect, if any
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    // --- ANIMATION EVENT TRIGGER ---
    // Call this method near the end of the death animation via an Animation Event.
    public void OnDieAnimationEndEvent()
    {
        // Now we do the dissolve logic. 
        // Because the user won't do anything else to this object, we can be sure it's safe to do so.
        StartCoroutine(DissolveAndDestroy());
    }

    private IEnumerator DissolveAndDestroy()
    {
        // Get all renderers (cache them once)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Swap in dissolve material
        foreach (Renderer r in renderers)
        {
            if (r == null) continue; // null check
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = deathMaterial;
            }
            r.materials = mats;
        }

        // Dissolve effect over fadeDuration
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            // For each renderer, ensure it's still valid
            foreach (Renderer r in renderers)
            {
                if (r == null) continue; // null check
                Material[] mats = r.materials; // re-fetch the materials each frame
                for(int i = 0; i < mats.Length; i++)
                {
                    Material mat = mats[i];
                    if (mat != null && mat.HasProperty("_DissolveAmount"))
                    {
                        mat.SetFloat("_DissolveAmount", dissolveValue);
                    }
                }
            }
            yield return null;
        }

        // After the dissolve completes, destroy the object
        Destroy(gameObject);
        OnDeath?.Invoke();
    }

    //For floating damage text
    public void ShowFloatingText(float damage)
    {
        GameObject floatingText =Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        floatingText.GetComponent<TextMeshPro>().text = damage.ToString();

        // ���B�ʤ�r�¦V��v��
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            floatingText.transform.LookAt(mainCamera.transform.position);  // �¦V��v��
            floatingText.transform.Rotate(0f, 180f, 0f); // �קK��r�ϦV�]���ɭ� LookAt �|�ɭP��r����^
        }

        //// �i��G�[�W�ʵe�ήĪG
        //floatingText.transform.DOMoveY(floatingText.transform.position.y + 1f, 1f);
        //tmpText.DOFade(0f, 1f).OnComplete(() => Destroy(floatingText));
    }
}
