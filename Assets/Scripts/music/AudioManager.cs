using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip openingMusic;
    public AudioClip templeMusic;
    public AudioClip explorationMusic;
    public AudioClip battleMusic;

    [Header("Fade Settings")]
    public float fadeDuration = 2.0f;
    public bool isBattleMusicActive = false;

    private void Awake()
    {
        // Singleton pattern: ensure only one AudioManager exists.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes.
        }
        else
        {
            Destroy(gameObject);
        }
        //PlayMusic(openingMusic);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called every time a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Change music based on the scene name.
        // (Replace "OpenScene", "TempleScene", etc., with your actual scene names.)
        if (scene.name == "OpeningVideo")
        {
            PlayMusic(openingMusic);
        }
        else if (scene.name == "Temple")
        {
            PlayMusic(templeMusic);
        }
        else if (scene.name == "PureNature")
        {
            // In PureNature, default to exploration music.
            PlayMusic(explorationMusic);
        }
       
    }

    // Cross-fades to a new music clip.
    public void PlayMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip) return;
        StartCoroutine(CrossFadeMusic(newClip));
    }

    IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // Fade out current music.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = startVolume;
    }

    // Call this to switch to battle music (e.g., in PureNature when battle begins).
    public void PlayBattleMusic()
    {
        // Optional: check that you're in the correct scene.
        if (SceneManager.GetActiveScene().name == "PureNature")
            PlayMusic(battleMusic);
    }

    // Call this to switch back to exploration music (after battle ends in PureNature).
    public void PlayExplorationMusic()
    {
        if (SceneManager.GetActiveScene().name == "PureNature")
            PlayMusic(explorationMusic);
    }
    public void TriggerBattleMusic()
    {
        if (!isBattleMusicActive)
        {
            PlayBattleMusic();
            isBattleMusicActive = true;
        }
    }

    public void ResetBattleMusic()
    {
        isBattleMusicActive = false;
        // When you reset, switch back to exploration music.
        PlayExplorationMusic();
    }

}
