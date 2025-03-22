using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{   
    public CanvasGroup fadeCanvasGroup;  
    public float fadeDuration = 1f;      // 淡入淡出動畫持續時間


    public static SceneController instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 避免場景切換時摧毀物件
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

    public IEnumerator FadeOutAndLoad(int buildIndex)
    {
        yield return StartCoroutine(Fade(1));

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

        yield return new WaitUntil(() => loadOperation.isDone);

        //Camera currentCamera = Camera.main;
        //if (currentCamera != null)
        //{
        //    currentCamera.gameObject.SetActive(false);  // 禁用舊場景的相機
        //}

        //if (currentSceneIndex != 1)
        //{
        //    SceneManager.UnloadSceneAsync(currentSceneIndex);
        //}
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        // 執行淡入動畫
        yield return StartCoroutine(Fade(0));
    }

    // 控制透明度的淡入淡出過程
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    
}
