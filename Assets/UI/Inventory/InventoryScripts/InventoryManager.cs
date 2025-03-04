using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

    Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newItem,Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

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

    private void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            UnEquipAll();
        }
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.type;
        
        Equipment oldItem = UnEquip(slotIndex);
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }
        currentEquipment[slotIndex] = newItem;
        RefreshItems();
    }

    public Equipment UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            int empty = myBag.FindEmpty();
            if (empty != -1)
            {
                myBag.itemList[empty] = oldItem;
            }
            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            return oldItem;
        }
        return null;
    }

    public void UnEquipAll()
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            UnEquip(i);
        }
    }

    public static void UpdateItemInfo(Image itemImage,string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
        instance.itemImage.sprite = itemImage.sprite;
    }
    
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
