using System.Collections;
using UnityEngine;

public class ChurlAttackState : ChurlBase
{
    private float attackInterval = 1f;  // 攻擊間隔1秒
    private float attackRange = 1.5f;     // 與巡邏狀態的攻擊範圍相同
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
        // 切換 Animator 至 attackLayer（假設 attackLayer 已在 Animator 中設定）
        SetAnimatorLayerWeight("attackLayer", 1);
        SetAnimatorLayerWeight("walkLayer", 0);
        animator.SetBool("isAttacking", true);
        Debug.Log("進入攻擊狀態");
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
            // 若玩家跑出攻擊範圍，切換回巡邏狀態
            if (distance > attackRange)
            {
                Debug.Log("changeToPatrol");
                churl.ChangeState(new ChurlPatrolState());
            }
        }
    }


    public override void Exit()
    {
        Debug.Log("離開攻擊狀態");
        animator.SetBool("isAttacking", false);
        SetAnimatorLayerWeight("attackLayer", 0);
        if (attackCoroutine != null)
        {
            churl.StopCoroutine(attackCoroutine);
            attackCoroutine = null;  // 確保攻擊協程被清空，避免舊的協程影響新狀態
        }
    }

    private IEnumerator AttackRoutine()
    {
        if (true)
        {
            Debug.Log("攻擊玩家！");
            // 這裡觸發攻擊動畫，可以用 animator.SetTrigger("Attack")
            animator.SetTrigger("Attack");
            // 在此處可呼叫傷害處理邏輯
            yield return new WaitForSeconds(attackInterval);
        }
    }
}

