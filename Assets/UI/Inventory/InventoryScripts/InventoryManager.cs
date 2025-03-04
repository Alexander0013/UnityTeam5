using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;

    public Inventory myBag;
    public GameObject slotGrid;
    //public Slot slotPrefab;
    public GameObject emptySlot;
    public TextMeshProUGUI itemInfo;
    public Image itemImage;
    //儲存生成過的slots
    public List<GameObject> slots = new List<GameObject>();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    void OnEnable()
    {
        RefreshItems();
        instance.itemInfo.text = "";
    }

    public static void UpdateItemInfo(Image itemImage,string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
        instance.itemImage.sprite = itemImage.sprite;
    }

    ////myBag List的資訊同步到Grid(背包介面)
    //public static void CreateNewItem(Item item)
    //{
    //    Debug.Log("AddNewItem");
    //    Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
    //    newItem.gameObject.transform.SetParent(instance.slotGrid.transform);
    //    //新建物品的資訊
    //    newItem.slotItem = item;
    //    newItem.slotImage.sprite = item.itemImage;
    //    newItem.slotText.text = item.itemHeld.ToString();
    //}

    public static void RefreshItems() //銷毀背包物件->重新生成物件(數量被更新)
    {
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)        
        {
            if (instance.slotGrid.transform.childCount == 0)break;
            
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }
        
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //CreateNewItem(instance.myBag.itemList[i]);            
            instance.slots.Add(Instantiate(instance.emptySlot));
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<Slot>().slotID = i;
            instance.slots[i].GetComponent<Slot>().SetUpSlot(instance.myBag.itemList[i]);            
        }        
    }

   
}
