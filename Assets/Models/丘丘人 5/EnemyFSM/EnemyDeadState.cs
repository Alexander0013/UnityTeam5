using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        //Debug.Log(enemy.name + " is Dead");
        enemy.animator.SetTrigger("Die");
        // Optionally, remove collider or other components
        Collider col = enemy.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destroy after some time or after the death animation
        Object.Destroy(enemy.gameObject, 2f);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // No update logic needed in dead state
    }

    public override void ExitState(EnemyFSM enemy)
    {
        // Not used in dead state
    }
}
