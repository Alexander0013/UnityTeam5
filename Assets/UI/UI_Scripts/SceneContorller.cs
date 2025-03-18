using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SceneContorller : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 連結到場景中的VideoPlayer物件
    public Button playPauseButton;  // 連結到UI中的Button

    private bool isPlaying = true;

    void Start()
    {
        // 設定按鈕的事件
        playPauseButton.onClick.AddListener(TogglePlayPause);
    }

    void TogglePlayPause()
    {
        if (isPlaying)
        {
            videoPlayer.Pause();  // 暫停影片
        }
        else
        {
            videoPlayer.Play();   // 播放影片
        }

        // 切換播放狀態
        isPlaying = !isPlaying;
    }
}
