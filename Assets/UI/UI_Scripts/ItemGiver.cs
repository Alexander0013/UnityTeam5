using System;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    public Item itemToGive;
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
        uiManager = UI_Manager.instance;
        if (uiManager != null)
        {
            uiManager.UnregisterItemGiver(this);
        }
    }
    public void AddNewItem()
    {
        if (!playerBag.itemList.Contains(itemToGive))
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i] == null)
                {
                    playerBag.itemList[i] = itemToGive;
                    break;
                }
            }
        }
        else
        {
            itemToGive.itemHeld += 1;
        }
        InventoryManager.RefreshItems(); 
        ItemAdded?.Invoke(itemToGive);
    }
}
