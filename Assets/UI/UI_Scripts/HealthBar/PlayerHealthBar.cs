using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBar
{
    //Change color
    public Gradient gradient;
    public Image fill;
    public float colorChangeTime; 
    Color startColor;
    Color targetColor;

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
       SetHealthBar(playerHealth.playerAttackData.health);

        fill.color = gradient.Evaluate(1f);
        UpdateHealthBarText();
    }


    protected override void UpdateHealthBar(float targetValue)
    {
        base.UpdateHealthBar(targetValue);

        UpdateHealthBarText();
        StartCoroutine(SmoothColorChange());
    }

    private IEnumerator SmoothColorChange()
    {
        float elapsed = 0f;
        startColor = fill.color;
        targetColor = gradient.Evaluate(mainSlider.normalizedValue);

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
        if (healthBarText != null)
        {
            healthBarText.text = mainSlider.value.ToString() + " / " + mainSlider.maxValue.ToString();
        }
        
    }
}
