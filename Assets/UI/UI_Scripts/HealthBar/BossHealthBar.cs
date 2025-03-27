using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : HealthBar
{
    public GameObject boss;
    public BossHealth bossHealth;
    CanvasGroup canvasGroup;

    bool isFading=false;
    public float fadeSpeed;

    //If fight with player-> canvasGroup.alpha = 1


    public void OnDisable()
    {
        if (boss != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnDeath -= BossDie;
        }        
    }

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeOutHealthBar(0));
        StartCoroutine(WairForBoss());
    }
    public void ShowHealthBar()
    {
        StartCoroutine(FadeOutHealthBar(1));
    }

    IEnumerator WairForBoss()
    {
        while (bossHealth.currentHealth==0)
        {
            yield return null;
        }
        bossHealth.OnHealthChanged += UpdateHealthBar;
        bossHealth.OnDeath += BossDie;
        SetHealthBar(bossHealth.currentHealth);
        yield break;
    }

    public void InitializeHealthBar(GameObject boss)
    {
        this.boss = boss;
        this.bossHealth = boss.GetComponent<BossHealth>();
        
    }

    public void BossDie()
    {
        yellowSlider.value =0;
        StartCoroutine(FadeOutHealthBar(0));
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
