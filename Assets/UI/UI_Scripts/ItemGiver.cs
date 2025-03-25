using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    //public List<ItemQuantity> itemsToGive;
    public List<Item> itemsToGive;

    public Inventory playerBag;
    UI_Manager uiManager;

    public event Action<Item> ItemAdded;

    private void Start()
    {
        playerBag = InventoryManager.instance.myBag;
        uiManager = UI_Manager.instance;
        if (uiManager != null)
        {
            uiManager.RegisterItemGiver(this);
        }
    }

    private void OnDestroy()
    {
        if (uiManager != null)
        {
            uiManager.UnregisterItemGiver(this);
        }
    }


    //public void AddNewItems()
    //{
    //    foreach (ItemQuantity itemQuantity in itemsToGive) 
    //    {
    //        Item item = itemQuantity.item;
    //        int quantity = itemQuantity.quantity;

    //        if (!playerBag.itemList.Contains(item))
    //        {
    //            playerBag.itemList[playerBag.FindEmpty()] = item;
    //            item.itemHeld = quantity;  
    //        }
    //        else
    //        {
    //            item.itemHeld += quantity;
    //        }

    //        InventoryManager.RefreshItems(); 
    //        ItemAdded?.Invoke(item);        
    //    }
    //}

    public void AddNewItems()
    {
        foreach (Item item in itemsToGive)
        {
            if (!playerBag.itemList.Contains(item))
            {
                playerBag.itemList[playerBag.FindEmpty()] = item;
            }
            else
            {
                item.itemHeld += 1;
            }
            InventoryManager.RefreshItems();
            ItemAdded?.Invoke(item);
            Debug.Log("Item added: " + item.itemName);
        }
    }

}
