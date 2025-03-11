using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    // Shield properties (adjustable via the Inspector)
    public float maxShieldHP = 10f;
    private float currentShieldHP = 0f;
    public ElementType shieldElement; // For later use if needed

    // Visual representation of the shield.
    public GameObject shieldVisualPrefab;
    private GameObject activeShieldVisual;

    // Cached reference for the shield duration coroutine.
    private Coroutine shieldDurationCoroutine;

    // Indicates whether the shield is currently active.
    public bool IsShieldActive { get { return currentShieldHP > 0; } }

    /// <summary>
    /// Activates the shield with full HP and starts the duration timer.
    /// </summary>
    /// <param name="duration">How long the shield should last.</param>
    public void ActivateShield(float duration)
    {
        currentShieldHP = maxShieldHP;
        if (activeShieldVisual == null && shieldVisualPrefab != null)
        {
            activeShieldVisual = Instantiate(shieldVisualPrefab, transform);
        }
        //Debug.Log("Shield activated with HP: " + currentShieldHP + " for " + duration + " seconds.");

        // Cancel any existing duration coroutine to avoid duplicate timers.
        if (shieldDurationCoroutine != null)
        {
            StopCoroutine(shieldDurationCoroutine);
        }
        shieldDurationCoroutine = StartCoroutine(ShieldDuration(duration));
    }

    /// <summary>
    /// Absorbs incoming damage. If shield HP falls below zero, deactivates the shield and returns overflow damage.
    /// </summary>
    public float AbsorbDamage(float damage)
    {
        if (!IsShieldActive)
            return damage;

        currentShieldHP -= damage;
        //Debug.Log("Shield absorbs " + damage + " damage. Remaining Shield HP: " + currentShieldHP);

        if (currentShieldHP <= 0)
        {
            float overflow = -currentShieldHP;
            currentShieldHP = 0;
            DeactivateShield();
            return overflow;
        }
        return 0f;
    }

    /// <summary>
    /// Deactivates the shield and its visual effect.
    /// </summary>
    public void DeactivateShield()
    {
        //Debug.Log("Shield is deactivated.");
        if (activeShieldVisual != null)
        {
            Destroy(activeShieldVisual);
            activeShieldVisual = null;
        }
        // Cancel the duration coroutine if it's still running.
        if (shieldDurationCoroutine != null)
        {
            StopCoroutine(shieldDurationCoroutine);
            shieldDurationCoroutine = null;
        }
    }

    /// <summary>
    /// Runs for the specified duration and then deactivates the shield.
    /// </summary>
    private IEnumerator ShieldDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        // When time is up, if the shield is still active, deactivate it.
        if (IsShieldActive)
        {
            DeactivateShield();
        }
        shieldDurationCoroutine = null;
    }
}
