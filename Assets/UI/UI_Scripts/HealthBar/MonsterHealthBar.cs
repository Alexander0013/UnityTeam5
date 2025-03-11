using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : HealthBar
{

    public GameObject redBar;
    public GameObject yellowBar;
    public GameObject Monster;
    public EnemyHealth EnemyHealth;

    public CanvasGroup canvasGroup;
    public Transform player;

    public float distance;

    public float visibilityRange; 
    public float fadeSpeed; 
    private Transform enemyTransform;
    private RectTransform rectTransform;
   
    public Vector3 offset ; 
    public Camera mainCamera; // 主攝影機


    public void Start()
    {
        EnemyHealth = Monster.GetComponent<EnemyHealth>();
        //mainSlider.maxValue = EnemyHealth.currentHealth;
        //mainSlider.value = EnemyHealth.currentHealth;
        //yellowSlider.maxValue = EnemyHealth.currentHealth;
        //yellowSlider.value = EnemyHealth.currentHealth;
        canvasGroup.alpha = 0f;
        enemyTransform = transform.parent;
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
        distance = Vector3.Distance(player.position, enemyTransform.position);
        // 如果目標在視野內，則顯示血條，否則隱藏
        if (screenPos.z > 0&& distance <= visibilityRange)
        {
            rectTransform.position = screenPos;
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 1f)); //  fade in
            //Debug.Log("Visible");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(canvasGroup, 0f)); //  fade out
            //Debug.Log("Invisible");
        }
    }


    //處理淡入淡出過程
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha) 
    {
        while (!Mathf.Approximately(cg.alpha, targetAlpha))
        {
            cg.alpha = Mathf.Lerp(cg.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            redBar.SetActive(targetAlpha > 0);
            yellowBar.SetActive(targetAlpha == 1);
            
            yield return null;
        }
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
