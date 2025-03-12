using UnityEngine;

public class BossReturnState : BossBaseState
{
    private float returnSpeed = 2f;
    private float returnRadius = 3f; // The distance from treasure you want
    private Vector3 targetPoint;
    private bool hasTargetPoint = false;

    public override void EnterState(BossFSM boss)
    {
        //Debug.Log("Enter Return State");
       boss.animator.SetBool("isWalking", true);

        // Find the treasure by checking our parent
        Transform treasure = boss.transform.parent;
        if (treasure == null)
        {
            //Debug.LogWarning("boss has no parent to return to!");
            hasTargetPoint = false;
            return;
        }

        // If you prefer to use boss.treasureReturnRadius, you can
        // but here let's just use "returnRadius = 1f"
        Vector2 randOffset = Random.insideUnitCircle * returnRadius;
        targetPoint = treasure.position + new Vector3(randOffset.x, 0, randOffset.y);
        hasTargetPoint = true;
    }

    public override void UpdateState(BossFSM boss)
    {
        if (!hasTargetPoint || boss.isDead) return;

        MoveToPoint(boss);

        float dist = Vector3.Distance(boss.transform.position, targetPoint);
        if (dist < 0.2f)
        {
            // Once close enough, transition to idle.
            boss.TransitionToState(boss.idleState);
            // After arriving, rotate to face opposite the treasure.
            Vector3 awayDirection = (boss.transform.position - boss.transform.parent.position).normalized;
            if (awayDirection != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(awayDirection);
                boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, desiredRotation, Time.deltaTime * 5f);
            }
        }
    }


    public override void ExitState(BossFSM boss)
    {
        boss.animator.SetBool("isWalking", false);
        hasTargetPoint = false;
    }

    private void MoveToPoint(BossFSM boss)
    {
        Vector3 direction = (targetPoint - boss.transform.position).normalized;

        // Rotate toward target
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            boss.transform.rotation = Quaternion.Slerp(
                boss.transform.rotation, 
                rot, 
                Time.deltaTime * 5f
            );
        }

        // Move toward the target
        boss.transform.position = Vector3.MoveTowards(
            boss.transform.position,
            targetPoint,
            returnSpeed * Time.deltaTime
        );

        //Rotate to face opposite side of target

    }
}
