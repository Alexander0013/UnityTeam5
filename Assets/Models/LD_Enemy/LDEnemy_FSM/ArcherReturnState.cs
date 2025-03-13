using UnityEngine;

public class ArcherReturnState : EnemyBaseState
{
    private float returnSpeed = 2f;
    private Vector3 targetPoint;
    private bool hasTargetPoint = false;

    public override void EnterState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        Debug.Log("[ArcherReturnState] Entering Return State");
        if (archer.animator != null)
            archer.animator.SetBool("isWalking", true);

        // Assume the archer’s parent is the treasure.
        Transform treasure = archer.transform.parent;
        if (treasure != null)
        {
            Vector2 randOffset = Random.insideUnitCircle * archer.treasureReturnRadius;
            targetPoint = treasure.position + new Vector3(randOffset.x, 0f, randOffset.y);
            hasTargetPoint = true;
        }
        else
        {
            hasTargetPoint = false;
        }
    }

    public override void UpdateState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null || archer.isDead || !hasTargetPoint) return;
        Vector3 direction = (targetPoint - archer.transform.position).normalized;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            archer.transform.rotation = Quaternion.Slerp(archer.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        archer.transform.position = Vector3.MoveTowards(archer.transform.position, targetPoint, returnSpeed * Time.deltaTime);
        if (Vector3.Distance(archer.transform.position, targetPoint) < 0.2f)
        {
            archer.TransitionToState(archer.idleState);
        }
    }

    public override void ExitState(EnemyFSM enemy)
    {
        ArcherFSM archer = enemy as ArcherFSM;
        if (archer == null) return;
        if (archer.animator != null)
            archer.animator.SetBool("isWalking", false);
        hasTargetPoint = false;
    }
}
