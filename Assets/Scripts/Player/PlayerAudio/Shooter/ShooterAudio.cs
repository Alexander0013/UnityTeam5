using UnityEngine;

public class ShooterAudio : PlayerAudio
{
    [Header("Sound Clips")]
    public AudioClip attackClip;
    public AudioClip legKickClip;
    public AudioClip getHitClip;
    public AudioClip dieClip;

    [Header("Cooldown Settings")]
    public float attackSoundCooldown = 3f;
    private float lastAttackSoundTime = -100f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Get or add an AudioSource component.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure for 3D sound.
        audioSource.spatialBlend = 1.0f;
    }

    public override void PlayAttackSound()
    {
        if (Time.time - lastAttackSoundTime >= attackSoundCooldown)
        {
            if (attackClip != null)
            {
                SFXManager.instance.PlaySFX(attackClip, transform.position);
            }
        }
    }
    public void PlayLegKickSound()
    {
        if (legKickClip != null)
        {
            SFXManager.instance.PlaySFX(legKickClip, transform.position);
        }
    }

    public override void PlayGetHitSound()
    {
        if (getHitClip != null)
            SFXManager.instance.PlaySFX(getHitClip, transform.position);
    }

    public override void PlayDieSound()
    {
        if (dieClip != null)
            SFXManager.instance.PlaySFX(dieClip, transform.position);
    }

    // For idle sound methods, if not used by the shooter:
    public override void PlayIdleSound() { }
    public override void UpdateIdleTimer() { }
    public override void ResetIdleTimer() { }
}
