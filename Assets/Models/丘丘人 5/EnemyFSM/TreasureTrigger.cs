using UnityEngine;
using System.Collections.Generic;

public class TreasureTrigger : MonoBehaviour
{
    [Header("Treasure Trigger Settings")]
    public float triggerRadius = 5f;
    public GameObject enemyPrefab; // Assign your enemy prefab in the Inspector
    public int enemyCount = 4;
    public float spawnRadius = 10f;
    public GameObject invisibleWall; // Assign a GameObject that represents the invisible wall/battlefield

    private bool hasSpawnedEnemies = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public GameObject magicShield; 

    private void Update()
    {
        // Check distance between player and treasure
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // If the player is within triggerRadius and we haven't spawned yet
        if (!hasSpawnedEnemies && distance < triggerRadius)
        {
            SpawnEnemies();
            hasSpawnedEnemies = true;
            magicShield.SetActive(true);
        }

        // Optional: if you want the trigger to do something else once the player has triggered it
    }

    private void SpawnEnemies()
    {
        // Enable the invisible wall/battlefield
        if (invisibleWall != null)
        {
            invisibleWall.SetActive(true);
        }

        // Spawn multiple enemies in random positions within 'spawnRadius'
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPos = GetRandomSpawnPosition();
            GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            newEnemy.transform.SetParent(transform);  // 'transform' is the treasure's transform
            spawnedEnemies.Add(newEnemy);
        }

        // Optionally, track how many are alive
        // Then if all are dead, disable the invisible wall
        StartCoroutine(CheckAllEnemiesDead());
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(
            transform.position.x + randCircle.x,
            transform.position.y,
            transform.position.z + randCircle.y
        );
        return spawnPos;
    }

    private System.Collections.IEnumerator CheckAllEnemiesDead()
    {
        // Wait a frame so the enemies are instantiated
        yield return null;

        bool allDead = false;
        while (!allDead)
        {
            // Filter out destroyed (null) entries
            spawnedEnemies.RemoveAll(e => e == null);

            // If nothing is left, all are dead
            if (spawnedEnemies.Count == 0)
            {
                allDead = true;
            }
            yield return new WaitForSeconds(1f);
        }

        // Once all enemies are dead, disable the invisible wall
        if (invisibleWall != null)
        {
            invisibleWall.SetActive(false);
            magicShield.SetActive(false);
        }

        Debug.Log("All enemies defeated! Invisible wall disabled.");
    }

    private void OnDrawGizmosSelected()
    {
        // Just for visualization in the Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
