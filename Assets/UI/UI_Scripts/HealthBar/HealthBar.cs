using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider mainSlider;
    public Slider yellowSlider;

    //Smooth color change
    public float smoothSpeed;
    protected bool isRunning = false;
    protected float yellowBarTarget;

    protected virtual void StartSmoothYellowBar()
    {
        if (!isRunning)
        {
            StartCoroutine(SmoothYellowBar());
        }
    }
    protected virtual IEnumerator SmoothYellowBar()
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
