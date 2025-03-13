using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class EnemyFSM : MonoBehaviour
{
    // Make the enemy list public so other states can check for blocking
    public static List<EnemyFSM> AllEnemies = new List<EnemyFSM>();
    // References to each state
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyAttackState attackState = new EnemyAttackState();
    public EnemyGotHitState gotHitState = new EnemyGotHitState();
    public EnemyReturnState returnState = new EnemyReturnState();
    public EnemyDeadState deadState = new EnemyDeadState();
    [Header("Enemy Settings")]
    public NPCStateData npcData;
    public float detectionRadius = 8f;
    public float attackRadius = 1.2f;
    public Transform treasureTransform;
    public float treasureReturnRadius = 1f;
    public Transform attackHitPoint;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public bool isDead;
    [HideInInspector] public EnemyBaseState currentState;
    private static List<EnemyFSM> allEnemies = new List<EnemyFSM>(); // For team collision avoidance
    private bool waitingForReturn = false;
    private void Awake()
    {
        allEnemies.Add(this);
        animator = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        // Attempt an initial player find
        playerTarget = FindActiveLivingPlayer();
        // Start in Idle (or ReturnState if no player found)
        if (playerTarget == null)
        {
            TransitionToState(returnState);
        }
        else
        {
            TransitionToState(idleState);
        }
    }
    private void Update()
    {
        // Skip updates if we're flagged dead or have no current state
        if (isDead || currentState == null) return;
        // 1) Check if the currently assigned player is valid
        if (playerTarget != null)
        {
            // If that player is no longer active or HP <= 0, set it to null
            if (!playerTarget.gameObject.activeInHierarchy || !IsPlayerAlive(playerTarget))
            {
                playerTarget = null;
                // Instead of immediately switching, start the wait coroutine if not already waiting.
                if (!waitingForReturn)
                    StartCoroutine(WaitAndReturnCoroutine());
            }
        }
        else
        {
            // 2) We have no current target. Try to find one.
            Transform newTarget = FindActiveLivingPlayer();
            if (newTarget != null)
            {
                // Found a new valid player => go back to Idle so we can detect and chase them.
                playerTarget = newTarget;
                TransitionToState(idleState);
            }
            // Otherwise, remain in the current state (which might be Return or Idle).
        }
        float distanceFromTreasure = Vector3.Distance(transform.position, transform.parent.position);
        if (distanceFromTreasure > 20f)
        {
            TransitionToState(returnState);
        }
        // Let our current state perform its update logic.
        currentState.UpdateState(this);
        PreventTeamCollision();
    }
    public void TransitionToState(EnemyBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }
    private void OnDestroy()
    {
        allEnemies.Remove(this);
    }
    private void PreventTeamCollision()
    {
        // your existing separation logic
        foreach (EnemyFSM otherEnemy in allEnemies)
        {
            if (otherEnemy == this) continue;
            if (otherEnemy == null) continue;
            float dist = Vector3.Distance(transform.position, otherEnemy.transform.position);
            if (dist < 1f && dist > 0f)  // you can use separationRadius
            {
                Vector3 pushDir = (transform.position - otherEnemy.transform.position);
                pushDir.y = 0f;
                pushDir = pushDir.normalized;
                transform.position += pushDir * (2f * Time.deltaTime);
            }
        }
    }
    /// <summary>
    /// Returns the first valid player transform that is activeInHierarchy and has HP > 0,
    /// or null if none found.
    /// </summary>
    protected Transform FindActiveLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy && IsPlayerAlive(p.transform))
            {
                return p.transform;
            }
        }
        return null;
    }
    private bool IsPlayerAlive(Transform playerTransform)
    {
        // Check the player's HP
        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph == null) return false;
        return (ph.CurrentHealth > 0);
    }
    private IEnumerator WaitAndReturnCoroutine()
    {
        waitingForReturn = true;
        //Debug.Log("Player dead detected. Waiting 2 seconds before returning...");
        yield return new WaitForSeconds(2f);
        // Double-check that the player is still dead (playerTarget is null)
        if (playerTarget == null)
        {
            TransitionToState(returnState);
        }
        waitingForReturn = false;
    }
    public void ApplyAttackDamage()
    {
        float damage = npcData.baseDamage * npcData.comboMultiplier;
        float radius = npcData.hitRadius;
        Vector3 attackCenter = attackHitPoint.position;
        Collider[] hits = Physics.OverlapSphere(attackCenter, radius, npcData.playerLayers);
        foreach (Collider c in hits)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }
    public void AttackHitEvent()
    {
        if (currentState is EnemyAttackState attackState)
        {
            attackState.OnAttackHit(this);
        }
    }
    public void AttackAnimationEndEvent()
    {
        if (currentState is EnemyAttackState attackState)
        {
            Debug.Log("Leaving EnemyAttack State");
            attackState.OnAttackAnimationFinished(this);
        }
        
    }
    public void TestEvent()
    {
        Debug.Log("TestEvent called!");
    }
}

