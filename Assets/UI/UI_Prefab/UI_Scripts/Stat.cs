using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "NewStat", menuName = "Inventory/Stat")]
public class Stat : ScriptableObject
{
   
    [SerializeField] private AttackData baseData;  // 直接引用 AttackData
    public SerializableDictionary<int, float> damageModifiers = new SerializableDictionary<int, float>();
    public SerializableDictionary<int, float> healthModifiers = new SerializableDictionary<int, float>();



    public float GetBaseValue(StatType type)
    {
        if (baseData == null) return 0;

        return type == StatType.Damage ? baseData.baseDamage : baseData.health;
    }

    public float GetValue(StatType type)
    {
        return GetBaseValue(type) + (type == StatType.Damage ? damageModifiers.Values.Sum() : healthModifiers.Values.Sum());
    }
    public void AddModifier(StatType type, int index, float modifier)
    {
        if (modifier == 0) return;

        var targetModifiers = type == StatType.Damage ? damageModifiers : healthModifiers;
        if (targetModifiers.ContainsKey(index))
            targetModifiers[index] += modifier;
        else
            targetModifiers.Add(index, modifier);

    }

    public void RemoveModifier(StatType type, int index, float modifier)
    {
        if (modifier == 0) return;

        var targetModifiers = type == StatType.Damage ? damageModifiers : healthModifiers;
        if (!targetModifiers.ContainsKey(index)) return;

        targetModifiers[index] -= modifier;
        if (targetModifiers[index] == 0)
            targetModifiers.Remove(index);
    }


}

public enum StatType
{
    Damage,
    Health
}