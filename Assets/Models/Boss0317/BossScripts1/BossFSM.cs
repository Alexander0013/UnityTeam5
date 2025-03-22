using UnityEngine;
using System.Collections;
using System.Xml.Linq;

public class BossFSM : MonoBehaviour
{
    // ���A��ҡ]�Ш̻ݨD�������ѩηs�W���A�^
    public BossIdleState idleState = new BossIdleState();
    public BossRoalingState roalingState = new BossRoalingState();
    public BossWalkState walkState = new BossWalkState();
    public BossSwipingState swipingState = new BossSwipingState();
    public BossChargeState chargeState = new BossChargeState();
    public BossChargeIdleState chargeIdleState = new BossChargeIdleState();
   
    [Header("Boss Settings")]
    public BossNPCStateData bossnpcData;
    public float detectionRadius = 10f;    // �����d��
    public float attackRadius = 2f;        // ��ԧ����d��]�i�� swiping �P�_�ϥΡ^
    public LayerMask playerLayer;          // ���a Layer

    [Header("Boss Attack Points")]
    public Transform handHitPoint;         // �����ɧP�w���ߡ]swiping �Ρ^

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public BossBaseState currentState;

    // �p�ɾ��ܼ�
    [HideInInspector] public float chargeUnlockTimer = 30f; // 30 ������� Charge
    [HideInInspector] public float chargeCooldownTimer = 15f; // 15 ���N�o
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
        playerTarget = FindActiveLivingPlayer();
        SnapToGround();
        StartCoroutine(ChargeUnlockCountdown());
        TransitionToState(idleState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }

        StartCoroutine(CheckForPlayerRoutine());
        // Charge �ޯ�N�o�˼�
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
            Debug.LogWarning("�L�k������a���A�нT�{�a�O Collider �]�w���T�æb 'Ground' Layer");
        }
    }
    protected Transform FindActiveLivingPlayer()
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
    protected bool IsPlayerAlive(Transform playerTransform)
    {
        PlayerHealth ph = playerTransform.GetComponent<PlayerHealth>();
        return ph != null && ph.CurrentHealth > 0;
    }
    private IEnumerator CheckForPlayerRoutine()
    {
        while (true)
        {
            DetectPlayer();
            yield return new WaitForSeconds(1f);
        }
    }
    public void DetectPlayer()
    {
        Transform candidate = FindActiveLivingPlayer();
        if (candidate != null)
        {
            float distance = Vector3.Distance(transform.position, candidate.position);
            // If the candidate is within detection range...
            if (distance <= detectionRadius)
            {
                if (playerTarget == null)
                {
                    playerTarget = candidate;
                    // If currently idle, transition to Roaling state.
                    if (currentState is BossIdleState)
                    {
                        Debug.Log("Player detected in range. Transitioning Boss to Roaling state.");
                        TransitionToState(roalingState);
                    }
                }
            }
            else
            {
                // If candidate is too far away, clear playerTarget and return to idle.
                if (playerTarget != null && distance > detectionRadius * 1.5f)
                {
                    Debug.Log("Player is out of range. Boss returning to Idle state.");
                    playerTarget = null;
                    TransitionToState(idleState);
                }
            }
        }
        else
        {
            // If no active living player is found, ensure playerTarget is null.
            if (playerTarget != null)
            {
                playerTarget = null;
                TransitionToState(idleState);
            }
        }
    }

    // ���A����
    public void TransitionToState(BossBaseState newState)
    {
        if (newState == null)
        {
            Debug.LogError("���դ����� null ���A�I");
            return;
        }
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        Debug.Log("�������A�G" + newState.GetType().Name);
        currentState.EnterState(this);
    }

    // �� Roaling �ʵe�ƥ�I�s�]�b�ʵe�ſ襽�ݲK�[�ƥ�A�I�s����k�^
    public void OnRoalingAnimationEnd()
    {
        Debug.Log("Roaling �ʵe�ƥ�Ĳ�o");
        if (playerTarget != null)
        {
            TransitionToState(walkState);
        }
        else
        {
            TransitionToState(idleState);
        }
    }
    // �Ω� Swiping �����ƥ�]�z�L�ʵe�ƥ�I�s�^
    public void AttackHitEvent()
    {
        if (currentState is BossSwipingState swipingState)
        {
            swipingState.OnAttackHit(this);
        }
    }
    // �z�L����k�p�� swiping �����ˮ`�]�ƥ�P�w�^
    public void ApplyAttackDamage()
    {
        Debug.Log("Boss�y���ˮ`");
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
    // �Ω� Swiping �ʵe�����ƥ�
    public void AttackAnimationEndEvent()
    {
        if (currentState is BossSwipingState swipingState)
        {
            Debug.Log("Swiping �ʵe����");
            swipingState.OnAttackAnimationFinished(this);
        }
    }

    // Charge �����ΡG����k�i�� Charge �ʵe�ƥ�I�s�A���ܰʵe����
    public void OnChargeAnimationEnd()
    {
        Debug.Log("Charge �ʵe�ƥ�Ĳ�o");
        TransitionToState(walkState);
    }
    // �b Scene ���������X�����d��]��K�ոա^
    private void OnDrawGizmosSelected()
    {
        if (detectionRadius > 0)
        {
            //Debug.Log("���X�d��");
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    // 30 ���˼ơA���� Charge
    public IEnumerator ChargeUnlockCountdown()
    {
        yield return new WaitForSeconds(10f);
        chargeUnlockTimer = 0f;
        Debug.Log("Charge �ޯ�w����I");
    }

    // 15 �� Charge �ޯ�N�o
    public void StartChargeCooldown()
    {
        chargeCooldownTimer = 15f;
    }
    public void OnChargeIdleAnimationEnd()
    {
        if (currentState is BossChargeIdleState chargeIdleState)
        {
            chargeIdleState.OnChargeIdleAnimationEnd(this);
        }
    }
}
