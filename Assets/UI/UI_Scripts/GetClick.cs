using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GetClick : MonoBehaviour, IPointerClickHandler
{   
    public GameObject menuPrefab; // 右鍵選單的 Prefab
    private static GameObject spawnedMenu; // 產生的選單

    //Alex
    private float lastClickTime; 
    private float doubleClickThreshold = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
        if (spawnedMenu != null)
        {
            Destroy(spawnedMenu); 
            spawnedMenu = null;
        }

        //if (eventData.pointerCurrentRaycast.gameObject.name == "Item Image")
        //{

        //    Slot slot = this.gameObject.GetComponentInParent<Slot>();
        //    Debug.Log(slot);
        //    if (slot != null)
        //    {
        //        if (eventData.button == PointerEventData.InputButton.Right) // 右鍵
        //        {
        //            ShowMenu(slot, eventData.position);
        //        }
        //        InventoryManager.UpdateItemInfo(slot.slotImage, slot.slotInfo);
        //    }
        //}

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // If the time between clicks is less than the threshold, it's a double click.
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                Slot slot = this.gameObject.GetComponentInParent<Slot>();
                if (slot != null)
                {
                    ShowMenu(slot, eventData.position);
                }
            }
            lastClickTime = Time.time;

            // Optionally, update item info on a single click:
            Slot singleClickSlot = this.gameObject.GetComponentInParent<Slot>();
            if (singleClickSlot != null)
            {
                InventoryManager.UpdateItemInfo(singleClickSlot.slotImage, singleClickSlot.slotInfo);
            }
        }

    }

    public void ShowMenu(Slot slot, Vector2 position)
    {
        GameObject bag = UI_Manager.instance.myBag;
        
        // 產生選單
        if(slot.slotItem.itemType!= Item.ItemType.Other)
        {
            spawnedMenu = Instantiate(menuPrefab, UI_Manager.instance.canvas.transform);
            spawnedMenu.transform.position = position;

            // 設定目前的 Menu
            bag.GetComponentInParent<UI_Manager>().SetMenu(spawnedMenu);

            // 讓選單知道是哪個物品被點擊
            spawnedMenu.GetComponent<RightClickMenu>().SetTargetItem(slot);
            //不同類型的物件顯示不同字樣Equip/Use
            RightClickMenu menuScript = spawnedMenu.GetComponent<RightClickMenu>();
            if (menuScript != null)
            {
                menuScript.SetTargetItem(slot); // 設定當前物品
                if (slot.slotItem is Equipment)
                {
                    menuScript.SetUseButtonText("Equip");
                }
                else
                {
                    menuScript.SetUseButtonText("Use");
                }
            }
        }
       
    }
}
