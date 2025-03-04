using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    [SerializeField]
    private Slot targetItem;
    public Inventory myBag;
    public Button useButton;  // 使用按鈕
    public Button dropButton; // 丟棄按鈕

   
    public void SetTargetItem(Slot slot)
    {
        targetItem = slot;
    }

    public void OnUseButtonClick()
    {
        //Debug.Log("使用 " + targetItem.name);
        UseItem(targetItem);
        Destroy(gameObject); // 關閉選單
    }

    public void OnDropButtonClick()
    {
        //Debug.Log("丟棄 " + targetItem.name);
        DropItem(targetItem);
        Destroy(gameObject); // 關閉選單
    }

    public void UseItem(Slot slot)
    {
        slot.slotItem.itemHeld -= 1;
        if (slot.slotItem.itemHeld == 0)
        {
            myBag.itemList[slot.slotID] = null;
        }
        //未來要加入使用物品的功能
        InventoryManager.RefreshItems();
    }

    public void DropItem(Slot targetItem)
    {        
        targetItem.slotItem.itemHeld = 0;
        myBag.itemList[targetItem.slotID] = null;
        InventoryManager.RefreshItems();
    }

   
}
