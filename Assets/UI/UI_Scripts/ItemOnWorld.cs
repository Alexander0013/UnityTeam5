using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item thisItem;
    public Inventory myBag;  //玩家背包資料庫

   
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("OnTriggerEnter");
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("touched Player");
            AddNewItem();
            Destroy(gameObject);
        }
    }
    public void AddNewItem()
    {
        if (!myBag.itemList.Contains(thisItem))
        {
            //新增到List
            //playerInventory.itemList.Add(thisItem);
            //同步到背包介面
            //InventoryManager.CreateNewItem(thisItem);

            //找空位->置換空位
            for (int i = 0; i < myBag.itemList.Count; i++)
            {
                if (myBag.itemList[i] == null)
                {
                    myBag.itemList[i] = thisItem;
                    break;
                }
            }
        }
        else
        {
            thisItem.itemHeld += 1;           
        }
        InventoryManager.RefreshItems(); //更新背包介面
    }

}
