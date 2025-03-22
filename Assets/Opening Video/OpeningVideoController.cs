using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OpeningVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in Inspector.
    public string nextSceneName = "Temple"; // Replace with your main scene name.
    private bool hasTransitioned = false;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.Stop();
            
            if (SceneTransitionManager.instance != null)
            {
                SceneTransitionManager.instance.TransitionToScene(nextSceneName);
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
