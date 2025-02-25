using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider hpSlider;
    public Slider yellowSlider;    
    public Gradient gradient;
    public Image fill;

    public void Start()
    {
        
    }

    public void SetMaxHealth(Slider slider, int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(Slider slider,int damage)
    {
        slider.value -=damage;
        fill.color = gradient.Evaluate(Mathf.Lerp(1, slider.normalizedValue, 0.3f * Time.deltaTime));
            //gradient.Evaluate(slider.normalizedValue);
    }

    

    public IEnumerator SmoothYellowBar(float targetValue)
    {
        yield return new WaitForSeconds(0.3f); // 延遲開始下降
        while (!Mathf.Approximately(yellowSlider.value, targetValue))
        {
            yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, targetValue, 0.3f * Time.deltaTime);
            yield return null;
        }
    }


}
