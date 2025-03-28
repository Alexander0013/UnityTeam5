using UnityEngine;

public class BossAudio : PlayerAudio
{
    //public float attackSoundCooldown = 1f;
    //private float lastAttackSoundTime = -100f;
    [Header("Sound Clips")]
    public AudioClip attackClip;
    public AudioClip getHitClip;
    public AudioClip dieClip;
    public AudioClip roalingClip;

    // We remove the local AudioSource playback since we use SFXManager now.

    public override void PlayAttackSound()
    {
        if (attackClip != null)
        {
            SFXManager.instance.PlaySFX(attackClip, transform.position);
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
    public void PlayRoalingSound()
    {
        if (attackClip != null)
            SFXManager.instance.PlaySFX(roalingClip, transform.position);
    }
    // For idle sounds, if enemies have them, you could implement empty methods or similar.
    public override void UpdateIdleTimer() { }
    public override void ResetIdleTimer() { }
    public override void PlayIdleSound() { }
}

