using UnityEngine;

public class BossIdleState : BossBaseState
{
    private float timer;

    public override void EnterState(BossFSM boss)
    {
        timer = 0f;
        Debug.Log("Boss 進入 Idle 狀態");
        if (boss.animator != null)
        {
            int layerIndex = boss.animator.GetLayerIndex("moveLayer");
            boss.animator.Play("Idle", layerIndex);
        }
    }

    public override void UpdateState(BossFSM boss)
    {
        timer += Time.deltaTime;
        if (timer >= boss.idleDuration)
        {
            // Idle 結束後進入 Roaling 狀態
            boss.TransitionToState(boss.roalingState);
        }
    }

    public override void ExitState(BossFSM boss)
    {
        Debug.Log("Boss 離開 Idle 狀態");
    }
}

