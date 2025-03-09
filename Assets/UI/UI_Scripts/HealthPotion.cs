using UnityEngine;

[CreateAssetMenu(fileName = "New Health Potion", menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    public int healAmount;  // 設定藥水恢復的數值

    public override void Use()
    {
        Debug.Log("使用了藥水：恢復 " + healAmount + " 點血量！");
        //PlayerStats.Instance.Heal(healAmount);  
    }
}
