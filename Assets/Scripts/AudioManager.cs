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
    public float fadeDuration = 1.0f;

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
        if (scene.name == "OpenScene")
        {
            PlayMusic(openingMusic);
        }
        else if (scene.name == "TempleScene")
        {
            PlayMusic(templeMusic);
        }
        else if (scene.name == "PureNatureScene")
        {
            // In PureNature, default to exploration music.
            PlayMusic(explorationMusic);
        }
        else if (scene.name == "BossScene")
        {
            PlayMusic(battleMusic);
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
        if (SceneManager.GetActiveScene().name == "PureNatureScene")
            PlayMusic(battleMusic);
    }

    // Call this to switch back to exploration music (after battle ends in PureNature).
    public void PlayExplorationMusic()
    {
        if (SceneManager.GetActiveScene().name == "PureNatureScene")
            PlayMusic(explorationMusic);
    }
}
