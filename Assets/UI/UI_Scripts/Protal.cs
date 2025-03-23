using UnityEngine;
using UnityEngine.SceneManagement;

public class Protal : MonoBehaviour
{
    public int targetSceneIndex;
    private bool isPlayerInRange = false;

    void Update()
    {
        // When player is in range and presses "E", we load the new scene (single).
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneController.instance.StartCoroutine(
                SceneController.instance.FadeOutAndLoadSingle(targetSceneIndex)
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  
        {
            isPlayerInRange = true;
            UI_Manager.instance.ShowInteractionText("按E傳送"); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            UI_Manager.instance.HideInteractionText();
        }
    }
}
