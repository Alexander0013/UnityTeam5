using UnityEngine;
using System.Collections;
using System.Xml.Linq;

public class BossFSM : MonoBehaviour
{
    // 狀態實例（請依需求取消註解或新增狀態）
    public BossIdleState idleState = new BossIdleState();
    public BossRoalingState roalingState = new BossRoalingState();
    public BossWalkState walkState = new BossWalkState();
    public BossSwipingState swipingState = new BossSwipingState();
    public BossDanceState danceState = new BossDanceState();
    //public BossSummonEnemyState summonEnemyState = new BossSummonEnemyState();

    [Header("Boss Settings")]
    public BossNPCStateData bossnpcData;
    public float detectionRadius = 15f;    // 偵測範圍
    public float attackRadius = 2.5f;        // 近戰攻擊範圍（可供 swiping 判斷使用）
    public LayerMask playerLayer;          // 玩家 Layer
    public Vector3 chargeDestination;      // 衝刺目的地（在 Idle/ChargeIdle 時決定）
    // 召喚敵人技能的冷卻時間（秒）
    //public float summonEnemyCooldown = 30f;
    //private float currentSummonCooldown = 0f;
    [Header("Boss Attack Points")]
    public Transform handHitPoint;         // 攻擊時判定中心（swiping 用）
    public GameObject hitEffectPrefab;
   

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public BossBaseState currentState;

 
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
        // Attempt an initial player find
        playerTarget = FindActiveLivingPlayer();
        TransitionToState(idleState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
        // 如果目標玩家存在但已死亡，則切換到 DanceState
        if (playerTarget != null && !IsPlayerAlive(playerTarget))
        {
            Debug.Log("玩家死亡，Boss 進入 Dance 狀態");
            TransitionToState(danceState);
        }

        //// 更新召喚技能冷卻
        //if (currentSummonCooldown > 0f)
        //{
        //    currentSummonCooldown -= Time.deltaTime;
        //}
        //else
        //{
        //    // 當冷卻結束且 Boss 處於 Idle 狀態時，優先使用召喚技能
        //    if (currentState is BossIdleState)
        //    {
        //        Debug.Log("召喚技能冷卻完畢，切換到 SummonEnemy 狀態");
        //        TransitionToState(summonEnemyState);
        //        currentSummonCooldown = summonEnemyCooldown; // 重置冷卻
        //    }
        //}
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
        //else
        //{
        //    // 玩家離開偵測範圍後回到 Idle
        //    if (playerTarget != null)
        //    {
        //        float distance = Vector3.Distance(transform.position, playerTarget.position);
        //        if (distance > detectionRadius * 1.5f)
        //        {
        //            Debug.Log("玩家完全離開範圍，Boss 回到 Idle");
        //            playerTarget = null;
        //            TransitionToState(idleState);
        //        }
        //    }
        //}
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
        if (currentState is BossRoalingState)
        {
            AudioManager.instance.TriggerBattleMusic();
        }
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
        Vector3 effectPosition = handHitPoint.position + Vector3.down * 0.3f+Vector3.left*1.5f;
        Collider[] hits = Physics.OverlapSphere(attackCenter, radius, bossnpcData.playerLayers);
        foreach (Collider c in hits)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, effectPosition, Quaternion.identity);
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

    private Transform FindActiveLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy && IsPlayerAlive(p.transform))
            {
                return p.transform;
            }
        }
        return null;
    }

    private bool IsPlayerAlive(Transform playerTransform)
    {
        // Check the player's HP
        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        if (ph == null) return false;
        return (ph.CurrentHealth > 0);
    }
    public void PLayRoalingSound()
    {
        GetComponent<PlayerAudio>()?.PlayAttackSound();
    }
    public void PLayDieSound()
    {
        GetComponent<PlayerAudio>()?.PlayDieSound();
    }
}
