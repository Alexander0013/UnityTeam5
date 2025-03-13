using UnityEngine;

public class ArcherShootState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        Debug.Log("[ArcherShootState] Entering Shoot State");
        // Reset the shoot timer so that the cooldown restarts after shooting.
        archer.shootTimer = 0f;
        // Trigger the shooting animation.
        if (archer.animator != null)
        {
            archer.animator.SetTrigger("ShootArrow");
            // Also set a flag so the animator knows we're not in idle.
            archer.animator.SetBool("isShooting", true);
        }
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null || archer.isDead) return;

        // Assume that the shoot animation is non-looping.
        // We check the normalized time of the shoot animation.
        AnimatorStateInfo stateInfo = archer.animator.GetCurrentAnimatorStateInfo(0);
        // "Shoot" should be the name of your shooting animation state.
        if (stateInfo.IsName("Shoot") && stateInfo.normalizedTime >= 1f)
        {
            // Shooting animation finished, return to Idle (aiming) state.
            archer.TransitionToState(archer.idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        // Clear the shooting flag.
        archer.animator.SetBool("isShooting", false);
    }
}
