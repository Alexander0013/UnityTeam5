using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    void Awake()
    {
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

    void Start()
    {
        fadeCanvasGroup.alpha = 0;
    }

    // Single-scene load version (no additive).
    /*
    public IEnumerator FadeOutAndLoadSingle(int buildIndex)
    {
        // 1) Fade the screen to black
        yield return StartCoroutine(Fade(1f));

        // 2) Load the new scene in single mode
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);

        // The old scene is automatically unloaded, the new scene is active.

        // 3) Fade back in
        yield return StartCoroutine(Fade(0f));
    }
    */
    public IEnumerator FadeOutAndLoadSingle(int buildIndex)
    {
        yield return StartCoroutine(Fade(1f));
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        yield return StartCoroutine(Fade(0f));
        // NOW the scene is loaded, so call:
        CharacterManager.instance?.OnSceneSwitchComplete(buildIndex);
    }


    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(
                startAlpha, targetAlpha,
                timeElapsed / fadeDuration
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
