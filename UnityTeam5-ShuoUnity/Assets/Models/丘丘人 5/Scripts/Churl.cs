using UnityEngine;

public class Churl : MonoBehaviour
{
    private ChurlBase currentState;
    private ChurlPatrolState patrolState;
    private Rigidbody rb;
    private Collider collider;
    void Start()
    {
        patrolState = gameObject.AddComponent<ChurlPatrolState>(); // �T�O�� `AddComponent`

        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if (rb != null)
        {
            rb.isKinematic = true;  // �T�O Rigidbody �O Kinematic
        }

        // �]�m Collider �� Trigger�A����P���a���������z�I��
        if (collider != null)
        {
            //collider.isTrigger = true;  // �]�� Trigger
        }
        // �]�w��l���A������
        ChangeState(patrolState);
        if (this == null)
        {
            Debug.LogError("Churl ���� null�I");
        }
        else
        {
            Debug.Log("Churl ����w��l�ơG" + gameObject.name);
        }
    }
    void Update()
    {
        currentState?.Update();

    }
    public void ChangeState(ChurlBase newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        if (newState is ChurlPatrolState)
        {
            newState = patrolState; // �ϥΤw��l�ƪ� patrolState
        }
        currentState = newState;
        currentState.SetChurl(this); // �]�m Churl �Ѧ�
        Debug.Log("Enter" + newState.GetType().Name);
        currentState.Enter();
    }





}


