using System.Collections;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float lastGetHitSoundTime = -100f; // Initialize to a very negative value.
    public float getHitSoundCooldown = 5.0f; // Set your cooldown duration (in seconds).
    // Reference to an AttackData asset that contains the player's health.
    public AttackData playerAttackData;
    private float currentHealth;
    private float maxHealth; 

    private Animator animator;
    private PlayerShield playerShield; // Reference to the shield component.
    public event System.Action<float> OnHealthChanged; 
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                OnHealthChanged?.Invoke(currentHealth);
            }
        }
    }
    
    void OnEnable() 
    {
        InventoryManager.ItemUsed += OnItemUsed;
        InventoryManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }

    void OnDisable() 
    {
        InventoryManager.ItemUsed -= OnItemUsed;
        InventoryManager.instance.onEquipmentChanged -= OnEquipmentChanged;
    }

    private void OnItemUsed(Item item) {
    HealthPotion potion = item as HealthPotion;
    if (potion != null) {
        if (CurrentHealth + potion.healAmount > maxHealth) {
            CurrentHealth = maxHealth;
        } else {
            CurrentHealth += potion.healAmount;
        }
        Debug.Log("Healed for " + potion.healAmount + ", new health: " + CurrentHealth);
    }
}

    private void OnEquipmentChanged(Equipment newItem, Equipment oldItem, int genderIndex) 
    {
        /* If the equipment changes include a health modifier, update health.
        float healthModifier = 0;
        if(oldItem != null)
            healthModifier -= oldItem.healthModifier;
        if(newItem != null)
            healthModifier += newItem.healthModifier;

        // Update the current and maximum health.
        UpdateHealthWithModifier(healthModifier);
        */
    }

    private void UpdateHealthWithModifier(float modifier) 
    {
        // Simply add the modifier to both maxHealth and currentHealth.
        maxHealth += modifier;
        CurrentHealth += modifier;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    

    void Start()
    {
        float baseHealth = (playerAttackData != null ? playerAttackData.health : 100f);
        maxHealth = baseHealth;
        currentHealth = baseHealth;
        animator = GetComponent<Animator>();
        playerShield = GetComponent<PlayerShield>(); // Cache the shield component.
        Debug.Log("Player Health Initialized: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth);
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
                if (Time.time - lastGetHitSoundTime >= getHitSoundCooldown)
                {
                    GetComponent<PlayerAudio>()?.PlayGetHitSound();
                    lastGetHitSoundTime = Time.time;
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
        yield return new WaitForSeconds(0.1f);
        //if (animator != null)
        //    animator.SetBool("getHit", false);
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
