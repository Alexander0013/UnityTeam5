using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentList", menuName = "Inventory/EquipmentList")]
public class EquipmentList : ScriptableObject
{
    public List<Equipment> equippedItems = new List<Equipment>();

    public void EquipItem(int typeIndex, Equipment newItem)
    {
        //while (equippedItems.Count <= slotIndex)
        //{
        //    equippedItems.Add(null);
        //}
        
        equippedItems[typeIndex] = newItem;
    }

    public void UnEquipItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedItems.Count)
        {
            equippedItems[slotIndex] = null;
        }
    }

    public Equipment GetEquippedItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedItems.Count)
        {
            return equippedItems[slotIndex];
        }
        return null;
    }
}
