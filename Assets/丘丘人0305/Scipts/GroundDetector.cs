using UnityEngine;

public class GrounDetector : MonoBehaviour
{
    public float raycastDistance = 10f;  // Raycast 的檢測距離
    public float groundOffset = 0.01f;    // 與地面保持微小距離
    private int groundLayerMask;

    void Start()
    {
        // 取得名為 "Ground" 的 Layer 的 mask
        groundLayerMask = 1 << LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        SnapToGround();
    }

    // 使用 Raycast 檢測地面並更新 y 軸
    public void SnapToGround()
    {
        RaycastHit hit;
        // 從物件位置的上方發射射線，這樣可以確保即使物件稍微懸空也能檢測到地面
        Vector3 rayOrigin = transform.position + Vector3.up;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            // 更新 y 軸，使物件緊貼地面（加上微小偏移以防止重疊）
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + groundOffset;
            transform.position = newPosition;
        }
    }
}


