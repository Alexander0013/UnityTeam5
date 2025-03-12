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
    //public CharacterManager characterManager;
    public TextMeshProUGUI healthBarText;



    public void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthBar;
        //characterManager.SwitchPlayer += StopSmoothBar;
    }

    public void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
        //characterManager.SwitchPlayer -= StopSmoothBar;
    }

    protected virtual void Start()
    {
        //Initialize the health bar
        mainSlider.maxValue = playerHealth.playerAttackData.health;
        yellowSlider.maxValue = playerHealth.playerAttackData.health;
        mainSlider.value = mainSlider.maxValue;
        yellowSlider.value = yellowSlider.maxValue;

        fill.color = gradient.Evaluate(1f);
        UpdateHealthBarText();
    }
    
    
    private void UpdateHealthBar()
    {        
        if (playerHealth != null)
        {
            mainSlider.value = playerHealth.CurrentHealth;
        }

        yellowBarTarget = mainSlider.value; // make sure slider's target value correct
        StartSmoothYellowBar(); 

        UpdateHealthBarText();
        StartCoroutine(SmoothColorChange()); 
    }

    //public void StopSmoothBar()
    //{
    //    StopCoroutine(SmoothColorChange());
    //    yellowSlider.value = mainSlider.value;
    //}

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
        healthBarText.text = mainSlider.value.ToString()+ " / " + mainSlider.maxValue.ToString();
    }
}
