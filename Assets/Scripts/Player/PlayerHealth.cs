using System.Collections;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    // Reference to an AttackData asset that contains the player's health.
    public AttackData playerAttackData;
    private float currentHealth;
    private float maxHealth; 

    private Animator animator;
    private PlayerShield playerShield; // Reference to the shield component.
    public event System.Action OnHealthChanged; 
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                OnHealthChanged?.Invoke();
            }
        }
    }

    void Start()
    {
        // Initialize current health from AttackData, fallback to 100 if not assigned.
        currentHealth = (playerAttackData != null) ? playerAttackData.health : 100f;
        animator = GetComponent<Animator>();
        playerShield = GetComponent<PlayerShield>(); // Cache the shield component.
        Debug.Log("Player Health Initialized: " + currentHealth);

        OnHealthChanged?.Invoke();
    }

    /// <summary>
    /// Applies damage to the player, triggers hit reaction or death.
    /// </summary>
    /// <param name="damage">The damage to apply.</param>
    public void TakeDamage(float damage)
    {
        // If a shield is active, let it absorb damage first.
        if (playerShield != null && playerShield.IsShieldActive)
        {
            damage = playerShield.AbsorbDamage(damage);
        }

        if (damage > 0)
        {
            CurrentHealth -= damage;
            Debug.Log("Player takes " + damage + " damage. Current health: " + CurrentHealth);
            if (currentHealth > 0)
            {
                // Trigger hit animation.
                if (animator != null)
                {
                    animator.SetBool("getHit", true);
                    StartCoroutine(ResetGetHit());
                }
            }
            else
            {
                // Health <= 0: trigger die animation.
                if (animator != null)
                {
                    animator.SetTrigger("Die");
                }
                StartCoroutine(DieAndDisable());
            }
        }
        
    }

    // Resets the getHit flag after a short duration.
    IEnumerator ResetGetHit()
    {
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
            animator.SetBool("getHit", false);
    }

    // Wait for die animation to finish then disable the player.
    IEnumerator DieAndDisable()
    {
        // Adjust the wait time to match your die animation length.
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
    // Optional: a method to reset health, called by a GameManager upon respawn.
    public void ResetHealth()
    {
        CurrentHealth = (playerAttackData != null) ? playerAttackData.health : 100f;
    }
}
