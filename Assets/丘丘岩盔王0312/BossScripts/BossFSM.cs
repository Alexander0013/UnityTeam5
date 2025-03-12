using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossFSM : MonoBehaviour
{
    // References to each state
    public BossIdleState idleState = new BossIdleState();
    public BossChaseState chaseState = new BossChaseState();
    public BossAttackState attackState = new BossAttackState();
    public BossReturnState returnState = new BossReturnState();
    public BossDeadState deadState = new BossDeadState();

    [Header("Boss Settings")]
    public NPCStateData npcData;
    public float detectionRadius = 6f;  
    public float attackRadius = 2.5f;     
    public Transform treasureTransform; 
    public float treasureReturnRadius = 1f; 
    public Transform attackHitPoint;
    

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public bool isDead;
    [HideInInspector] public BossBaseState currentState;

    private static List<BossFSM> allBoss = new List<BossFSM>(); // For team collision avoidance
    private bool waitingForReturn = false;

    private void Awake()
    {
        allBoss.Add(this);

        animator = GetComponent<Animator>();
        if (npcData != null)
        {
            // You mentioned currentHealth in code, but you're using BossHealth for actual HP in other examples
            // If you're not using it anymore, you can remove this if/else entirely
        }
        else
        {
            // fallback
        }
    }

    private void Start()
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

        // Let our current state perform its update logic.
        currentState.UpdateState(this);
        PreventTeamCollision();
    }

    public void TransitionToState(BossBaseState newState)
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
        allBoss.Remove(this);
    }

    private void PreventTeamCollision()
    {
        // your existing separation logic
        foreach (BossFSM otherBoss in allBoss)
        {
            if (otherBoss == this) continue;
            if (otherBoss == null) continue;

            float dist = Vector3.Distance(transform.position, otherBoss.transform.position);
            if (dist < 1f && dist > 0f)  // you can use separationRadius
            {
                Vector3 pushDir = (transform.position - otherBoss.transform.position);
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
    private Transform FindActiveLivingPlayer()
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
}
