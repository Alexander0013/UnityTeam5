using UnityEngine;

public class ArcherShootState : EnemyBaseState
{
    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[ArcherShootState] Entering Ranged Attack");
        // Face the player immediately.
        if(enemy.playerTarget != null)
        {
            Vector3 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
            direction.y = 0f;
            enemy.transform.rotation = Quaternion.LookRotation(direction);
        }
        enemy.animator.SetBool("isAttacking", true);
        // Trigger the Shoot animation (ensure your Animator has a trigger named "Shoot").
        enemy.animator.SetTrigger("Shoot");
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        // Optionally, continuously update rotation to face the player.
        if(enemy.playerTarget != null)
        {
            Vector3 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isAttacking", false);
    }
}
