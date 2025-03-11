using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider yellowSlider;    
    public Gradient gradient;
    public Image fill;
    //public float health;
    //public float maxHealth;
    public float smoothSpeed = 0.3f;

    public PlayerHealth playerHealth;

    protected virtual void Start()
    {
        //Initialize the health bar
        healthSlider.maxValue = playerHealth.playerAttackData.health;
        yellowSlider.maxValue = playerHealth.playerAttackData.health;
        healthSlider.value = playerHealth.CurrentHealth;
        yellowSlider.value = playerHealth.CurrentHealth;


        playerHealth.OnHealthChanged += UpdateHealthBar;
        fill.color = gradient.Evaluate(1f);
       
       // healthSlider.value = 1f;
        //yellowSlider.value = 1f;
    }
    //can delete
    //when get hurt "SetDamage()"
    //public virtual void SetHealthBar(float damage)
    //{
    //    health -= damage;
    //    float hpValue = health/maxHealth;

    //    hpSlider.value = hpValue;

    //    fill.color = gradient.Evaluate(Mathf.Lerp(1, hpSlider.normalizedValue, 0.3f * Time.deltaTime));
    //    StartCoroutine(SmoothYellowBar(hpValue));       
    //}
    
    private void UpdateHealthBar()
    {
        if (playerHealth != null)
        {
            healthSlider.value = playerHealth.CurrentHealth;
        }
        fill.color = gradient.Evaluate(Mathf.Lerp(1, healthSlider.normalizedValue, 0.3f * Time.deltaTime));
        StartCoroutine(SmoothYellowBar(healthSlider.value));
    }

    public IEnumerator SmoothYellowBar(float targetValue)
    {
        yield return new WaitForSeconds(0.3f); // wait for 0.3 seconds
        while (!Mathf.Approximately(yellowSlider.value, targetValue))
        {
            yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, targetValue, smoothSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
