using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    private EnemyHealth enemyHealth; // enemyHealth script
    private Transform enemyTransform;

    //Canvas canvas;
    RectTransform rectTransform;
    public Vector3 offset;
    public int smoothMove;

    //RectTransform canvasRect;
    //RectTransform rectTransform;
    Vector3 worldPosition;
    Vector3 directionToCamera;
    Vector3 currentScale;

    public void Initialize(GameObject enemy)
    {
        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyTransform = enemy.transform;

        enemyHealth.OnHealthChanged += UpdateHealthBar;
        enemyHealth.OnDeath += DestroyHealthBar;

        //canvas = GetComponentInParent<Canvas>();
        //canvasRect = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        StartCoroutine(DelayedInitialization());
    }

    private void OnDisable()
    {
        enemyHealth.OnHealthChanged -= UpdateHealthBar;
        enemyHealth.OnDeath -= DestroyHealthBar;
    }



    private IEnumerator DelayedInitialization()
    {
        yield return new WaitUntil(() => enemyHealth.isInitialized);
        SetHealthBar(enemyHealth.currentHealth);
    }

    

    void LateUpdate()
    {
        UpdateTransform();
    }

    //screen space camera
    //void UpdateTransform()
    //{
    //    if (enemyTransform != null)
    //    {
    //        //1.計算怪物的世界座標(加上偏移)
    //    Vector3 worldPosition = enemyTransform.position + offset;

    //        //2.將世界座標轉換為螢幕座標
    //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

    //        //3.將螢幕座標轉換為 Canvas 的本地座標
    //    Vector2 localPoint;
    //        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvas.worldCamera, out localPoint))
    //        {
    //            //4.平滑移動血條
    //            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, localPoint, smoothSpeed * Time.deltaTime);
    //        }
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
            worldPosition = enemyTransform.position + offset;            
            rectTransform.position = worldPosition;

            // 固定血條縮放比例，防止它在不同角度下變形
            currentScale = rectTransform.localScale;

            if (rectTransform.localScale.x > 0)
            {
                rectTransform.localScale = new Vector3(-Mathf.Abs(rectTransform.localScale.x), rectTransform.localScale.y, rectTransform.localScale.z); // 從右至左顯示
            }
            //else
            //{
            //    rectTransform.localScale = new Vector3(Mathf.Abs(rectTransform.localScale.x), rectTransform.localScale.y, rectTransform.localScale.z); // 從左至右顯示
            //}

            // 確保血條不會縮放改變高度（固定 Y 軸比例）
            rectTransform.localScale = new Vector3(rectTransform.localScale.x, 1, rectTransform.localScale.z); // 保持高度一致
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
