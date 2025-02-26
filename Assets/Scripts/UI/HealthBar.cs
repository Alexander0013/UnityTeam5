using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar :PlayerHealthBar
{

    public GameObject redBar;
    public GameObject yellowBar;       
    public CanvasGroup canvasGroup;
    public Transform player;
    public float visibilityRange = 10f; //顯示距離
    public float fadeSpeed=2f; //淡入淡出速度
    private Transform enemyTransform;
    private bool isVisible = false;


    protected override void Start()
    {
        hpSlider.value = 1f;  // 初始為滿血
        yellowSlider.value = 1f;
        health = maxHealth;
        canvasGroup.alpha = 0f;
        enemyTransform = transform.parent;
        redBar.SetActive(false);
        yellowBar.SetActive(false);
    }

    void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility() //決定顯示血條與否
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, enemyTransform.position);

        if (distance <= visibilityRange && !isVisible)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 1f)); //  淡入
            isVisible = true;
        }
        else if (distance > visibilityRange && isVisible)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); //  淡出
            isVisible = false;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha) //處理淡入淡出過程
    {
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            if (targetAlpha ==1 ) //淡入
            {
                redBar.SetActive(true); 
                if (cg.alpha == 1 ) yellowBar.SetActive(true);
            }

            if (targetAlpha == 0) //淡出
            {
                yellowBar.SetActive(false);
                if (cg.alpha == 0) redBar.SetActive(false);
            }
            yield return null;
        }
    }

    public override void SetDamage(float damage)
    {
        health -= damage;
        float hpValue = health / maxHealth;
        hpSlider.value = hpValue;
        
        StartCoroutine(SmoothYellowBar(hpValue));

        if (hpSlider.value <= 0)
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); // 血量歸零時隱藏血條
        }
    }

    //public void TakeDamage(float damage)
    //{
    //    damage = Mathf.Clamp(damage, 0, maxHealth);
    //    float healthPercent = damage / maxHealth;

    //    hpSlider.value = healthPercent;
    //    StartCoroutine(SmoothYellowBar(healthPercent));

    //    if (hpSlider.value <= 0)
    //    {
    //        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); // 血量歸零時隱藏血條
    //    }
    //}



}
