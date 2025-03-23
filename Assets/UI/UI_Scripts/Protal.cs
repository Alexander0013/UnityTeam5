using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Protal : MonoBehaviour
{
    public int targetSceneIndex; 
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            UI_Manager.instance.ShowInteractionText("«öE¶Ç°e");
            UI_Manager.instance.inProtalRange = true;
            UI_Manager.instance.targetSceneIndex = targetSceneIndex;
        }
    }

    private void OnTriggerExit(Collider other)
    {       
        if (other.CompareTag("Player"))
        {
            UI_Manager.instance.HideInteractionText();
            UI_Manager.instance.inProtalRange = true;
        }
    }

}
