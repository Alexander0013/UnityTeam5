using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/New Equipment")]
public class Equipment : Item
{
    public int damageModifier;
    public int healthModifier;
    public EquipmentType type;
    public Gender gender;
    public override void Use()
    {
        base.Use();
        InventoryManager.instance.Equip(this);
    }
}

public enum EquipmentType { Weapon, Ring }
public enum Gender {Female, Male }



