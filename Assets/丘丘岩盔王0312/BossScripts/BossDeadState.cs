using UnityEngine;

public class BossDeadState : BossBaseState
{
    public override void EnterState(BossFSM boss)
    {
        //Debug.Log(boss.name + " is Dead");
        boss.animator.SetTrigger("Die");
        // Optionally, remove collider or other components
        Collider col = boss.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destroy after some time or after the death animation
        Object.Destroy(boss.gameObject, 2f);
    }

    public override void UpdateState(BossFSM boss)
    {
        // No update logic needed in dead state
    }

    public override void ExitState(BossFSM boss)
    {
        // Not used in dead state
    }
}
