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
    //�x�s�ͦ��L��slots
    public List<GameObject> slots = new List<GameObject>();

    //Equipment
    EquipmentList equipmentList;

    public EquipmentList equipmentList_A;
    public Image[] equipmentSlots_A;
    public GameObject[] equipmentText_A;

    public EquipmentList equipmentList_B;
    public Image[] equipmentSlots_B;
    public GameObject[] equipmentText_B;

    public delegate void OnEquipmentChanged(Equipment newItem,Equipment oldItem,int genderIndex);
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


    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
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

        // ��s `EquipmentList`      
        equipmentList.EquipItem(typeIndex, newItem);    

        // Ĳ�o UI ��s
        UpdateEquipmentUI(genderIndex);

        onEquipmentChanged?.Invoke(newItem, oldItem, genderIndex);
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
            equipmentSlots_A[i].sprite = null;
            equipmentSlots_A[i].enabled = false; // ���ùϤ�
            equipmentSlots_B[i].sprite = null;
            equipmentSlots_B[i].enabled = false; // ���ùϤ�
        }
    }

    public static void UpdateItemInfo(Image itemImage,string itemDescription)
    {
        instance.itemInfo.text = itemDescription;
        instance.itemImage.sprite = itemImage.sprite;
    }
    
    public static void RefreshItems() //�P���I�]����->���s�ͦ�����(�ƶq�Q��s)
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
            //Debug.Log("gender0");
        }
        else
        {
            UpdateEquipmentUI_B();
            //Debug.Log("gender1");
        }
        //UpdateStatsText();
    }

    public void UpdateEquipmentUI_A()
    {
        for (int i = 0; i < equipmentSlots_A.Length; i++)
        {
            Equipment equippedItem = equipmentList_A.GetEquippedItem(i);
            if (equippedItem != null)
            {
                equipmentSlots_A[i].sprite = equippedItem.itemImage;
                equipmentSlots_A[i].enabled = true;
                equipmentText_A[i].SetActive(false);
            }
            else
            {
                equipmentSlots_A[i].sprite = null;
                equipmentSlots_A[i].enabled = false;
                equipmentText_A[i].SetActive(true);
            }
        }
    }

    public void UpdateEquipmentUI_B()
    {
        for (int i = 0; i < equipmentSlots_A.Length; i++)
        {
            Equipment equippedItem = equipmentList_B.GetEquippedItem(i);
            if (equippedItem != null)
            {
                equipmentSlots_B[i].sprite = equippedItem.itemImage;
                equipmentSlots_B[i].enabled = true;
                equipmentText_B[i].SetActive(false);
            }
            else
            {
                equipmentSlots_B[i].sprite = null;
                equipmentSlots_B[i].enabled = false;
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
