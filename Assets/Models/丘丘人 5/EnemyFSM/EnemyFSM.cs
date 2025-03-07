using UnityEngine;
using System.Collections.Generic;

public class EnemyFSM : MonoBehaviour
{
    // References to each state
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyChaseState chaseState = new EnemyChaseState();
    public EnemyAttackState attackState = new EnemyAttackState();
    public EnemyReturnState returnState = new EnemyReturnState();
    public EnemyDeadState deadState = new EnemyDeadState();

    [Header("Enemy Settings")]
    public NPCStateData npcData;
    public float detectionRadius = 7f;  // detectDistance
    public float attackRadius = 2f;     // hitRadius
    public Transform treasureTransform; // The position to return to if player is dead
    public float treasureReturnRadius = 1f; // They gather within 1 unit of treasure
    public float separationForce = 2f;  // how strongly they push away from each other
    public float separationRadius = 1f; // if enemies are within 1 unit, they separate

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public bool isDead;
    [HideInInspector] public EnemyBaseState currentState;

    private static List<EnemyFSM> allEnemies = new List<EnemyFSM>(); // For team collision avoidance

    private void Awake()
    {
        // Register in a static list so each enemy can check others for separation
        allEnemies.Add(this);

        animator = GetComponent<Animator>();
        if (npcData != null)
        {
            currentHealth = npcData.maxHealth;
        }
        else
        {
            currentHealth = 100f;
        }
    }

    private void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        
        // Start in Idle
        TransitionToState(idleState);
    }

    private void Update()
    {
        if (!isDead && currentState != null)
        {
            currentState.UpdateState(this);
        }

        // Always prevent collisions among enemies
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
        // Remove from static list on destroy
        allEnemies.Remove(this);
    }

    /// <summary>
    /// Simple method to keep enemies from overlapping each other, applying a separation force.
    /// This is a naive approach, but works similarly to how Genshin Impact enemies spread out in a group.
    /// </summary>
    private void PreventTeamCollision()
    {
        foreach (EnemyFSM otherEnemy in allEnemies)
        {
            if (otherEnemy == this) continue; // skip self
            if (otherEnemy == null) continue;

            float dist = Vector3.Distance(transform.position, otherEnemy.transform.position);
            if (dist < separationRadius && dist > 0f)
            {
                Vector3 pushDir = (transform.position - otherEnemy.transform.position);
                pushDir.y = 0f;                      // Zero out vertical
                pushDir = pushDir.normalized;        // Re-normalize after you remove y
                transform.position += pushDir * (separationForce * Time.deltaTime);

            }
        }
    }
}
