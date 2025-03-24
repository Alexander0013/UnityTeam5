using UnityEngine;
using UnityEngine.SceneManagement;

public class Protal : MonoBehaviour
{
    public int targetSceneIndex; 
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            UI_Manager.instance.ShowInteractionText("��E�ǰe");
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
