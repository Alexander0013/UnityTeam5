public abstract class BossBaseState
{
    public abstract void EnterState(BossFSM boss);
    public abstract void UpdateState(BossFSM boss);
    public abstract void ExitState(BossFSM boss);
}