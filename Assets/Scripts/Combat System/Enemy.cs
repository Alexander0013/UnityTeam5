using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    
    public void TakeDamage(float amount)
    {
        health -= amount;

        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
