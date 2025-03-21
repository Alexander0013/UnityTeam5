using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ElementalStatus : MonoBehaviour
{
    // Current elemental effect on this character.
    public ElementType currentElement = ElementType.None;
    // Duration that the current effect will last.
    public float effectDuration = 15f;

    //CrystallizeShard prefab for when a reaction occurs.
    public GameObject crystallizeShardPrefab;

    // Define an event that is fired when a reaction occurs.
    // The event passes the element that reacted and the position.
    public event Action<ElementType, Vector3> OnCrystallizeReaction;

    /// <summary>
    /// Apply an elemental effect to the character.
    /// </summary>
    /// <param name="newElement">The element to apply (e.g., Geo, Electro, etc.).</param>
    /// <param name="duration">How long the effect lasts.</param>
    public void ApplyElement(ElementType newElement, float duration)
    {
        // If no element is currently applied, simply set the new effect.
        if (currentElement == ElementType.None)
        {
            currentElement = newElement;
            effectDuration = duration;
            //Debug.Log($"{gameObject.name} is now affected by {newElement} for {duration} seconds.");
        }
        else
        {
            // If an effect already exists, check for a reaction.
            // For Crystallize: when a Geo attack is applied to a target already affected by another element.
            if (newElement == ElementType.Geo && currentElement != ElementType.Geo)
            {
                // Trigger Crystallize reaction.
                TriggerCrystallizeReaction(currentElement);
                // Clear the current elemental effect after reaction.
                ClearElement();
            }
            else
            {
                // Otherwise, refresh the duration of the existing effect (or optionally combine effects).
                effectDuration = duration;
                //Debug.Log($"{gameObject.name} refreshes its {currentElement} effect for {duration} seconds.");
            }
        }
    }

    /// <summary>
    /// Triggers the Crystallize reaction by spawning an elemental shard.
    /// </summary>
    /// <param name="reactant">The element that was already present on the character.</param>
    private void TriggerCrystallizeReaction(ElementType reactant)
    {
        //Debug.Log($"Crystallize reaction triggered! {reactant} reacts with Geo.");
        // Spawn a corresponding shard if a prefab is assigned.
        if (crystallizeShardPrefab != null)
        {
            // Generate a random horizontal direction (ignore y)
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
            // Normalize to ensure it's exactly 1 unit away
            randomDirection = randomDirection.normalized;
            // Compute the spawn position 1 unit away from the enemy's position.
            Vector3 spawnPosition = transform.position + randomDirection;
            Instantiate(crystallizeShardPrefab, spawnPosition, Quaternion.identity);
        }
        // Fire the event to notify interested systems.
        OnCrystallizeReaction?.Invoke(reactant, transform.position);
        ClearElement();
    }

    /// <summary>
    /// Clears the current elemental effect.
    /// </summary>
    public void ClearElement()
    {
        //Debug.Log($"{gameObject.name} clears its {currentElement} effect.");
        currentElement = ElementType.None;
        effectDuration = 0f;
    }

    private void Update()
    {
        // If an elemental effect is active, decrease its timer.
        if (currentElement != ElementType.None)
        {
            effectDuration -= Time.deltaTime;
            if (effectDuration <= 0f)
            {
                ClearElement();
            }
        }
    }
}
