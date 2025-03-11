using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Reference to an AttackData asset that contains the player's health.
    public AttackData playerAttackData;
    private float currentHealth;

    private Animator animator;

    void Start()
    {
        // Initialize current health from AttackData, fallback to 100 if not assigned.
        currentHealth = (playerAttackData != null) ? playerAttackData.health : 100f;
        animator = GetComponent<Animator>();

        Debug.Log("Player Health Initialized: " + currentHealth);
    }

    // Call this method when the player takes damage.
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Player takes " + damage + " damage. Current health: " + currentHealth);
        
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
        yield return new WaitForSeconds(1.2f);
        gameObject.SetActive(false);
    }
}

