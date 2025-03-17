using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossHealthBar : HealthBar
{
    public GameObject Boss;
    CanvasGroup canvasGroup;
    //boss health script

    bool isFading=false;
    public float fadeSpeed;



    public void OnEnable()
    {
        //.OnHealthChanged += UpdateHealthBar;
        //.OnDeath += BossDie();
    }

    public void OnDisable()
    {
        //.OnHealthChanged -= UpdateHealthBar;
        //.OnDeath -= BossDie();
    }

    protected virtual void Start()
    {
        //SetHealthBar();
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeOutHealthBar(1));
    }

    public void BossDie()
    {
        StartCoroutine(FadeOutHealthBar(0));
        //Destroy(this);
    }

    IEnumerator FadeOutHealthBar(float targetAlpha)
    {
        if (!isFading)
        {
            isFading = true;

            while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.01f)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }
            isFading = false;
        }
        yield return null;
    }
}
