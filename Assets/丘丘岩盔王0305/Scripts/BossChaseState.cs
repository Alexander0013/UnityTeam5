using UnityEngine;

public class BossChaseState : BossBaseState
{
    private float currentSpeed = 0f;
    private float smoothVelocity = 0f;

    public override void EnterState(BossFSM boss)
    {
        Debug.Log("Boss 進入 Chase (Walk) 狀態");
        if (boss.animator != null)
        {
            boss.animator.SetFloat("Speed", 0f);
            int layerIndex = boss.animator.GetLayerIndex("moveLayer");
            boss.animator.Play("Walk", layerIndex);
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        if (boss.playerTarget == null)
        {
            boss.TransitionToState(boss.idleState);
            return;
        }

        Vector3 direction = (boss.playerTarget.position - boss.transform.position).normalized;
        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        // 使用 SmoothDamp 平滑控制移動速度
        currentSpeed = Mathf.SmoothDamp(currentSpeed, boss.speed, ref smoothVelocity, 0.1f);
        float step = currentSpeed * Time.deltaTime;
        Vector3 newPos = Vector3.MoveTowards(boss.transform.position, boss.playerTarget.position, step);
        boss.transform.position = newPos;

        // 平滑轉向
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // 更新動畫參數，讓 Blend Tree 自然切換 Idle/Walk
        if (boss.animator != null)
        {
            boss.animator.SetFloat("Speed", step);
        }

        // 當進入攻擊範圍時切換到 Attack 狀態
        if (distance <= boss.attackRadius)
        {
            boss.TransitionToState(boss.attackState);
        }
        // 使用目前的移動速度來更新動畫參數，確保 Blend Tree 持續播放 Walk 動畫
        if (boss.animator != null)
        {
            boss.animator.SetFloat("Speed", currentSpeed);
        }

    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Chase (Walk) 狀態");
        if (boss.animator != null)
        {
            boss.animator.SetFloat("Speed", 0f);
        }
    }
}
