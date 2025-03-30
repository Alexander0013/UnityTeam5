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
    public BossDanceState danceState = new BossDanceState();
    //public BossSummonEnemyState summonEnemyState = new BossSummonEnemyState();

    [Header("Boss Settings")]
    public BossNPCStateData bossnpcData;
    public float detectionRadius = 15f;    // �����d��
    public float attackRadius = 2.5f;        // ��ԧ����d��]�i�� swiping �P�_�ϥΡ^
    public LayerMask playerLayer;          // ���a Layer
    public Vector3 chargeDestination;      // �Ĩ�ت��a�]�b Idle/ChargeIdle �ɨM�w�^
    // �l��ĤH�ޯ઺�N�o�ɶ��]���^
    //public float summonEnemyCooldown = 30f;
    //private float currentSummonCooldown = 0f;
    [Header("Boss Attack Points")]
    public Transform handHitPoint;         // �����ɧP�w���ߡ]swiping �Ρ^
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
        Debug.Log("�}�l���� BossFSM�ASnapToGround �ó]�w��l���A");
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
        // �p�G�ؼЪ��a�s�b���w���`�A�h������ DanceState
        if (playerTarget != null && !IsPlayerAlive(playerTarget))
        {
            Debug.Log("���a���`�ABoss �i�J Dance ���A");
            TransitionToState(danceState);
        }
        if (currentState is BossRoalingState)
        {
            BossHealthBar bossBar = FindObjectOfType<BossHealthBar>();
            if (bossBar != null)
            {
                // Start the fade-in coroutine to set alpha = 1
                bossBar.StartCoroutine(bossBar.FadeOutHealthBar(1f));
            }
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

    // �Q�� OverlapSphere �������a
    public void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            Transform detectedPlayer = hits[0].transform;
            playerTarget = detectedPlayer;
            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // Idle ���A�U�����쪱�a�i�J�d��A�i�J Roaling
            if (currentState is BossIdleState)
            {
                Debug.Log("���a�i�J�����d��ABoss �i�J Roaling ���A");
                TransitionToState(roalingState);
                return;
            }
        }
        //else
        //{
        //    // ���a���}�����d���^�� Idle
        //    if (playerTarget != null)
        //    {
        //        float distance = Vector3.Distance(transform.position, playerTarget.position);
        //        if (distance > detectionRadius * 1.5f)
        //        {
        //            Debug.Log("���a�������}�d��ABoss �^�� Idle");
        //            playerTarget = null;
        //            TransitionToState(idleState);
        //        }
        //    }
        //}
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
        if (currentState is BossRoalingState)
        {
            AudioManager.instance.TriggerBattleMusic();
        }
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
        PLayAttackSound();
        Debug.Log("Boss�y���ˮ`");
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
                GameObject effect = Instantiate(hitEffectPrefab, effectPosition, Quaternion.identity);
                Destroy(effect,1f);
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
        GetComponent<BossAudio>()?.PlayRoalingSound();
    }
    public void PLayDieSound()
    {
        GetComponent<PlayerAudio>()?.PlayDieSound();
    }
    public void PLayAttackSound()
    {
        GetComponent<PlayerAudio>()?.PlayAttackSound();
    }
    public void PLayGetHitSound()
    {
        GetComponent<PlayerAudio>()?.PlayGetHitSound();
    }
}
