using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBar
{
    public GameObject Enemy;
    public Transform EnemyTransform;
    public CanvasGroup canvasGroup;
    //public Image barImage;
    Canvas canvas;
    RectTransform rectTransform;
    Camera cam;
    public Vector3 offset;
    public EnemyHealth enemyHealth;


   
    private void OnDisable()
    {
        enemyHealth.OnHealthChanged -= UpdateHealthBar;
        enemyHealth.OnDeath -= DestroyHealthBar;
    }


    public void InitializeHealthBar(GameObject enemy)
    {
        this.Enemy = enemy;
        this.enemyHealth = enemy.GetComponent<EnemyHealth>();
        this.EnemyTransform = enemy.transform;
        this.canvasGroup = GetComponent<CanvasGroup>();
        cam = UI_Manager.instance.mainCamera;
        canvas = UI_Manager.instance.canvas;
        rectTransform = UI_Manager.instance.rectTransform;
        enemyHealth.OnHealthChanged += UpdateHealthBar;
        enemyHealth.OnDeath += DestroyHealthBar;
        // ³]¸mªì©l¦å±ø
        SetHealthBar(Enemy.GetComponent<EnemyFSM>().npcData.maxHealth);
    }

    public void UpdateHealthBarPos()
    {
        if (EnemyTransform != null)
        {
            Vector3 spos = UI_Manager.instance.mainCamera.WorldToScreenPoint(EnemyTransform.position + offset);
            if (spos.z < 0)
            {
                canvasGroup.alpha = 0;
                return;
            }
            else if (spos.z > 0)
            {
                canvasGroup.alpha = 1;
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

    void DestroyHealthBar()
    {
        this.canvasGroup.alpha = Mathf.Lerp(1f, 0f, 10f);
        UI_Manager.instance.UnregisterHealthBar(Enemy);
        Destroy(gameObject);
    }
}
