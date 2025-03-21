using UnityEngine;

[CreateAssetMenu(fileName = "New Health Potion", menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    public int healAmount;  

    public override void Use()
    {
        Debug.Log("Useed potion¡Gheal " + healAmount + "¡I");
        //PlayerStats.Instance.Heal(healAmount);  
    }
}
