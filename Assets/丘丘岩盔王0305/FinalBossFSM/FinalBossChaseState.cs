using UnityEngine;

public class BossChaseState : EnemyBaseState
{
    private float chaseSpeed = 5f;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("[BossChaseState] Entering Chase");
        enemy.animator.SetBool("isWalking", true);
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (enemy.playerTarget == null) return;
        
        // Move towards the player
        Vector3 targetPos = enemy.playerTarget.position;
        targetPos.y = enemy.transform.position.y;
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPos, chaseSpeed * Time.deltaTime);

        // Rotate towards the player
        Vector3 direction = (targetPos - enemy.transform.position).normalized;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // If within attack range, return to idle to decide an attack.
        float distance = Vector3.Distance(enemy.transform.position, enemy.playerTarget.position);
        if (distance <= enemy.attackRadius)
        {
            enemy.TransitionToState(((FinalBossFSM)enemy).idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isWalking", false);
    }
}
