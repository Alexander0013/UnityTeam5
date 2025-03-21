using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("SFX Source Prefab")]
    // Create a prefab that has an AudioSource (set playOnAwake to false)
    public AudioSource sfxSourcePrefab;

    [Header("Pool Settings")]
    public int poolSize = 20;
    private Queue<AudioSource> audioPool;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        audioPool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = Instantiate(sfxSourcePrefab, transform);
            src.playOnAwake = false;
            audioPool.Enqueue(src);
        }
    }

    /// <summary>
    /// Returns an available AudioSource from the pool.
    /// </summary>
    public AudioSource GetAvailableAudioSource()
    {
        if (audioPool.Count > 0)
        {
            AudioSource src = audioPool.Dequeue();
            return src;
        }
        else
        {
            // Optionally, instantiate a new one if the pool is empty.
            AudioSource src = Instantiate(sfxSourcePrefab, transform);
            return src;
        }
    }

    /// <summary>
    /// Returns an AudioSource back to the pool.
    /// </summary>
    public void ReturnAudioSource(AudioSource src)
    {
        audioPool.Enqueue(src);
    }

    /// <summary>
    /// Plays a sound effect clip at the specified position with the given volume.
    /// </summary>
    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
            return;

        AudioSource src = GetAvailableAudioSource();
        src.transform.position = position;
        src.volume = volume;
        src.PlayOneShot(clip);
        // Return the source to the pool after the clip's duration.
        StartCoroutine(ReturnSourceAfter(src, clip.length));
    }

    private IEnumerator ReturnSourceAfter(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnAudioSource(src);
    }
}
