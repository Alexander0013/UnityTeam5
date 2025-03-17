using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Tooltip("The name of the scene to load when entering the portal.")]
    public string sceneToLoad;

    [Tooltip("Optional: a fade duration for smooth transition.")]
    public float fadeDuration = 1f;

    // Optionally, add a spawn point ID to tell the new scene where the player should appear.
    // This can be used with a GameManager that holds persistent data.
    public string spawnPointID;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            isTransitioning = true;
            StartCoroutine(Transition());
        }
    }

    private IEnumerator Transition()
    {
        // Optionally: Start a fade-out animation here
        // e.g., using a UI canvas with an image and a FadeOut() coroutine.
        yield return new WaitForSeconds(fadeDuration);

        // Optionally: Save data (like player stats or spawn point info) in a GameManager or using PlayerPrefs.
        // Example: GameManager.Instance.SetSpawnPoint(spawnPointID);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);

        // Optionally: Start a fade-in after scene loads.
    }
}
