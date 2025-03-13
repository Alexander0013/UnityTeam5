using UnityEngine;

public class ArcherDeadState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        Debug.Log("[ArcherDeadState] Archer is Dead");
        if (archer.animator != null)
            archer.animator.SetTrigger("Die");

        Collider col = archer.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Object.Destroy(archer.gameObject, 3f);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // No update logic once dead.
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Not needed.
    }
}
