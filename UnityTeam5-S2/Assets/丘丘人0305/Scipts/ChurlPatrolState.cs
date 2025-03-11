using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChurlPatrolState : ChurlBase
{
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPointIndex = 0;
    private float patrolRange = 3f;
    private float moveSpeed = 2f;
    private float detectionRange = 5f; // 偵測範圍
    private LayerMask playerLayer; // 玩家層
    private float attackRange = 1.5f;

    private static Vector3 patrolCenter; // 巡邏區域的中心
    private static float patrolRadius = 5f; // 巡邏範圍的半徑
    private bool returningToPatrolArea = false; // 是否正在回歸巡邏範圍
    private Vector3 returnPoint; // 回歸範圍時的目標點
    public override void Enter()
    {
        Debug.Log("EnterPatrol");
        // 設置玩家層，確保它可以正確識別
        playerLayer = LayerMask.GetMask("Player"); // 確保你已經設定了 'Player' 層
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            GeneratePatrolPoints();
        }
        if (churl == null)
        {
            churl = GetComponent<Churl>();
            if (churl == null)
            {
                churl = GameObject.FindObjectOfType<Churl>();
            }
        }
        // 確保 churl 存在
        if (churl == null)
        {
            return;
        }
        // 設置 churlObject
        churlObject = churl.gameObject;
        // 確保 animator 初始化
        if (animator == null)
        {
            animator = churlObject.GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }
        }
        // **第一次進入時，設定巡邏中心**
        if (patrolCenter == Vector3.zero)
        {
            patrolCenter = churlObject.transform.position; // 以當前位置作為巡邏中心
        }

        // **如果是從 AttackState 回來，先回到範圍內**
        if (!IsInsidePatrolArea(churlObject.transform.position))
        {
            returningToPatrolArea = true;
            returnPoint = GetRandomPointInPatrolArea(); // 隨機選一個點回去
        }
        else
        {
            GeneratePatrolPoints(); // 直接生成巡邏點
        }
        // 嘗試獲取 churl，如果仍然為 null 則從場景中尋找
        GeneratePatrolPoints();
        IsPlayerDetected();
        MoveToPatrolPoint();
        DetectPlayer();
        ChasePlayer();
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            SetAnimatorLayerWeight("walkLayer", 1);
            SetAnimatorLayerWeight("attackLayer", 0);
            animator.SetBool("isWalking", true);
        }
    }

    public override void Update()
{
        Debug.Log("CPSUpdate");
    // 防止 Update 執行時 churlObject 為 null
    if (churlObject == null)
    {
        if (churl == null)
        {
            churl = GetComponent<Churl>();
            if (churl == null)
            {
                churl = GameObject.FindObjectOfType<Churl>();
            }
        }

        if (churl != null)
        {
            churlObject = churl.gameObject;
        }
    }

    if (churlObject == null)
    {
        Debug.LogError("Update(): churlObject 仍然為 null，無法執行邏輯！");
        return;
    }

    GameObject player = GameObject.FindWithTag("Player");

    //if (player == null)
    //{
    //    Debug.LogError("Update(): 找不到標記為 'Player' 的物件！");
    //    return;
    //}

    bool detected = IsPlayerDetected();
    if (detected)
    {
        float distance = Vector3.Distance(churlObject.transform.position, player.transform.position);
        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            churl.ChangeState(new ChurlAttackState());
        }
    }
    else
    {
            if (returningToPatrolArea)
            {
                MoveToReturnPoint();
            }
            else
            {
                MoveToPatrolPoint();
            }
    }
}
    public override void Exit()
    {
        // 離開巡邏狀態時不需要做特殊處理
    }
    private void MoveToReturnPoint()
    {
        Debug.Log("returnPoint");
        churlObject.transform.position = Vector3.MoveTowards(churlObject.transform.position, returnPoint, moveSpeed * Time.deltaTime);
        Vector3 direction = (returnPoint - churlObject.transform.position).normalized;

        // 讓怪物面對目標點
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churlObject.transform.rotation = Quaternion.Slerp(churlObject.transform.rotation, targetRotation, Time.deltaTime * 5f); // 使用平滑旋轉
        }
        if (Vector3.Distance(churlObject.transform.position, returnPoint) < 0.1f)
        {
            returningToPatrolArea = false;
            GeneratePatrolPoints();
        }
    }
    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        for (int i = 0; i < 5; i++)
        {
            patrolPoints.Add(GetRandomPointInPatrolArea());
        }
    }

    private Vector3 GetRandomPointInPatrolArea()
    {
        Vector3 randomPoint;
        do
        {
            randomPoint = patrolCenter + new Vector3(Random.Range(-patrolRadius, patrolRadius), 0, Random.Range(-patrolRadius, patrolRadius));
        } while (!IsInsidePatrolArea(randomPoint));
        return randomPoint;
    }
    private bool IsInsidePatrolArea(Vector3 position)
    {
        return Vector3.Distance(position, patrolCenter) <= patrolRadius;
    }
    private void MoveToPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Vector3 target = patrolPoints[currentPointIndex];
        churlObject.transform.position = Vector3.MoveTowards(churlObject.transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 direction = (target - churlObject.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churlObject.transform.rotation = Quaternion.Slerp(churlObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(churlObject.transform.position, target) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
    }
    private bool IsPlayerDetected()
    {
        if (churl == null)
        {
            return false;
        }
        // 偵測範圍內是否有玩家
        Collider[] hitColliders = Physics.OverlapSphere(churl.transform.position, detectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            // 如果有玩家，返回 true
            return true;
        }
        return false;
    }
    private void DetectPlayer()
    {
        // 確保 playerLayer 和 detectionRange 已初始化
        if (playerLayer == 0)
        {
            Debug.LogWarning("Player Layer 未設置！");
        }
        if (detectionRange <= 0)
        {
            Debug.LogWarning("Detection Range 為 0 或負數！");
        }

        if (IsPlayerDetected())
        {
            // 執行追逐玩家的邏輯
            Debug.Log("玩家已被偵測到，準備追逐！");
        }
    }
    private void ChasePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("無法找到玩家物體！");
            return;
        }
        Vector3 direction = (player.transform.position - churlObject.transform.position).normalized;
        // 轉向玩家
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churlObject.transform.rotation = Quaternion.Slerp(churlObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        // 朝玩家移動
        churlObject.transform.position = Vector3.MoveTowards(churlObject.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }

}