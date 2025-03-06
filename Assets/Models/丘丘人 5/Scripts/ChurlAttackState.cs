using System.Collections;
using UnityEngine;

public class ChurlAttackState : ChurlBase
{
    // Assign these in the Inspector:
    public Transform attackHitPoint;  // Place this at the tip of the enemy's weapon or appropriate hit area.
    public NPCStateData npcStateData;   // Holds enemy attack values and player layer mask.
    private float attackInterval = 1f;  // �������j1��
    private float attackRange = 1f;     // �P���ު��A�������d��ۦP
    private Coroutine attackCoroutine;

    public override void Enter()
    {
        if (churl == null)
        {
            churl = GetComponent<Churl>();
        }
        if (churl != null)
        {
            churlObject = churl.gameObject;
        }
        if (animator == null)
        {
            animator = churlObject.GetComponent<Animator>();
        }
        // ���� Animator �� attackLayer�]���] attackLayer �w�b Animator ���]�w�^
        SetAnimatorLayerWeight("attackLayer", 1);
        SetAnimatorLayerWeight("walkLayer", 0);
        //SetAnimatorLayerWeight("deathLayer", 0);
        Debug.Log("�i�J�������A");
        attackCoroutine = churl.StartCoroutine(AttackRoutine());
    }

    public override void Update()
    {
        Debug.Log("CASUpdate");
        if (churl == null) return;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && churlObject != null)
        {
            float distance = Vector3.Distance(churlObject.transform.position, player.transform.position);
            // �Y���a�]�X�����d��A�����^���ު��A
            if (distance > attackRange)
            {
                Debug.Log("changeToPatrol");
                churl.ChangeState(new ChurlPatrolState());
            }
        }
    }


    public override void Exit()
    {
        Debug.Log("���}�������A");
        SetAnimatorLayerWeight("attackLayer", 0);
        if (attackCoroutine != null)
        {
            churl.StopCoroutine(attackCoroutine);
            attackCoroutine = null;  // �T�O������{�Q�M�šA�קK�ª���{�v�T�s���A
        }
    }

    private IEnumerator AttackRoutine()
    {
        if (true)
        {
            Debug.Log("�������a�I");
            // �o��Ĳ�o�����ʵe�A�i�H�� animator.SetTrigger("Attack")
            animator.SetTrigger("Attack");
            // �b���B�i�I�s�ˮ`�B�z�޿�
            yield return new WaitForSeconds(attackInterval);
        }
        else
        {
            Exit();
        }
    }
    /// <summary>
    /// This function is called by an Animation Event in the enemy attack animation.
    /// It performs hit detection against the player.
    /// </summary>
    public void PerformAttackHitDetection()
    {
        Debug.Log("Enemy PerformAttackHitDetection");

        if (npcStateData == null)
        {
            Debug.LogWarning("NPCStateData not assigned.");
            return;
        }

        // Calculate the damage based on NPCStateData.
        float damage = npcStateData.baseDamage * npcStateData.comboMultiplier;

        // Use OverlapSphere to detect the player.
        Collider[] hitColliders = Physics.OverlapSphere(
            attackHitPoint.position,
            npcStateData.hitRadius,
            npcStateData.playerLayers  // Make sure the player's layer is included in this mask.
        );

        foreach (Collider hit in hitColliders)
        {
            // Attempt to get the player's IDamageable component.
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.LogWarning("apply damage");
                damageable.TakeDamage(damage);
            }
        }

        Debug.DrawRay(attackHitPoint.position, Vector3.one * npcStateData.hitRadius, Color.red, 1f);
    }

}

