using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string scene1Name; // Set the first scene name
    [SerializeField] private string scene2Name; // Set the second scene name
    //[SerializeField] private string scene3Name; // Set the third scene name

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Detect Enter key press
        {
            SwitchScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // Detect Esc key press
        {
            QuitApplication();
        }
    }

    private void SwitchScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == scene1Name)
        {
            SceneManager.LoadScene(scene2Name);
        }
        else if (currentScene == scene2Name)
        {
            SceneManager.LoadScene(scene1Name);
        }
        else
        {
            Debug.LogError("Current scene does not match the specified scenes! Check scene names.");
        }
    }

    private void QuitApplication()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();

        // This is only for testing in the Unity Editor (it won’t work in the build)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
