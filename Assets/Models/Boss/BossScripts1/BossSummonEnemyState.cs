//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class BossSummonEnemyState : BossBaseState
//{
//    // 召喚的 Enemy 預製物
//    public GameObject enemyPrefab;
//    // 召喚數量
//    public int enemyCount = 3;
//    // 可以設定召喚的偏移範圍
//    public Vector3 spawnOffsetMin = new Vector3(-2f, 0, -2f);
//    public Vector3 spawnOffsetMax = new Vector3(2f, 0, 2f);

//    public override void EnterState(BossFSM boss)
//    {
//        Debug.Log("Boss 進入 SummonEnemy 狀態");
//        // 播放 SummonEnemy 動畫，該動畫中應加入動畫事件 OnSummonEnemyEvent
//        boss.animator.SetTrigger("SummonEnemy");
//    }

//    public override void UpdateState(BossFSM boss)
//    {
//        // 這裡不需要自行更新，等待動畫事件觸發
//    }

//    public override void ExitState(BossFSM boss)
//    {
//        Debug.Log("Boss 離開 SummonEnemy 狀態");
//    }

//    // 此方法由 SummonEnemy 動畫事件呼叫
//    public void OnSummonEnemyEvent(BossFSM boss)
//    {
//        Debug.Log("SummonEnemy 動畫事件觸發，召喚敵人");
//        for (int i = 0; i < enemyCount; i++)
//        {
//            // 計算隨機偏移量，讓敵人散開生成
//            Vector3 offset = new Vector3(
//                Random.Range(spawnOffsetMin.x, spawnOffsetMax.x),
//                Random.Range(spawnOffsetMin.y, spawnOffsetMax.y),
//                Random.Range(spawnOffsetMin.z, spawnOffsetMax.z)
//            );
//            // 敵人生成位置以 Boss 位置加上偏移
//            Vector3 spawnPos = boss.transform.position + offset;
//            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
//        }
//    }
//}

