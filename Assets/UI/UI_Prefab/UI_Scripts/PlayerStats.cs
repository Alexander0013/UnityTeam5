using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats :MonoBehaviour
{
    //public AttackData AttackData_A;
    //public AttackData AttackData_B;

    public TextMeshProUGUI damageText_A;
    public TextMeshProUGUI healthText_A;
    public TextMeshProUGUI damageText_B;
    public TextMeshProUGUI healthText_B;

    public Stat stat_A;
    public Stat stat_B;


    void Start()
    {
        InventoryManager.instance.onEquipmentChanged += OnEquipmentChanged;
        UpdateUIText();
    }


    //合併的時候角色能力值訂閱裝備變化事件
    //新增OnEquipmentChanged方法即可
    void OnEquipmentChanged(Equipment newItem, Equipment oldItem,int genderIndex)
    {
        Stat targetStat = genderIndex == 0 ? stat_A : stat_B;
        if (newItem != null)
        {
            targetStat.AddModifier(StatType.Health, genderIndex, newItem.healthModifier);
            targetStat.AddModifier(StatType.Damage, genderIndex, newItem.damageModifier);
            //Debug.Log("newItemChanged");
        }
        if (oldItem != null)
        {
            targetStat.RemoveModifier(StatType.Health, genderIndex, oldItem.healthModifier);
            targetStat.RemoveModifier(StatType.Damage, genderIndex, oldItem.damageModifier);
        }
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        healthText_A.text = stat_A.GetValue(StatType.Health).ToString();
        damageText_A.text = stat_A.GetValue(StatType.Damage).ToString();
        healthText_B.text = stat_B.GetValue(StatType.Health).ToString();
        damageText_B.text = stat_B.GetValue(StatType.Damage).ToString();
    }


}
