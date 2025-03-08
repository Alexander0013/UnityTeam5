using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MonsterPatrol : MonoBehaviour
{
    public float patrolRange = 3f; // 巡邏範圍
    public float moveSpeed = 2f; // 移動速度
    private List<Vector3> patrolPoints = new List<Vector3>(); // 存放巡邏點
    private int currentPointIndex = 0; // 目前目標巡邏點
    private Rigidbody rb;
    private Collider outCollider;
    private Collider innerColider;
    void Start()
    {
        //GeneratePatrolPoints();     
        rb = GetComponent<Rigidbody>();
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if(col.isTrigger)
                outCollider = col;  
            else
                innerColider = col;
        }
        GeneratePatrolPoints();
    }

    void Update()
    {
        if (patrolPoints.Count > 0)
        {
            MoveToPatrolPoint();
        }
    }

    void GeneratePatrolPoints()
    {
        patrolPoints.Clear(); // 清除舊的巡邏點
        Vector3 startPosition = transform.position; // 以當前位置為起點
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint = startPosition + new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
            patrolPoints.Add(randomPoint);
        }
    }

    void MoveToPatrolPoint()
    {
        Vector3 target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        Vector3 direction = (target - transform.position).normalized;
        // 讓怪物面向目標方向
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 轉向速度
        }
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count; // 循環巡邏
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("churl"))
        {
            Vector3 awayDirection = (transform.position - other.transform.position).normalized;
            transform.position += awayDirection * moveSpeed * 2f * Time.deltaTime;
            float randomAngle = Random.Range(-120f, 120f);
            ChangePatrolTarget();
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("churl"))
        {
            Vector3 awayDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(awayDirection * 10f, ForceMode.Impulse);
            //float randomAngle = Random.Range(30f, 90f);
            //awayDirection = Quaternion.Euler(0, randomAngle, 0) * awayDirection;
            //transform.position += awayDirection * moveSpeed * Time.deltaTime;
        }
    }
    void ChangePatrolTarget()
    {
        // 嘗試找到一個新的巡邏點，避免與其他怪物重疊
        Vector3 newTarget;
        int attempts = 0;
        do
        {
            newTarget = transform.position + new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
            attempts++;
        }
        while (attempts < 5 && Vector3.Distance(newTarget, patrolPoints[currentPointIndex]) < 2f); // 確保新點遠離當前點

        // 將新的目標加入巡邏點列表，並設為當前目標
        patrolPoints[currentPointIndex] = newTarget;
    }

}

