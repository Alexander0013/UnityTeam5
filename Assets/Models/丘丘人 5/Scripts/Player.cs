using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Collider collider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        // 設置 Rigidbody 為 Kinematic，避免物理影響
        if (rb != null)
        {
            rb.isKinematic = true;  // 設為 Kinematic，避免重力影響
        }

        // 設置 Collider 為 Trigger，允許進行觸發事件
        if (collider != null)
        {
            collider.isTrigger = true;  // 設為 Trigger，防止物理碰撞
        }
    }
}

