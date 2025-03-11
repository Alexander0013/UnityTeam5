using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureTrigger : MonoBehaviour
{
    [Header("Treasure Trigger Settings")]
    public float triggerRadius = 6f;
    
    [Tooltip("Assign the invisible wall or barrier GameObject here.")]
    public GameObject invisibleWall; 

    [Tooltip("Assign your magic shield (if any) here.")]
    public GameObject magicShield;
    
    [Header("Pre-Placed Enemies")]
    [Tooltip("Drag all pre-placed enemy GameObjects into this list in the Inspector.")]
    public List<GameObject> sceneEnemies = new List<GameObject>();

    public GameObject player; 

    private bool hasTriggered = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(UpdatePlayerReferenceCoroutine());
        
    }


    private void Update()
    {
        
        if (player == null) return;
        // 1) Check distance between player and treasure
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 2) If the player is within triggerRadius and we haven't triggered yet
        if (!hasTriggered && distance < triggerRadius)
        {
            hasTriggered = true; 
            
            // Enable the wall + shield
            if (invisibleWall != null) invisibleWall.SetActive(true);
            if (magicShield != null) magicShield.SetActive(true);

            // Start checking if all enemies in sceneEnemies are dead
            StartCoroutine(CheckAllEnemiesDead());
        }
    }

    private IEnumerator CheckAllEnemiesDead()
    {
        // Wait a frame for safety
        yield return null;

        bool allDead = false;
        while (!allDead)
        {
            // Remove null references (enemies that have been destroyed)
            sceneEnemies.RemoveAll(e => e == null);

            // If nothing is left, all are dead
            if (sceneEnemies.Count == 0)
            {
                allDead = true;
            }

            yield return new WaitForSeconds(1f);
        }

        // Once all enemies are dead, disable the invisible wall and shield
        if (invisibleWall != null)
        {
            invisibleWall.SetActive(false);
        }
        if (magicShield != null)
        {
            magicShield.SetActive(false);
        }

        Debug.Log("All enemies defeated! Invisible wall disabled.");
    }

    private void OnDrawGizmosSelected()
    {
        // For visualization in the Editor: trigger radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    private IEnumerator UpdatePlayerReferenceCoroutine()
{
    while (true)
    {
        GameObject newPlayer = GameObject.FindWithTag("Player");
        if (newPlayer != null && newPlayer.activeInHierarchy)
        {
            player = newPlayer;
        }
        else
        {
            player = null;
        }
        yield return new WaitForSeconds(1f); // update every  second
    }
}


}
