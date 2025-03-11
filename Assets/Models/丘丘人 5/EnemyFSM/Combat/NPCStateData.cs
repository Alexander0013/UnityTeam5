using UnityEngine;

[CreateAssetMenu(fileName = "NPCStateData", menuName = "Combat/NPCStateData", order = 1)]
public class NPCStateData : ScriptableObject
{
    // Attack properties similar to AttackData
    public float baseDamage = 10f;
    public float comboMultiplier = 1.0f;
    public float hitRadius = 1.5f;
    
    // Health for the enemy
    public float maxHealth = 100f;
    public LayerMask playerLayers; // set to player layer

    // the elemental state currently affecting this NPC.
    // Initially, it can be set to None. Later, when an attack is applied, you can update this.
    public ElementType currentElement = ElementType.None;
}
