using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    // Reference to the CanvasGroup component on the fade canvas.
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.0f;
    private bool isTransitioning = false;

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

    private void Start()
    {
        // When the scene loads, start with a fade-in (alpha from 1 to 0)
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fades in from black (alpha=1) to transparent (alpha=0).
    /// </summary>
    public IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Fades out from transparent (alpha=0) to black (alpha=1), loads the new scene asynchronously,
    /// then fades in (alpha=1 to 0).
    /// </summary>
    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Fade out: from 0 to 1.
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        // Load scene asynchronously.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in: from 1 to 0.
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
        isTransitioning = false;
    }

    /// <summary>
    /// Initiates the transition process.
    /// </summary>
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }
}
