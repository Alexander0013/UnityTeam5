using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Protal : MonoBehaviour
{
    public int targetSceneIndex; 
    private bool isPlayerInRange = false;  // 用來檢測玩家是否進入傳送門範圍

    void Update()
    {
        // 當玩家進入範圍並按下 "E" 鍵
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneController.instance.StartCoroutine(SceneController.instance.FadeOutAndLoad(targetSceneIndex));
        }
    }

    // 當玩家進入傳送門範圍時觸發
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            isPlayerInRange = true;
        }

        UI_Manager.instance.ShowInteractionText("按E傳送");
    }

    // 當玩家離開傳送門範圍時觸發
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }

        UI_Manager.instance.HideInteractionText();
    }

   
}
