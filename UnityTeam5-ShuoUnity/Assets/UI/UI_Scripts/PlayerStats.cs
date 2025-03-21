using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats :MonoBehaviour
{
    public AttackData AttackData_A;
    public AttackData AttackData_B;

    public TextMeshProUGUI damageText_A;
    public TextMeshProUGUI healthText_A;
    public TextMeshProUGUI damageText_B;
    public TextMeshProUGUI healthText_B;

    // 新增 Stat 來管理數值
    private List<Stat> healthStats = new List<Stat>(); // 每個角色的血量
    private List<Stat> damageStats = new List<Stat>(); // 每個角色的攻擊力

    void Start()
    {
        // 初始化兩位角色的數值，使用 AttackData 內的基礎數值
        healthStats.Add(new Stat(AttackData_A.health)); // 角色 A
        healthStats.Add(new Stat(AttackData_B.health)); // 角色 B

        damageStats.Add(new Stat(AttackData_A.baseDamage)); // 角色 A
        damageStats.Add(new Stat(AttackData_B.baseDamage)); // 角色 B

        InventoryManager.instance.onEquipmentChanged += OnEquipmentChanged;
        UpdateUIText();
    }
    //合併的時候角色能力值訂閱裝備變化事件
    //新增OnEquipmentChanged方法即可
    void OnEquipmentChanged(Equipment newItem, Equipment oldItem,int genderIndex)
    {
        if (newItem != null)
        {
            healthStats[genderIndex].AddModifier(genderIndex, newItem.healthModifier);
            damageStats[genderIndex].AddModifier(genderIndex, newItem.damageModifier);
            //Debug.Log("newItemChanged");
        }
        if (oldItem != null)
        {
            healthStats[genderIndex].RemoveModifier(genderIndex, oldItem.healthModifier);
            damageStats[genderIndex].RemoveModifier(genderIndex, oldItem.damageModifier);
            //Debug.Log("oldItemChanged");
        }
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        healthText_A.text = healthStats[0].GetValue().ToString();
        damageText_A.text = damageStats[0].GetValue().ToString();
        healthText_B.text = healthStats[1].GetValue().ToString();
        damageText_B.text = damageStats[1].GetValue().ToString();
    }
}
