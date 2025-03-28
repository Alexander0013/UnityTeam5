using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningContorller : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    //public AudioSource audioSource;
    float pauseTime= 8.0f;

    public CanvasGroup firstCanvasGroup;
    public CanvasGroup secondCanvasGroup;
    float firstFadeDuration = 1f;
    float secondFadeDuration = 3f;
    bool isPlaying = true;

    private void Start()
    {
        firstCanvasGroup.alpha = 0;
        secondCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        PauseWhenTimesUp();
    }

    void PauseWhenTimesUp()
    {
        if (videoPlayer.time >= pauseTime && isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;
            //Debug.Log("Pause videoPlayer");
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float firstStartAlpha = firstCanvasGroup.alpha;
        for (float t = 0; t < firstFadeDuration; t += Time.deltaTime)
        {
            firstCanvasGroup.alpha = Mathf.Lerp(firstStartAlpha, 1, t / firstFadeDuration);
            yield return null;
        }
        firstCanvasGroup.alpha = 1;

        float secondStartAlpha = secondCanvasGroup.alpha;
        for (float t = 0; t < secondFadeDuration; t += Time.deltaTime)
        {
            secondCanvasGroup.alpha = Mathf.Lerp(secondStartAlpha, 1, t / secondFadeDuration);
            yield return null;
        }
        secondCanvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        float startAlpha = secondCanvasGroup.alpha;
        //float startVolume = audioSource.volume;
        for (float t = 0; t < secondFadeDuration; t += Time.deltaTime)
        {
            secondCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / 2);
            firstCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / 2);
            //audioSource.volume = Mathf.Lerp(startVolume, 0, t / 2);
            yield return null;
        }
        //audioSource.volume = 0;
        //audioSource.Stop();
    }
    public void OnStartButtonClicked()
    {
        videoPlayer.Play();
        StartCoroutine(FadeOut());
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(SceneController.instance.FadeOutAndLoadSingle(1));
    }

    public void OnQuitButtonClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
