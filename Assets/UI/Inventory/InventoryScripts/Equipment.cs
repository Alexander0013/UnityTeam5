using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/New Equipment")]
public class Equipment : Item
{
    public int damageModifier;
    public int armorModifier;
    public EquipmentType type; 
    public override void Use()
    {
        base.Use();
        InventoryManager.instance.Equip(this);
        // 裝備到角色身上
    }
}

public enum EquipmentType { Weapon, Ring }



