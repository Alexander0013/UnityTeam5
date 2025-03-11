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
    private bool isRunning = false;
    private float yellowBarTarget;
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
        if (playerHealth != null)
        {
            healthSlider.value = playerHealth.CurrentHealth;
        }

        yellowBarTarget = healthSlider.value; // 確保目標數值正確
        StartSmoothYellowBar(); // 使用統一方法來啟動協程

        UpdateHealthBarText();
        StartCoroutine(SmoothColorChange()); // 確保顏色變化

    }


    public void StartSmoothYellowBar()
    {
        if (!isRunning)
        {
            StartCoroutine(SmoothYellowBar());
        }
    }

    public IEnumerator SmoothYellowBar()
    {
        isRunning = true;

        yield return new WaitForSeconds(0.2f); 
        //while (!Mathf.Approximately(yellowSlider.value, targetValue))
        //{
        //    yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, targetValue, smoothSpeed * Time.deltaTime);
        //    yield return null;
        //}

        while (Mathf.Abs(yellowSlider.value - yellowBarTarget) > 0.01f) // 避免無窮迴圈
        {
            yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, yellowBarTarget, smoothSpeed * Time.deltaTime);
            yield return null;
        }

        isRunning = false;
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
