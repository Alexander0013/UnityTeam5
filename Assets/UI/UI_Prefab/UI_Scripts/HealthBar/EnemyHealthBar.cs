using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    CanvasGroup canvasGroup;
    Canvas canvas;
    RectTransform rectTransform;
    Camera cam;

    GameObject Enemy;
    Transform EnemyTransform;
    EnemyHealth enemyHealth;

    public Vector3 offset;
    public Vector3 shooterOffset;
    public GameObject elementImage;
    public bool element = false;

    //HealthBar Fade in/out
    public float visibleDistance;
    public float fadeSpeed;

    private ElementalStatus elementalStatus;
    
    bool isFading = false;
    float targetAlpha;

    public void InitializeHealthBar(GameObject enemy)
    {
        this.Enemy = enemy;
        this.enemyHealth = enemy.GetComponent<EnemyHealth>();
        this.EnemyTransform = enemy.transform;
        this.canvasGroup = GetComponent<CanvasGroup>();
        this.elementalStatus = enemy.GetComponent<ElementalStatus>();
        
        if(elementalStatus == null ) { Debug.Log("elementalStatus null"); }
        if(elementalStatus!=null )
        {
            elementalStatus.OnElementApplied += ShowElement;
        }
        this.elementImage.SetActive(false);
        cam = UI_Manager.instance.mainCamera;
        canvas = UI_Manager.instance.canvas;
        rectTransform = UI_Manager.instance.rectTransform;
        canvasGroup.alpha = 0f;
        enemyHealth.OnHealthChanged += UpdateHealthBar;
        enemyHealth.OnDeath += DestroyHealthBar;

        SetHealthBar(Enemy.GetComponent<EnemyFSM>().npcData.maxHealth);
    }

    private void OnDisable()
    {
        if (elementalStatus != null)
        {
            elementalStatus.OnElementApplied -= ShowElement;
        }
        enemyHealth.OnHealthChanged -= UpdateHealthBar;
        enemyHealth.OnDeath -= DestroyHealthBar;
    }

    public void UpdateHealthBarPos()
    {
        if (EnemyTransform != null)
        {
            Vector3 eOffset;
            if (enemyHealth.gender == Gender.Female)
            {
                eOffset = shooterOffset;
            }
            else { eOffset = offset; }

            Vector3 spos = UI_Manager.instance.mainCamera.WorldToScreenPoint(EnemyTransform.position + eOffset);
            
            float distance = Vector3.Distance(Enemy.transform.position, UI_Manager.instance.playerPosition);
            
            if (spos.z < 0 || distance > visibleDistance)
            {
                targetAlpha = 0;
                StartCoroutine(FadeOutHealthBar(targetAlpha));
                return;
            }
            else if (spos.z > 0&& distance<visibleDistance)
            {
                targetAlpha = 1;
                StartCoroutine(FadeOutHealthBar(targetAlpha));
            }
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
            {
                Vector2 vout = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, spos, cam, out vout);
                spos = vout;
                transform.localPosition = spos;
            }
            else
            {
                transform.position = spos;
            }
        }
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

    void DestroyHealthBar()
    {

        UI_Manager.instance.UnregisterHealthBar(Enemy);
        StopAllCoroutines();
        StartCoroutine(FadeOutHealthBar(0));        
        Destroy(gameObject);
    }

    void ShowElement(ElementType elementType)
    {
        Debug.Log("elementType="+elementType);
        if(elementType== ElementType.Electro)
        {
            elementImage.SetActive(true);
        }
        else
        {
            elementImage.SetActive(false);
        }
    }
}
