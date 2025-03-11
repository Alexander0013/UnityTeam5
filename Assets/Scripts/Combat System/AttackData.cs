using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData", order = 1)]
public class AttackData : ScriptableObject
{
    public float baseDamage = 10f;
    public float comboMultiplier = 1.0f;
    public float hitRadius = 1.5f;
    public LayerMask enemyLayers; // Set in Inspector to include enemy layers.
    public float health = 100f;
    public int currentWeaponIndex = 0;
     
    public ElementType element = ElementType.None; //the elemental type of this attack.
}
