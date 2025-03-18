using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class OpeningVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in Inspector.
    public string nextSceneName = "Temple"; // Replace with your main scene name.

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Optionally add a fade-out effect here.
        SceneManager.LoadScene(nextSceneName);
    }
}
