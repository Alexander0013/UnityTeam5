using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChurlPatrolSrate : ChurlBase
{
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPointIndex = 0;
    private float patrolRange = 3f;
    private float moveSpeed = 2f;

    public ChurlPatrolSrate(Churl churl) : base(churl) { }

    public override void Enter()
    {
        GeneratePatrolPoints();
        Debug.Log("�i�J���ު��A");
        SetAnimatorLayerWeight("walkLayer", 1); // �ҥΨ��� Layer
        SetAnimatorLayerWeight("combatLayer", 0); // �T�ξ԰� Layer
        SetAnimatorLayerWeight("deathLayer", 0); // �T�Φ��` Layer
        animator.SetBool("isWalking", true); // Ĳ�o���ްʵe
    }

    public override void Update()
    {
        if (patrolPoints.Count > 0)
        {
            MoveToPatrolPoint();
        }
    }

    public override void Exit()
    {
        // ���}���ު��A�ɤ��ݭn���S���B�z
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        Vector3 startPosition = churl.transform.position;
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint = startPosition + new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
            patrolPoints.Add(randomPoint);
        }
    }

    private void MoveToPatrolPoint()
    {
        Vector3 target = patrolPoints[currentPointIndex];
        churl.transform.position = Vector3.MoveTowards(churl.transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 direction = (target - churl.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churl.transform.rotation = Quaternion.Slerp(churl.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(churl.transform.position, target) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
    }
}

