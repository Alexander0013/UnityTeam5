using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Inventory/New Item")]

public class Item :ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemHeld;
    [TextArea]
    public string itemInfo;
    public ItemType itemType;

    public void OnUse()
    {
        switch (itemType)
        {
            case ItemType.Equipment:
                Debug.Log("裝備");
                break;
            case ItemType.Prop:
                Debug.Log("道具");
                break;
            default:
                break;
        }
    }

}
public enum ItemType
{   
    Equipment,
    Prop
}