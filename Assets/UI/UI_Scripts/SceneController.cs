using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{   
    public CanvasGroup fadeCanvasGroup;  
    public float fadeDuration = 1f;      // �H�J�H�X�ʵe����ɶ�


    public static SceneController instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �קK���������ɺR������
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
        //    currentCamera.gameObject.SetActive(false);  // �T���³������۾�
        //}

        //if (currentSceneIndex != 1)
        //{
        //    SceneManager.UnloadSceneAsync(currentSceneIndex);
        //}
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        // ����H�J�ʵe
        yield return StartCoroutine(Fade(0));
        // 5) **Notify CharacterManager** that the new scene is ready
        if (CharacterManager.instance != null)
        {
            CharacterManager.instance.OnSceneSwitchComplete(buildIndex);
        }
    }

    // ����z���ת��H�J�H�X�L�{
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
