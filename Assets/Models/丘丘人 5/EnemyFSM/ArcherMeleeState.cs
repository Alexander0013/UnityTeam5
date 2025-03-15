using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherMeleeState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherMeleeState] Entering Melee Attack");
        enemy.animator.SetBool("isAttacking", true);
        // Trigger the melee kick animation (ensure your Animator has a "MeleeKick" trigger).
        enemy.animator.SetTrigger("MeleeKick");
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // No update logic needed; timing is handled via Animation Events.
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }
}

