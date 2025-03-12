using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public Canvas canvas;

    private Dictionary<GameObject, GameObject> healthBars = new Dictionary<GameObject, GameObject>();

    public void Start()
    {
        SpawnHealthBars();
    }

    void SpawnHealthBars()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (!healthBars.ContainsKey(enemy)) // 確保不會為同一敵人生成多個血條
            {
                GameObject healthBar = Instantiate(healthBarPrefab, canvas.transform);
                healthBar.transform.localPosition = Vector3.zero;
                EnemyHealthBar healthBarScript = healthBar.GetComponent<EnemyHealthBar>();
                healthBarScript.Initialize(enemy);

                healthBars[enemy] = healthBar;
            }
        }
    }

    void Update()
    {
        // 清除已死亡的敵人血條
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var kvp in healthBars)
        {
            if (kvp.Key == null) // 如果敵人被刪除
            {
                Destroy(kvp.Value); // 刪除血條
                toRemove.Add(kvp.Key);
            }
        }

        // 從字典中移除
        foreach (var enemy in toRemove)
        {
            healthBars.Remove(enemy);
        }
    }

}
