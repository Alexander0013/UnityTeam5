using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Inventory myBag;
    public int currentItemID;    

    public void OnBeginDrag(PointerEventData eventData) //eventData:滑鼠拖曳事件資訊
    {
        GameObject usingMenu = GameObject.Find("Bag Menu(Clone)");
        if (usingMenu != null)
        {
            Destroy(usingMenu);
        }
        originalParent = transform.parent;//記錄原本的父物件
        currentItemID = originalParent.GetComponent<Slot>().slotID;
        transform.position = eventData.position;
        transform.SetParent(transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log(target);
        if (target != null)
        {
            //有拖曳到Item Image上
            if (target.name == "Item Image" || target.name == "Text")
            {
                //置換位置跟parent到目標
                transform.SetParent(target.transform.parent.parent);
                transform.position = target.transform.parent.parent.position;
                var temp = myBag.itemList[currentItemID];   //目前拖曳的物件ID
                //置換成對方物件ID
                myBag.itemList[currentItemID] = myBag.itemList[target.GetComponentInParent<Slot>().slotID];
                myBag.itemList[target.GetComponentInParent<Slot>().slotID] = temp;

                target.transform.parent.position = originalParent.position;
                target.transform.parent.SetParent(originalParent);
            }
            //拖曳到空格上
            else if (target.name == "Slot(Clone)")
            {
                transform.SetParent(target.transform);
                transform.position = target.transform.position;

                myBag.itemList[target.GetComponentInParent<Slot>().slotID] = myBag.itemList[currentItemID];
                if (target.GetComponent<Slot>().slotID != currentItemID)
                {
                    myBag.itemList[currentItemID] = null;//原本的位置清空
                }
            }
            //Other
            else
            {
                transform.SetParent(originalParent);
                transform.position = originalParent.position;
            }
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            InventoryManager.RefreshItems();
        }
    }
}

