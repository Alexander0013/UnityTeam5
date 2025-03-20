using UnityEngine;

public class Player1Audio : PlayerAudio
{
    [Header("Sound Clips")]
    public AudioClip idleClip;
    public AudioClip attackClip;
    public AudioClip getHitClip;
    public AudioClip dieClip;

    private AudioSource audioSource;
    public float idleSoundDelayMin = 10f;
    public float idleSoundDelayMax = 20f;
    private float idleTimer = 0f;
    private float nextIdleSoundTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        ResetIdleTimer();
    }
    public override void UpdateIdleTimer()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= nextIdleSoundTime)
        {
            PlayIdleSound();
            ResetIdleTimer();
        }
    }

    public override void ResetIdleTimer()
    {
        idleTimer = 0f;
        nextIdleSoundTime = Random.Range(idleSoundDelayMin, idleSoundDelayMax);
    }

    public override void PlayIdleSound()
    {
        if (idleClip != null)
            audioSource.PlayOneShot(idleClip);
    }

    public override void PlayAttackSound()
    {
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }

    public override void PlayGetHitSound()
    {
        if (getHitClip != null)
            audioSource.PlayOneShot(getHitClip);
    }

    public override void PlayDieSound()
    {
        if (dieClip != null)
            audioSource.PlayOneShot(dieClip);
    }
}
