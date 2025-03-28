using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Inventory/New Item")]

public class Item :ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemHeld;
    [TextArea(3, 10)]
    public string itemInfo;
    public ItemType itemType;
    
    public enum ItemType
    {
        Potion,
        Equipment,
        Other
    }

    public virtual void Use()
    {        
        Debug.Log("Use :" + itemName);
    }

}
