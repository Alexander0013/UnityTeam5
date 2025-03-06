using System.Collections;
using UnityEngine;

public class ChurlAttackState : ChurlBase
{
    // Assign these in the Inspector:
    public Transform attackHitPoint;  // Place this at the tip of the enemy's weapon or appropriate hit area.
    public NPCStateData npcStateData;   // Holds enemy attack values and player layer mask.
    private float attackInterval = 1f;  // �������j1��
    private float attackRange = 1f;     // �P���ު��A�������d��ۦP
    private Coroutine attackCoroutine;
    private float outOfRangeTimer = 0f;

    private float minAttackInterval = 1f;
    private float maxAttackInterval = 3f;
    //public object PlayerHealth;

    // Cached reference to the player's health component
    private PlayerHealth cachedPlayerHealth;


    public override void Enter()
    {
        
        if (churl == null)
        {
            churl = GetComponent<Churl>();
        }
        if (churl != null)
        {
            churlObject = churl.gameObject;
        }
        if (animator == null)
        {
            animator = churlObject.GetComponent<Animator>();
            Debug.Log("find animator");
        }
        // ���� Animator �� attackLayer�]���] attackLayer �w�b Animator ���]�w�^
        //StartCoroutine(SmoothSetAnimatorLayerWeight("attackLayer", 1f));

        //StartCoroutine(SmoothSetAnimatorLayerWeight("walkLayer", 0f));

        SetAnimatorLayerWeight("attackLayer", 1);
        SetAnimatorLayerWeight("walkLayer", 0);
        attackCoroutine = StartCoroutine(AttackRoutine());

        Debug.Log("Enter churl attack");

        // Cache the player's health component once here:
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cachedPlayerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("Player not found when caching health!");
        }

    }

    public override void Update()
    {
        Debug.Log("CASUpdate");
        if (churl == null) return;
        
        // Check if player's health is 0 or below.
        if (cachedPlayerHealth != null && cachedPlayerHealth.CurrentHealth <= 0)
        {
            Debug.Log("Player is dead. Switching to PatrolState.");
            churl.ChangeState(new ChurlPatrolState());
            return;
        }

        // Fallback: if cached reference is lost, try to get it once more
        if (cachedPlayerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                cachedPlayerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        GameObject playerObj = cachedPlayerHealth != null ? cachedPlayerHealth.gameObject : GameObject.FindWithTag("Player");
        if (playerObj != null && churlObject != null)
        {
            float distance = Vector3.Distance(churlObject.transform.position, playerObj.transform.position);
            // If the player is significantly farther than the attack range, increment a timer.
            if (distance >= attackRange * 1.2f)
            {
                outOfRangeTimer += Time.deltaTime;
                if (outOfRangeTimer >= 0.5f)
                {
                    Debug.Log("Player is out of range for 0.5 sec, switching to PatrolState.");
                    churl.ChangeState(new ChurlPatrolState());
                    return;
                }
            }
            else
            {
                // Reset the timer if the player comes back into range.
                outOfRangeTimer = 0f;
            }
        }
    }


    public override void Exit()
    {
        Debug.Log("���}�������A");
        SetAnimatorLayerWeight("attackLayer", 0);
        if (attackCoroutine != null)
        {
            churl.StopCoroutine(attackCoroutine);
            attackCoroutine = null;  // �T�O������{�Q�M�šA�קK�ª���{�v�T�s���A
        }
    }

    
    private IEnumerator AttackRoutine()
    {
        // Optional: add an initial random delay so enemies don't all start at the same time.
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        while (true)
        {
            animator.SetTrigger("Attack");
            // Wait for a random duration between min and max attack intervals
            float delay = Random.Range(minAttackInterval, maxAttackInterval);
            yield return new WaitForSeconds(delay);
        }
    }
    /// <summary>
    /// This function is called by an Animation Event in the enemy attack animation.
    /// It performs hit detection against the player.
    /// </summary>
    public void PerformAttackHitDetection()
    {
        Debug.Log("Enemy PerformAttackHitDetection");

        if (npcStateData == null)
        {
            Debug.LogWarning("NPCStateData not assigned.");
            return;
        }

        // Calculate the damage based on NPCStateData.
        float damage = npcStateData.baseDamage * npcStateData.comboMultiplier;

        // Use OverlapSphere to detect the player.
        Collider[] hitColliders = Physics.OverlapSphere(
            attackHitPoint.position,
            npcStateData.hitRadius,
            npcStateData.playerLayers  // Make sure the player's layer is included in this mask.
        );

        foreach (Collider hit in hitColliders)
        {
            // Attempt to get the player's IDamageable component.
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.LogWarning("apply damage");
                damageable.TakeDamage(damage);
            }
        }

        Debug.DrawRay(attackHitPoint.position, Vector3.one * npcStateData.hitRadius, Color.red, 1f);
    }

}

