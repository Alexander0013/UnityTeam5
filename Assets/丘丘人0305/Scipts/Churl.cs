using UnityEngine;

public class Churl : MonoBehaviour
{
    private ChurlBase currentState;
    private ChurlPatrolState patrolState;
    private Rigidbody rb;
    private Collider collider;
    void Start()
    {
        patrolState = gameObject.AddComponent<ChurlPatrolState>(); // 確保用 `AddComponent`
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if (rb != null)
        {
            rb.isKinematic = true;  // 確保 Rigidbody 是 Kinematic
        }

        // 設置 Collider 為 Trigger，防止與玩家之間的物理碰撞
        if (collider != null)
        {
            collider.isTrigger = true;  // 設為 Trigger
        }
        // 設定初始狀態為巡邏
        ChangeState(patrolState);
        if (this == null)
        {
            Debug.LogError("Churl 物件為 null！");
        }
        else
        {
            Debug.Log("Churl 物件已初始化：" + gameObject.name);
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
            newState = patrolState; // 使用已初始化的 patrolState
        }
        currentState = newState;
        currentState.SetChurl(this); // 設置 Churl 參考
        Debug.Log("切換狀態到：" + newState.GetType().Name);
        currentState.Enter();
    }


}


