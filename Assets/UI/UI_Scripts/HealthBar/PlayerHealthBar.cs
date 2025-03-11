using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider yellowSlider;    

    public Gradient gradient;
    public Image fill;
    public float colorChangeTime; // 顏色變化時間
    Color startColor;
    Color targetColor;

    public float smoothSpeed;
    public PlayerHealth playerHealth;

    public TextMeshProUGUI healthBarText;



    public void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    public void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    protected virtual void Start()
    {
        //Initialize the health bar
        healthSlider.maxValue = playerHealth.playerAttackData.health;
        yellowSlider.maxValue = playerHealth.playerAttackData.health;
        healthSlider.value = healthSlider.maxValue;
        yellowSlider.value = yellowSlider.maxValue;

        fill.color = gradient.Evaluate(1f);
        UpdateHealthBarText();
        //UpdateHealthBar();
    }
    
    
    private void UpdateHealthBar()
    {
        StartCoroutine(SmoothColorChange());
        if (playerHealth != null)
        {
            healthSlider.value = playerHealth.CurrentHealth;
        }
        StartCoroutine(SmoothYellowBar(healthSlider.value));
        UpdateHealthBarText();

    }

    public IEnumerator SmoothYellowBar(float targetValue)
    {
        yield return new WaitForSeconds(0.2f); // wait for 0.3 seconds
        while (!Mathf.Approximately(yellowSlider.value, targetValue))
        {
            yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, targetValue, smoothSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SmoothColorChange()
    {        
        float elapsed = 0f;
        startColor = fill.color;
        targetColor = gradient.Evaluate(healthSlider.normalizedValue);

        while (elapsed < colorChangeTime)
        {
            elapsed += Time.deltaTime;
            fill.color = Color.Lerp(startColor, targetColor, elapsed / colorChangeTime);
            yield return null;
        }

        fill.color = targetColor;
    }

    public void UpdateHealthBarText()
    {
        healthBarText.text = healthSlider.value.ToString()+ " / " + healthSlider.maxValue.ToString();
    }
}
