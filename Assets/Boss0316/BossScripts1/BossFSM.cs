using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossFSM : MonoBehaviour
{
    public BossIdleState idleState = new BossIdleState();
    public BossWalkState walkState = new BossWalkState();
    public BossCombatState combatState = new BossCombatState();
    public BossSwipingState swipingState = new BossSwipingState();
    public BossRoalingState roalingState = new BossRoalingState();
    public BossJumpAttackState jumpAttackState = new BossJumpAttackState(); 
    public BossGetHitState getHitState = new BossGetHitState();
    public BossDieState dieState = new BossDieState();


    [Header("Boss Settings")]
    public BossNPCStateData bossnpcData;
    public float detectionRadius =20f;    // 偵測範圍
    public float attackRadius = 2f;       // 攻擊範圍（也可當作 Swiping 攻擊範圍）
    public float swipingRange = 2f;       // Swiping 攻擊範圍
    public float jumpAttackCooldown = 5f;   // JumpAttack 冷卻時間
    public LayerMask playerLayer;           // 玩家 Layer

    [Header("Boss Attack Points")]
    public Transform handHitPoint;       // Swiping 攻擊範圍中心
    public Transform jumpHitPoint;   // Jump Attack 命中判定範圍中心

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public BossBaseState currentState;

    private float jumpAttackCooldownTimer = 0f; // 獨立管理 JumpAttack 冷卻時間
    private BossPhysicsHandler physicsHandler;
    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;  // 確保重力啟用
        }
        animator = GetComponent<Animator>();

        physicsHandler = GetComponent<BossPhysicsHandler>();
        if (physicsHandler == null)
        {
            Debug.LogError("BossFSM 找不到 BossPhysicsHandler，請確認有掛載該組件！");
        }
    }

    private void Start()
    {
        SnapToGround();
  
        TransitionToState(roalingState); // 進場時播放 Roaling
    }

    private void Update()
    {
        if (currentState == null) return;

        if (!(currentState is BossRoalingState))
        {
            DetectPlayer();
        }
        currentState.UpdateState(this);

        // 更新 JumpAttack 冷卻計時
        if (jumpAttackCooldownTimer > 0f)
        {
            jumpAttackCooldownTimer -= Time.deltaTime;
        }
    }
    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
        }
    }
    private void DetectPlayer()
    {
        if (playerTarget != null)
        {
            float distance = Vector3.Distance(transform.position, playerTarget.position);
        }
        else
        {
            Debug.LogWarning("playerTarget 為 null，無法計算距離");
        }
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hits.Length > 0)
        {
            Transform detectedPlayer = hits[0].transform;
            playerTarget = detectedPlayer;

            float distance = Vector3.Distance(transform.position, playerTarget.position);

            if (distance > attackRadius && distance < detectionRadius && IsJumpAttackReady())
            {
                Debug.Log("玩家在 JumpAttack 範圍內，進行跳躍攻擊");
                TransitionToState(jumpAttackState);
                return;
            }

            if (distance > attackRadius)
            {
                if (!(currentState is BossWalkState))
                {
                    Debug.Log("玩家在偵測範圍內，Boss 開始追擊");
                    TransitionToState(walkState);
                }
            }
            else
            {
                Debug.Log("玩家進入攻擊範圍，Boss 進入戰鬥狀態");
                TransitionToState(combatState);
            }
        }
        else
        {
            if (playerTarget != null)
            {
                float distance = Vector3.Distance(transform.position, playerTarget.position);
                if (distance > detectionRadius * 1.5f)
                {
                    Debug.Log("玩家完全消失，Boss 回到 Idle");
                    playerTarget = null;
                    TransitionToState(idleState);
                }
                else
                {
                    Debug.Log("玩家暫時超出偵測範圍，但 Boss 仍在追擊");
                }
            }
        }
    }


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
    public void TakeDamage(float damage)
    {
        Debug.Log("Boss 損失了 " + damage + " 點生命");
        if (currentState is BossDieState)
            return; // 死亡狀態時不受傷害

        bossnpcData.maxHealth -= damage;

        if (bossnpcData.maxHealth <= 0)
        {
            TransitionToState(dieState);
        }
        else
        {
            animator.SetTrigger("getHit"); // 讓 AnyState 直接播放受擊動畫
            TransitionToState(getHitState);
        }
    }


    public void OnRoalingAnimationEnd()
    {
        if (currentState is BossRoalingState)
        {
            TransitionToState(idleState);
        }
    }

    public void OnIdleAnimationEnd()
    {
        if (currentState is BossIdleState)
        {
            Debug.Log("Idle 動畫結束，切換到 Walk 狀態");
            animator.SetBool("Walk", true);
            TransitionToState(walkState);
        }
    }
  

    public void ApplyAttackDamage()
    {
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



    public void AttackHitEvent()
    {
        // 如果需要在動畫中某個時刻確認是否命中，可以用這個方法
        if (currentState is BossSwipingState swipingState)
        {
            swipingState.OnAttackHit(this);
        }
    }
   
    public void AttackAnimationEndEvent()
    {
        // 在動畫結束時，先嘗試施加傷害
        ApplyAttackDamage();

        // 再通知攻擊狀態動畫已經結束
        if (currentState is BossSwipingState swipingState)
        {
            Debug.Log("Leaving EnemyAttack State");
            swipingState.OnAttackAnimationFinished(this);
        }
    }
    public void ResetJumpAttackCooldown()
    {
        jumpAttackCooldownTimer = jumpAttackCooldown;
    }

    public bool IsJumpAttackReady()
    {
        return jumpAttackCooldownTimer <= 0f;
    }
    public void ApplyJumpDamage()
    {
        float damage = bossnpcData.baseDamage * bossnpcData.comboMultiplier;
        float radius = bossnpcData.hitRadius;
        Vector3 attackCenter = jumpHitPoint.position;
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
    public void JumpHitEvent()
    {
        // 如果需要在動畫中某個時刻確認是否命中，可以用這個方法
        if (currentState is BossJumpAttackState jumpAttackState)
        {
            jumpAttackState.OnJumpHit(this);
        }
    }
    public void JumpAnimationEndEvent()
    {
        // 在動畫結束時，先嘗試施加傷害
        ApplyJumpDamage();

        // 再通知攻擊狀態動畫已經結束
        if (currentState is BossJumpAttackState jumpAttackState)
        {
            Debug.Log("Leaving EnemyAttack State");
            jumpAttackState.OnJumpAnimationFinished(this);
        }
    }
}