using UnityEngine;
using UnityEngine.EventSystems;

public class GetClick : MonoBehaviour, IPointerClickHandler
{   
    public GameObject menuPrefab; // 右鍵選單的 Prefab
    private GameObject spawnedMenu; // 產生的選單

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(spawnedMenu); // 刪除舊選單
        spawnedMenu = null;
        Slot slot = this.gameObject.GetComponentInParent<Slot>();
        if (eventData.button == PointerEventData.InputButton.Right) // 右鍵
        {
            ShowMenu(slot, eventData.position);
            //Debug.Log(this.gameObject.GetComponent<Slot>());
        }
        else if (eventData.button == PointerEventData.InputButton.Left) // 左鍵
        {
            //Debug.Log("左鍵點擊 " );
            InventoryManager.UpdateItemInfo(slot.slotImage, slot.slotInfo);
            
        }
    }

    public void ShowMenu(Slot slot, Vector2 position)
    {
        //if (spawnedMenu != null)
        //{
        //    Destroy(spawnedMenu); // 刪除舊選單
        //    spawnedMenu = null;
        //}

        // 產生選單
        spawnedMenu = Instantiate(menuPrefab, FindObjectOfType<Canvas>().transform);
        spawnedMenu.transform.position = position;

        // 讓選單知道是哪個物品被點擊
        spawnedMenu.GetComponent<RightClickMenu>().SetTargetItem(slot);
    }
}
