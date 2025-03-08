using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : CharaterStats
{
    public TextMeshProUGUI damageText_A;
    public TextMeshProUGUI healthText_A;
    public TextMeshProUGUI damageText_B;
    public TextMeshProUGUI healthText_B;

    void Start()
    {
        InventoryManager.instance.onEquipmentChanged += OnEquipmentChanged;
        UpdateUIText();
    }
    //合併的時候角色能力值訂閱裝備變化事件
    //新增OnEquipmentChanged方法即可
    void OnEquipmentChanged(Equipment newItem, Equipment oldItem,int genderIndex)
    {
        if (newItem != null)
        {
            health.AddModifier(genderIndex,newItem.healthModifier);
            damage.AddModifier(genderIndex,newItem.damageModifier);
            //Debug.Log("newItemChanged");
        }
        if (oldItem != null)
        {
            health.RemoveModifier(genderIndex, oldItem.healthModifier);
            damage.RemoveModifier(genderIndex, oldItem.damageModifier);
            //Debug.Log("oldItemChanged");
        }
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        healthText_A.text = health.GetValue_A().ToString();
        damageText_A.text = damage.GetValue_A().ToString();
        healthText_B.text = health.GetValue_B().ToString();
        damageText_B.text = damage.GetValue_B().ToString();
    }
}
