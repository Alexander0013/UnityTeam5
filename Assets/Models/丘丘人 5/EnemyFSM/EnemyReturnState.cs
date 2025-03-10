using UnityEngine;

public class EnemyReturnState : EnemyBaseState
{
    private float returnSpeed = 2f;
    private float returnRadius = 3f; // The distance from treasure you want
    private Vector3 targetPoint;
    private bool hasTargetPoint = false;

    public override void EnterState(EnemyFSM enemy)
    {
        Debug.Log("Enter Return State");
        enemy.animator.SetBool("isWalking", true);

        // Find the treasure by checking our parent
        Transform treasure = enemy.transform.parent;
        if (treasure == null)
        {
            Debug.LogWarning("Enemy has no parent to return to!");
            hasTargetPoint = false;
            return;
        }

        // If you prefer to use enemy.treasureReturnRadius, you can
        // but here let's just use "returnRadius = 1f"
        Vector2 randOffset = Random.insideUnitCircle * returnRadius;
        targetPoint = treasure.position + new Vector3(randOffset.x, 0, randOffset.y);
        hasTargetPoint = true;
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        if (!hasTargetPoint || enemy.isDead) return;

        MoveToPoint(enemy);

        float dist = Vector3.Distance(enemy.transform.position, targetPoint);
        if (dist < 0.2f)
        {
            // Once close enough, transition to idle.
            enemy.TransitionToState(enemy.idleState);
            // After arriving, rotate to face opposite the treasure.
            Vector3 awayDirection = (enemy.transform.position - enemy.transform.parent.position).normalized;
            if (awayDirection != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(awayDirection);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, desiredRotation, Time.deltaTime * 5f);
            }
        }
    }


    public override void ExitState(EnemyFSM enemy)
    {
        enemy.animator.SetBool("isWalking", false);
        hasTargetPoint = false;
    }

    private void MoveToPoint(EnemyFSM enemy)
    {
        Vector3 direction = (targetPoint - enemy.transform.position).normalized;

        // Rotate toward target
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation, 
                rot, 
                Time.deltaTime * 5f
            );
        }

        // Move toward the target
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            targetPoint,
            returnSpeed * Time.deltaTime
        );

        //Rotate to face opposite side of target

    }
}
