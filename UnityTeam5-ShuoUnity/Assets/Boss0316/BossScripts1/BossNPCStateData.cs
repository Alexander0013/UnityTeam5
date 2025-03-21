using UnityEngine;

[CreateAssetMenu(fileName = "BossNPCStateData", menuName = "BossScripts1/BossNPCStateData", order = 1)]
public class BossNPCStateData : ScriptableObject
{
    // Attack properties similar to AttackData
    public float baseDamage = 10f;
    public float comboMultiplier = 1.0f;
    public float hitRadius = 2f;
    public float chargeDamage = 50f; // 新增的 chargeDamage 欄位，用於 Charge 攻擊傷害
    // Health for the enemy
    public float maxHealth = 100f;
    public LayerMask playerLayers; // set to player layer

    // the elemental state currently affecting this NPC.
    // Initially, it can be set to None. Later, when an attack is applied, you can update this.
    public ElementType currentElement = ElementType.None;
}
