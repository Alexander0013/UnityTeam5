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
    
    
    // For enemies, you'll likely want to detect the player when attacking.
    // You can set this to the layer(s) your player(s) are on.
    public LayerMask playerLayers;
}
