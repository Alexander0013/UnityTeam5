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
    private RectTransform rectTransform;
    private bool isVisible = false;
    //備註起來的部分為globel Canvas用(未完成)
    //public Transform enemy; // 怪物的 Transform    
    public Vector3 offset = new Vector3(0, 2, 0); // 調整血條位置
    public Camera mainCamera; // 主攝影機

    private bool isFading = false;


    protected override void Start()
    {
        healthSlider.value = 1f;  // 初始為滿血
        yellowSlider.value = 1f;
        //health = maxHealth;
        canvasGroup.alpha = 0f;
        enemyTransform = transform.parent;
        redBar.SetActive(false);
        yellowBar.SetActive(false);

        rectTransform = GetComponent<RectTransform>();

    }
    void Update()
    {
        //UpdateVisibility();
        //FollowMonster();
    }

    //void FollowMonster()
    //{
    //    transform.position = enemy.position + offset;
    //    transform.forward = Camera.main.transform.forward;
    //}


    void LateUpdate()
    {
        if (enemyTransform == null) return;

        // 轉換世界座標到螢幕座標
        Vector3 screenPos = mainCamera.WorldToScreenPoint(enemyTransform.position + offset);

        // 如果目標在視野內，則顯示血條，否則隱藏
        if (screenPos.z > 0)
        {
            rectTransform.position = screenPos;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 1f)); //  fade in 
            isVisible = true;
            Debug.Log("Visible");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); //  fade out
            isVisible = false;
            Debug.Log("Invisible");
        }
    }


    //private void UpdateVisibility() 
    //{
    //    if (player == null) return;

    //    float distance = Vector3.Distance(player.position, enemyTransform.position);
    //    //float distance = Vector3.Distance(player.position, enemy.position);

    //    if (distance <= visibilityRange && !isVisible)
    //    {
    //        StopAllCoroutines();
    //        StartCoroutine(FadeCanvasGroup(canvasGroup, 1f)); //  fade in 
    //        isVisible = true;
    //        Debug.Log("Visible");
    //    }
    //    else if (distance > visibilityRange && isVisible)
    //    {
    //        StopAllCoroutines();
    //        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); //  fade out
    //        isVisible = false;
    //        Debug.Log("Invisible");
    //    }
    //}

    //處理淡入淡出過程
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha) 
    {
        isFading = true;
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            redBar.SetActive(targetAlpha > 0);
            yellowBar.SetActive(targetAlpha == 1);
            //if (targetAlpha ==1) //fade in
            //{
            //    redBar.SetActive(true); 
            //    if (cg.alpha == 1 ) yellowBar.SetActive(true);
            //}

            //if (targetAlpha == 0) //fade out
            //{
            //    yellowBar.SetActive(false);
            //    if (cg.alpha == 0) redBar.SetActive(false);
            //}
            yield return null;
        }
        isFading = false;
    }

    ////怪物端差異為血條歸零時隱藏血條
    //public override void SetHealthBar(float damage)
    //{
    //    health -= damage;
    //    float hpValue = health / maxHealth;
    //    hpSlider.value = hpValue;
        
    //    StartCoroutine(SmoothYellowBar(hpValue));

    //    if (hpSlider.value <= 0)
    //    {
    //        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); 
    //    }
    //}
}
