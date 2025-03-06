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
        AlignToGround();
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
        Debug.Log("�������A��G" + newState.GetType().Name);
        currentState.Enter();
    }
    void AlignToGround()
{
    RaycastHit hit;
    // Cast a ray from 1 unit above the enemy downward.
    Vector3 rayOrigin = transform.position + Vector3.up * 1f;
    // Use a reasonable distance (e.g., 10 units) and a ground layer mask (adjust if needed)
    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f, LayerMask.GetMask("Default")))
    {
        Vector3 pos = transform.position;
        // Optionally add a small offset (like 0.1f) so the enemy doesn't clip into the ground.
        pos.y = hit.point.y + 0.1f;
        transform.position = pos;
    }
}




}


