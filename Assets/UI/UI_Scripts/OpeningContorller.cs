using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningContorller : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public float pauseTime;

    public CanvasGroup firstCanvasGroup; 
    public CanvasGroup secondCanvasGroup; 
    public float firstFadeDuration = 1f; 
    public float secondFadeDuration = 3f;
    bool isPlaying = true;

    public string nextSceneName = "Temple";


    private void Update()
    {
        PauseWhenTimesUp();
        Debug.Log(audioSource.volume);
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
        secondCanvasGroup.alpha =1;
    }

    IEnumerator FadeOut()
    {
        float startAlpha = secondCanvasGroup.alpha;
        float startVolume = audioSource.volume;
        for (float t = 0; t < secondFadeDuration; t += Time.deltaTime)
        {
            secondCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / 2);
            firstCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / 2);
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / 2);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }
    public void OnStartButtonClicked()
    {
        videoPlayer.Play();
        StartCoroutine(FadeOut());
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        SceneTransitionManager.instance.TransitionToScene("Temple");
    }
   
    public void OnQuitButtonClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
