using UnityEngine;

public class BossFSM : MonoBehaviour
{
    // 狀態實例（建議分別放在不同檔案中）
    public BossIdleState idleState = new BossIdleState();
    public BossRoalingState roalingState = new BossRoalingState();
    public BossChaseState chaseState = new BossChaseState();
    public BossAttackState attackState = new BossAttackState();
    // public BossSpecialAttackState specialAttackState = new BossSpecialAttackState(); // 略

    [Header("Boss Settings")]
    public float health = 100f;
    public float speed = 5f;
    public float detectionRadius = 10f;   // 偵測範圍
    public float attackRadius = 2f;       // 攻擊範圍
    public float idleDuration = 2f;       // Idle 狀態持續時間
    public float roalingDuration = 3f;    // 若動畫資訊失效時的備用時間
    public float attackDuration = 1f;     // 攻擊持續時間

    [Header("Special Attack Settings")]
    public float specialAttackCooldown = 10f; // 特殊攻擊冷卻 10 秒
    [HideInInspector] public float specialAttackTimer = 0f;

    public LayerMask playerLayer;         // 玩家所在 Layer
    public Animator animator;             // 請在 Inspector 指派 Animator
    public Transform playerTarget;        // 玩家目標

    [HideInInspector] public BossBaseState currentState;

    private void Start()
    {
        // 嘗試找玩家（若尚未指派）
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }
        // 初始進入 Idle 狀態
        TransitionToState(idleState);
    }

    private void Update()
    {
        // 累加特殊攻擊計時器（如有特殊攻擊需求）
        specialAttackTimer += Time.deltaTime;

        // 持續更新玩家目標（全局偵測，但不直接切換狀態）
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }
        else
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            if (hits.Length > 0)
            {
                playerTarget = hits[0].transform;
            }
            // 若偵測不到則可根據需求清空 playerTarget
        }

        // 呼叫當前狀態的 Update 方法（請確保只呼叫一次）
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }

        if (playerTarget != null)
        {
            // 計算 Boss 與玩家之間的方向，忽略 Y 軸
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // 使用 Slerp 平滑旋轉，turnSpeed 為旋轉速度，可以在 Inspector 中設定
                float turnSpeed = 5f;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

    }

    public void TransitionToState(BossBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }
}
