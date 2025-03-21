using UnityEngine;

public abstract class PlayerAudio : MonoBehaviour
{
    public abstract void PlayIdleSound();
    public abstract void PlayAttackSound();
    public abstract void PlayGetHitSound();
    public abstract void PlayDieSound();
    public abstract void UpdateIdleTimer();
    public abstract void ResetIdleTimer();
}
