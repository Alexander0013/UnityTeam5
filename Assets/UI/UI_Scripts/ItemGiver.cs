using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    public Item itemToGive;
    public Inventory playerBag;

    public event Action<Item> ItemAdded;

    private void Start()
    {
        playerBag = InventoryManager.instance.myBag;
        // 註冊UIManager來監聽這個物品
        UI_Manager uiManager = FindObjectOfType<UI_Manager>();
        if (uiManager != null)
        {
            uiManager.RegisterItemGiver(this);
        }
    }

    private void OnDestroy()
    {
        // 取消註冊
        UI_Manager uiManager = FindObjectOfType<UI_Manager>();
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
        InventoryManager.RefreshItems(); //更新背包介面
        ItemAdded?.Invoke(itemToGive);
    }
}
