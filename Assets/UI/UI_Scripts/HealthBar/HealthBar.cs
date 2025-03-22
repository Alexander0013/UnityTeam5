using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider mainSlider;
    public Slider yellowSlider;

    //Smooth yellow bar
    public float smoothSpeed;
    protected bool isRunning = false;
    protected float yellowBarTarget;


    protected virtual void SetHealthBar(float value)
    {
        mainSlider.maxValue = value;        
        mainSlider.value = value;        
        if (yellowSlider != null)
        {
            yellowSlider.maxValue = value;
            yellowSlider.value = value;
        }
    }

    protected virtual void UpdateHealthBar(float targetValue)
    {
        mainSlider.value = targetValue;
        yellowBarTarget = targetValue; // make sure slider's target value correct
        if (!isRunning)
        {
            StartCoroutine(SmoothYellowBar());
        }
    }


    protected virtual IEnumerator SmoothYellowBar()
    {
        if (yellowSlider != null)
        {
            isRunning = true;
            yield return new WaitForSeconds(0.2f);

            while (Mathf.Abs(yellowSlider.value - yellowBarTarget) > 0.01f) // Á×§KµL½a°j°é
            {
                yellowSlider.value = Mathf.MoveTowards(yellowSlider.value, yellowBarTarget, smoothSpeed * Time.deltaTime);
                yield return null;
            }
            isRunning = false;
        }        
    }

   
}
