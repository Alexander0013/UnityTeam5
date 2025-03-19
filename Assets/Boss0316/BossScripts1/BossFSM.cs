using UnityEngine;
using System.Collections;
using System.Xml.Linq;

public class BossFSM : MonoBehaviour
{
    // 狀態實例（請依需求取消註解或新增狀態）
    public BossIdleState idleState = new BossIdleState();
    public BossRoalingState roalingState = new BossRoalingState();
    public BossWalkState walkState = new BossWalkState();
    public BossStandByState standByState = new BossStandByState();
    public BossSwipingState swipingState = new BossSwipingState();
    public BossChargeState chargeState = new BossChargeState();

    [Header("Boss Settings")]
    public BossNPCStateData bossnpcData;
    public float detectionRadius = 10f;    // 偵測範圍
    public float attackRadius = 2f;        // 近戰攻擊範圍（可供 swiping 判斷使用）
    public LayerMask playerLayer;          // 玩家 Layer

    [Header("Boss Attack Points")]
    public Transform handHitPoint;         // 攻擊時判定中心（swiping 用）

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public BossBaseState currentState;

    // 計時器變數
    [HideInInspector] public float chargeUnlockTimer = 30f; // 30 秒後解鎖 Charge
    [HideInInspector] public float chargeCooldownTimer = 0f; // 15 秒冷卻
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void Start()
    {
        Debug.Log("開始執行 BossFSM，SnapToGround 並設定初始狀態");
        SnapToGround();
        StartCoroutine(ChargeUnlockCountdown()); // 開始 30 秒計時
        TransitionToState(idleState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
        // Charge 技能冷卻倒數
        if (chargeCooldownTimer > 0)
        {
            chargeCooldownTimer -= Time.deltaTime;
        }
    }

    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("無法偵測到地面，請確認地板 Collider 設定正確並在 'Ground' Layer");
        }
    }

    // 利用 OverlapSphere 偵測玩家
    public void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            Transform detectedPlayer = hits[0].transform;
            playerTarget = detectedPlayer;
            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // Idle 狀態下偵測到玩家進入範圍，進入 Roaling
            if (currentState is BossIdleState)
            {
                Debug.Log("玩家進入偵測範圍，Boss 進入 Roaling 狀態");
                TransitionToState(roalingState);
                return;
            }
        }
        else
        {
            // 玩家離開偵測範圍後回到 Idle
            if (playerTarget != null)
            {
                float distance = Vector3.Distance(transform.position, playerTarget.position);
                if (distance > detectionRadius * 1.5f)
                {
                    Debug.Log("玩家完全離開範圍，Boss 回到 Idle");
                    playerTarget = null;
                    TransitionToState(idleState);
                }
            }
        }
    }

    // 狀態切換
    public void TransitionToState(BossBaseState newState)
    {
        if (newState == null)
        {
            Debug.LogError("嘗試切換到 null 狀態！");
            return;
        }
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        Debug.Log("切換狀態：" + newState.GetType().Name);
        currentState.EnterState(this);
    }

    // 由 Roaling 動畫事件呼叫（在動畫剪輯末端添加事件，呼叫此方法）
    public void OnRoalingAnimationEnd()
    {
        Debug.Log("Roaling 動畫事件觸發");
        if (playerTarget != null)
        {
            TransitionToState(walkState);
        }
        else
        {
            TransitionToState(idleState);
        }
    }
    // 用於 Swiping 攻擊事件（透過動畫事件呼叫）
    public void AttackHitEvent()
    {
        if (currentState is BossSwipingState swipingState)
        {
            swipingState.OnAttackHit(this);
        }
    }
    // 透過此方法計算 swiping 攻擊傷害（事件判定）
    public void ApplyAttackDamage()
    {
        Debug.Log("Boss造成傷害");
        float damage = bossnpcData.baseDamage * bossnpcData.comboMultiplier;
        float radius = bossnpcData.hitRadius;
        Vector3 attackCenter = handHitPoint.position;
        Collider[] hits = Physics.OverlapSphere(attackCenter, radius, bossnpcData.playerLayers);
        foreach (Collider c in hits)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }
    // 用於 Swiping 動畫結束事件
    public void AttackAnimationEndEvent()
    {
        if (currentState is BossSwipingState swipingState)
        {
            Debug.Log("Swiping 動畫結束");
            swipingState.OnAttackAnimationFinished(this);
        }
    }

    // Charge 攻擊用：此方法可由 Charge 動畫事件呼叫，表示動畫結束
    public void OnChargeAnimationEnd()
    {
        Debug.Log("Charge 動畫事件觸發");
        TransitionToState(standByState);
    }
    // 在 Scene 視窗中劃出偵測範圍（方便調試）
    private void OnDrawGizmosSelected()
    {
        if (detectionRadius > 0)
        {
            //Debug.Log("劃出範圍");
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    // 30 秒倒數，解鎖 Charge
    public IEnumerator ChargeUnlockCountdown()
    {
        yield return new WaitForSeconds(30f);
        chargeUnlockTimer = 0f;
        Debug.Log("Charge 技能已解鎖！");
    }

    // 15 秒 Charge 技能冷卻
    public void StartChargeCooldown()
    {
        chargeCooldownTimer = 15f;
    }
}
