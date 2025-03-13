using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    private EnemyHealth enemyHealth; // enemyHealth script
    private Transform enemyTransform;

    Canvas canvas;
    public Vector3 pos;
   

    Vector3 worldPosition;
    Vector3 directionToCamera;

    public void Initialize(GameObject enemy)
    {
        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyTransform = enemy.transform;

        enemyHealth.OnHealthChanged += UpdateHealthBar;
        enemyHealth.OnDeath += DestroyHealthBar;

        canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        StartCoroutine(DelayedInitialization());
    }

   

    private IEnumerator DelayedInitialization()
    {
        yield return new WaitUntil(() => enemyHealth.isInitialized);
        SetHealthBar(enemyHealth.currentHealth);
    }

    

    void Update()
    {
        UpdateTransform();
    }

    //screen space camera
    //void UpdateTransform()
    //{
    //    if (enemyTransform != null)
    //    {
    //        // 計算怪物的世界座標
    //        Vector3 worldPosition = enemyTransform.position + vector3;

    //        // 將世界座標轉換為螢幕座標
    //        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

    //        // 將螢幕座標轉換為 Canvas 的本地座標
    //        Vector2 localPoint;
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvas.worldCamera, out localPoint);

    //        // 使用插值來平滑血條的移動
    //        RectTransform rectTransform = GetComponent<RectTransform>();
    //        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, localPoint, smoothMove * Time.deltaTime);
    //    }
    //}

    //world space 
    void UpdateTransform()
    {
        if (enemyTransform != null)
        {
            // 讓血條永遠面向攝影機
            directionToCamera = Camera.main.transform.position - transform.position;
            directionToCamera.y = 0; // 不改變血條的垂直方向 (這樣血條不會朝上或朝下旋轉)

            // 設定血條的旋轉使其始終面向攝影機
            transform.rotation = Quaternion.LookRotation(directionToCamera);

            // 如果血條是 World Space Canvas，保持其位置不變
            worldPosition = enemyTransform.position + pos;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.position = worldPosition;
        }
    }

    void OnDestroy()
    {
        enemyHealth.OnHealthChanged -= UpdateHealthBar;
        enemyHealth.OnDeath -= DestroyHealthBar;
    }

    void DestroyHealthBar()
    {
        Destroy(gameObject);
    }
}
