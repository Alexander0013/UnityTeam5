using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]

public class Inventory : ScriptableObject
{
    public List<Item> itemList = new List<Item>();
    

    public int FindEmpty()
    {
        for(int i = 0; i<itemList.Count; i++)
        {
            if(itemList[i] == null)
                return i;
        }
        return -1;
    }
}

   
