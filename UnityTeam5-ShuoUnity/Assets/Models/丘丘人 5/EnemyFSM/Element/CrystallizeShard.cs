using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystallizeShard : MonoBehaviour
{
    // How long the shield should last on the player.
    public float shieldDuration = 15f;
    
    // When the player collides with the shard, activate the shield.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the player.
        if (other.CompareTag("Player"))
        {
            // Try to get the player's shield component.
            PlayerShield shield = other.GetComponent<PlayerShield>();
            if (shield != null)
            {
                shield.ActivateShield(shieldDuration);
                Debug.Log($"Player picked up a Crystallize shard. Shield activated for {shieldDuration} seconds.");
            }
            else
            {
                Debug.LogWarning("PlayerShield component not found on player.");
            }
            // Destroy the shard after pickup.
            Destroy(gameObject);
        }
    }
}
