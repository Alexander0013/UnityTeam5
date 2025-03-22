using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            isTransitioning = true;
            SceneTransitionManager.instance.NextScene();
        }
    }
}
