using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    [SerializeField] Animator transitionAnimator;

    // Reference to the CanvasGroup component on the fade canvas.
    public CanvasGroup fadeCanvasGroup;
    private void Awake()
    {
        // Singleton pattern: ensure only one SceneTransitionManager exists.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }
    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Trigger fade-out animation.
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("End");
        }
        // Wait for fade out to complete.
        yield return new WaitForSeconds(1f);

        // Load the scene asynchronously.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Optionally, wait a frame or a short duration before fading in.
        yield return null;

        // Trigger fade-in animation.
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }
    }
    
}
