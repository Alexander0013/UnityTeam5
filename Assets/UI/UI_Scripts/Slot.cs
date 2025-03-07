using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;

public class Slot : MonoBehaviour
{
    public int slotID;//背包格子編號
    public Item slotItem;
    public Image slotImage;
    public TextMeshProUGUI slotText;
    public string slotInfo;

    public GameObject itemInSlot;


    ////點擊顯示物品描述
    //public void ItemOnClick()
    //{
    //    Debug.Log("ItemOnClick");
    //    InventoryManager.UpdateItemInfo(slotImage,slotInfo);        
    //}

    public void SetUpSlot(Item item)
    {
        if (item == null)
        {
            itemInSlot.SetActive(false);
            return;
        }
        slotImage.sprite = item.itemImage;
        if (item.itemType!= ItemType.Equipment)
        { 
            slotText.text = item.itemHeld.ToString();
        }
        slotInfo = item.itemInfo;
        slotItem = item;
    }
}
