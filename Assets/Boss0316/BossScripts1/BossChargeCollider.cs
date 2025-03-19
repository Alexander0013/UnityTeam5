using UnityEngine;

public class BossChargeCollider : MonoBehaviour
{
    private BossFSM bossFSM;

    private void Start()
    {
        // 確保能找到 BossFSM
        bossFSM = GetComponentInParent<BossFSM>();
        if (bossFSM == null)
        {
            Debug.LogError("BossFSM 未找到！請確保此腳本掛載在 Boss Charge Collider 子物件上");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bossFSM != null && bossFSM.currentState is BossChargeState chargeState)
        {
            chargeState.OnChargeCollision(bossFSM, other);
        }
    }
}

