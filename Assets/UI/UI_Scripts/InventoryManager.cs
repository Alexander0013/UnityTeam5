using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;

    //Backpack
    public Inventory myBag;
    public GameObject slotGrid;   
    public GameObject emptySlot;
    public TextMeshProUGUI itemInfo;
    public Image itemImage;
    //slots
    public List<GameObject> slots = new List<GameObject>();

    //Equipment
    EquipmentList equipmentList;
        
    public EquipmentList equipmentList_A; //save equipment is equipped
    public Image[] equipmentImage_A;
    public GameObject[] equipmentText_A;
    public EquipmentList equipmentList_B;
    public Image[] equipmentImage_B;
    public GameObject[] equipmentText_B;

    public delegate void OnEquipmentChanged(Equipment newItem,Equipment oldItem,int genderIndex);
    public OnEquipmentChanged onEquipmentChanged;

    //Add Item
   

    


    //Alex
    public static event System.Action<Item> ItemUsed;
    public static void RaiseItemUsed(Item item)
    {
        ItemUsed?.Invoke(item);
    }
    //Alex
    void Awake()
    {
        Debug.Log("InventoryManager Awake");
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

    public void Start()
    {
        UpdateEquipmentUI_A();
        UpdateEquipmentUI_B();
        onEquipmentChanged?.Invoke(null, null, 0);
        onEquipmentChanged?.Invoke(null, null, 1);
    }


    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Backspace))
        {
            UnEquipAll();
        }
    }

    public void Equip(Equipment newItem)
    {
        int typeIndex = (int)newItem.type;
        int genderIndex = (int)newItem.gender;
        
        Equipment oldItem = UnEquip(genderIndex,typeIndex);

        equipmentList = GetEquipmentList(genderIndex);

        equipmentList.EquipItem(typeIndex, newItem);    

        UpdateEquipmentUI(genderIndex);

        onEquipmentChanged?.Invoke(newItem, null, genderIndex);
    }

    public Equipment UnEquip(int genderIndex,int slotIndex)
    {
        equipmentList = GetEquipmentList(genderIndex);
        
        if (equipmentList.equippedItems[slotIndex] != null)
        {
            Equipment oldItem = equipmentList.equippedItems[slotIndex];
            int empty = myBag.FindEmpty();
            if (empty != -1)
            {
                myBag.itemList[empty] = oldItem;
            }
            
            onEquipmentChanged?.Invoke(null, oldItem, genderIndex);

            equipmentList.UnEquipItem(slotIndex);
            UpdateEquipmentUI(genderIndex);
            RefreshItems();
            return oldItem;
        }

        return null;
    }

    public void UnEquipAll()
    {
        for(int i = 0;i< equipmentList_A.equippedItems.Count;i++)
        {
            UnEquip(0,i);
            UnEquip(1, i);
            equipmentImage_A[i].sprite = null;
            equipmentImage_A[i].enabled = false; 
            equipmentImage_B[i].sprite = null;
            equipmentImage_B[i].enabled = false; 
        }
    }

    public static void UpdateItemInfo(Image itemImage,string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
        instance.itemImage.sprite = itemImage.sprite;
    }    
    public static void RefreshItems() //Delete all slots and create new ones
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
    private void UpdateEquipmentUI(int genderIndex)
    {
        if (genderIndex == 0)
        {
            UpdateEquipmentUI_A();
        }
        else
        {
            UpdateEquipmentUI_B();
        }
    }
    public void UpdateEquipmentUI_A()
    {
        for (int i = 0; i < equipmentImage_A.Length; i++)
        {
            Equipment equippedItem = equipmentList_A.GetEquippedItem(i);
            if (equippedItem != null)
            {
                equipmentImage_A[i].sprite = equippedItem.itemImage;
                equipmentImage_A[i].enabled = true;
                equipmentText_A[i].SetActive(false);
            }
            else
            {
                equipmentImage_A[i].sprite = null;
                equipmentImage_A[i].enabled = false;
                equipmentText_A[i].SetActive(true);
            }
        }
    }

    public void UpdateEquipmentUI_B()
    {
        for (int i = 0; i < equipmentImage_A.Length; i++)
        {
            Equipment equippedItem = equipmentList_B.GetEquippedItem(i);
            if (equippedItem != null)
            {
                equipmentImage_B[i].sprite = equippedItem.itemImage;
                equipmentImage_B[i].enabled = true;
                equipmentText_B[i].SetActive(false);
            }
            else
            {
                equipmentImage_B[i].sprite = null;
                equipmentImage_B[i].enabled = false;
                equipmentText_B[i].SetActive(true);
            }
        }
    }
    public EquipmentList GetEquipmentList(int genderIndex)
    {
        if (genderIndex == 0)
        {
            return equipmentList_A;
        }
        else
        {
            return equipmentList_B;
        }
    }

    
}
