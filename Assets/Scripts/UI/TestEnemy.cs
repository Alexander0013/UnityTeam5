using UnityEngine;
using UnityEngine.UI;

public class TestEnemy : MonoBehaviour
{
    public Slider slider;
    private HealthBar healthBar;
    float maxHP = 100f;
    float HP=0;


    void Start()
    {
        HP = maxHP;
        //healthBar = GetComponentInChildren<HealthBar>(); // 自動尋找子物件的血條
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 按下空白鍵
        {
            DealDamage();
        }
    }

    void DealDamage() 
    {
        if (healthBar != null)
        {            
            Debug.Log("get damage");
            healthBar.SetDamage(10);//  讓血條減少
        }
    }
}
