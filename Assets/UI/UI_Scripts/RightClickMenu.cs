using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    [SerializeField]
    private Slot targetItem;
    public Inventory myBag;
    public Button useButton;  // 使用按鈕
    public Button dropButton; // 丟棄按鈕
    public TextMeshProUGUI useButtonText;
    public Item item;



    public void SetTargetItem(Slot Item)
    {
        targetItem = Item;
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
        if (targetItem.slotItem is Equipment equipment)  // 檢查 targetItem 是否是 Equipment 類型
        {
            equipment.Use();  // 呼叫 Equipment 的 Use 方法            
            myBag.itemList[slot.slotID] = null;
        }
        else
        {          
            slot.slotItem.Use();
            slot.slotItem.itemHeld -= 1;
            if (slot.slotItem.itemHeld == 0)
            {
                myBag.itemList[slot.slotID] = null;
            }
        }
        InventoryManager.RefreshItems();
    }

    public void DropItem(Slot targetItem)
    {        
        targetItem.slotItem.itemHeld = 0;
        myBag.itemList[targetItem.slotID] = null;
        InventoryManager.RefreshItems();
    }

    public void SetUseButtonText(string text)
    {
        if (useButtonText != null)
        {
            useButtonText.text = text;
        }
    }
}
