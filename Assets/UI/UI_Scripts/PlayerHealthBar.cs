using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider hpSlider;
    public Slider yellowSlider;    
    public Gradient gradient;
    public Image fill;
    public float health;
    public float maxHealth;
    public float smoothYellowBar = 0.3f;

    protected virtual void Start()
    {
        fill.color = gradient.Evaluate(1f);
        health = maxHealth;
        hpSlider.value = 1f;
        yellowSlider.value = 1f;
    }

    //受傷端用SetDamage()，參數為受到的傷害值
    public virtual void SetDamage(float damage)
    {
        health -= damage;
        float hpValue = health/maxHealth;
        hpSlider.value = hpValue;
        fill.color = gradient.Evaluate(Mathf.Lerp(1, hpSlider.normalizedValue, 0.3f * Time.deltaTime));
        StartCoroutine(SmoothYellowBar(hpValue));       
    }

    //黃血條緩降，下降速度用smoothYellowBar控制
    public IEnumerator SmoothYellowBar(float targetValue)
    {
        yield return new WaitForSeconds(0.3f); // 延遲開始下降
        while (!Mathf.Approximately(yellowSlider.value, targetValue))
        {
            yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, targetValue, smoothYellowBar * Time.deltaTime);
            yield return null;
        }
    }


}
