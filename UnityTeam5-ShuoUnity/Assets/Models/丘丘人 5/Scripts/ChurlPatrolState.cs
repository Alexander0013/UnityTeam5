using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChurlPatrolState : ChurlBase
{
    private List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPointIndex = 0;
    //private float patrolRange = 3f;
    private float moveSpeed = 2f;
    private float detectionRange = 5f; // �����d��
    private LayerMask playerLayer; // ���a�h
    private float attackRange = 1.5f;

    private static Vector3 patrolCenter; // ���ްϰ쪺����
    private static float patrolRadius = 5f; // ���޽d�򪺥b�|
    private bool returningToPatrolArea = false; // �O�_���b�^�k���޽d��
    private Vector3 returnPoint; // �^�k�d��ɪ��ؼ��I
    public override void Enter()
    {
        Debug.Log("EnterPatrol");
        // �]�m���a�h�A�T�O���i�H���T�ѧO
        playerLayer = LayerMask.GetMask("Player"); // �T�O�A�w�g�]�w�F 'Player' �h
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
        // �T�O churl �s�b
        if (churl == null)
        {
            return;
        }
        // �]�m churlObject
        churlObject = churl.gameObject;
        // �T�O animator ��l��
        if (animator == null)
        {
            animator = churlObject.GetComponent<Animator>();
        }
        // **�Ĥ@���i�J�ɡA�]�w���ޤ���**
        if (patrolCenter == Vector3.zero)
        {
            patrolCenter = churlObject.transform.position; // �H���e��m�@�����ޤ���
        }

        // **�p�G�O�q AttackState �^�ӡA���^��d��**
        if (!IsInsidePatrolArea(churlObject.transform.position))
        {
            returningToPatrolArea = true;
            returnPoint = GetRandomPointInPatrolArea(); // �H����@���I�^�h
        }
        else
        {
            GeneratePatrolPoints(); // �����ͦ������I
        }
        // ������� churl�A�p�G���M�� null �h�q�������M��
        GeneratePatrolPoints();
        IsPlayerDetected();
        MoveToPatrolPoint();
        DetectPlayer();
        ChasePlayer();
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            //StartCoroutine(SmoothSetAnimatorLayerWeight("attackLayer", 0f));

            //StartCoroutine(SmoothSetAnimatorLayerWeight("walkLayer", 1f));
            SetAnimatorLayerWeight("walkLayer", 1);
            SetAnimatorLayerWeight("attackLayer", 0);
            //SetAnimatorLayerWeight("deathLayer", 0);
            //animator.SetBool("isWalking", true);
        }
    }

    public override void Update()
    {
        Debug.Log("CPSUpdate");
        // ���� Update ����� churlObject �� null
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
            Debug.LogError("Update(): churlObject ���M�� null�A�L�k�����޿�I");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Update(): �䤣��аO�� 'Player' ������I");
            return;
        }

        bool detected = IsPlayerDetected();
        if (detected)
        {
            float distance = Vector3.Distance(churlObject.transform.position, player.transform.position);
            //Debug.Log("Distance to player: " + distance);
            if (distance <= attackRange * 0.8f)
            {
                Debug.Log("Switching to AttackState because player is close enough.");
                churl.ChangeState(new ChurlAttackState());
            }
            else
            {
                ChasePlayer();
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
        // ���}���ު��A�ɤ��ݭn���S���B�z
    }
    private void MoveToReturnPoint()
    {
        Debug.Log("returnPoint");
        // �p�Ⲿ�ʤ�V
        Vector3 direction = (returnPoint - churlObject.transform.position).normalized;

        // ���Ǫ�����ؼ��I
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churlObject.transform.rotation = Quaternion.Slerp(churlObject.transform.rotation, targetRotation, Time.deltaTime * 5f); // �ϥΥ��Ʊ���
        }

        // ���ʨ�^�Ӫ������I
        churlObject.transform.position = Vector3.MoveTowards(churlObject.transform.position, returnPoint, moveSpeed * Time.deltaTime);

        // �������^�����I�ɡA����öi�J���ު��A
        if (Vector3.Distance(churlObject.transform.position, returnPoint) < 0.1f)
        {
            returningToPatrolArea = false;
            GeneratePatrolPoints(); // �ͦ��s�������I
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
        // �����d�򤺬O�_�����a
        Collider[] hitColliders = Physics.OverlapSphere(churl.transform.position, detectionRange, playerLayer);
        if (hitColliders.Length > 0)
        {
            // �p�G�����a�A��^ true
            return true;
        }
        return false;
    }
    private void DetectPlayer()
    {
        // �T�O playerLayer �M detectionRange �w��l��
        if (playerLayer == 0)
        {
            Debug.LogWarning("Player Layer ���]�m�I");
        }
        if (detectionRange <= 0)
        {
            Debug.LogWarning("Detection Range �� 0 �έt�ơI");
        }

        if (IsPlayerDetected())
        {
            // ����l�v���a���޿�
            Debug.Log("���a�w�Q������A�ǳưl�v�I");
        }
    }
    private void ChasePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("�L�k��쪱�a����I");
            return;
        }
        Vector3 direction = (player.transform.position - churlObject.transform.position).normalized;
        // ��V���a
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            churlObject.transform.rotation = Quaternion.Slerp(churlObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        // �ª��a����
        churlObject.transform.position = Vector3.MoveTowards(churlObject.transform.position, player.transform.position, moveSpeed * Time.deltaTime);
    }
}