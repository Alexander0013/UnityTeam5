using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    private HealthBar healthBar;

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>(); // 自動尋找子物件的血條
    }

    void OnMouseDown() // 滑鼠點擊怪物
    {
        if (healthBar != null)
        {
            Debug.Log("get damage");
            healthBar.TakeDamage(10); //  讓血條減少
        }
    }
}
