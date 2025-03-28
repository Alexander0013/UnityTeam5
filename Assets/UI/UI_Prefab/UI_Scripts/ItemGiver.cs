using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
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
            ItemAdded?.Invoke(item);
            Debug.Log("Item added: " + item.itemName);
        }
        InventoryManager.RefreshItems();
    }

}
