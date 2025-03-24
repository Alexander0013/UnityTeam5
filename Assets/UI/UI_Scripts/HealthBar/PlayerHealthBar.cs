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


    public void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdatePlayerHealthBar;
        }
    }

    protected virtual void Start()
    {
        StartCoroutine(WaitUntilUIIsReady());
        fill.color = gradient.Evaluate(1f);
    }

     IEnumerator WaitUntilUIIsReady()
     {
        while (UI_Manager.instance.IsReady !=true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.01f);
        playerHealth.OnHealthChanged += UpdatePlayerHealthBar;
        
        SetHealthBar(playerHealth.playerAttackData.health);
        UpdateHealthBarText();
        yield return null;
     }

    public void UpdatePlayerHealthBar(float currentHealth,float maxHealth)
    {
        base.UpdateHealthBar(currentHealth);
        mainSlider.maxValue = maxHealth;
        if (yellowSlider != null)
        {
            yellowSlider.maxValue = maxHealth;
        }        
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
