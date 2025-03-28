using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentList", menuName = "Inventory/EquipmentList")]
public class EquipmentList : ScriptableObject
{
    public List<Equipment> equippedItems = new List<Equipment>();

    public List<Stat> equipmentStats = new List<Stat>();


    public void EquipItem(int typeIndex, Equipment newItem)
    {
        //while (equippedItems.Count <= typeIndex)
        //{
        //    equippedItems.Add(null);
        //}

        equippedItems[typeIndex] = newItem;
    }

    public void EquipItem(int typeIndex, Equipment newItem, Stat stat)
    {
        while (equippedItems.Count <= typeIndex)
        {
            equippedItems.Add(null);
            equipmentStats.Add(null);
        }

        equippedItems[typeIndex] = newItem;
        equipmentStats[typeIndex] = stat;
    }

    public void UnEquipItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedItems.Count)
        {
            equippedItems[slotIndex] = null;
            //equipmentStats[slotIndex] = null;
        }
        else
        {
            Debug.LogWarning($"UnEquipItem: slotIndex {slotIndex} ¶W¥X½d³ò (List ªø«×: {equippedItems.Count})");
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
