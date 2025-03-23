using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [Header("Characters & Camera")]
    public List<GameObject> characters;              // All possible characters
    public CinemachineVirtualCamera virtualCamera;   // The vcam in the *current* scene
    private int currentCharacterIndex = 0;
    public Vector3 fallbackPosition = new Vector3(-14f, 5f, 6f);
    public Quaternion fallbackRotation = Quaternion.identity;         
    private string cameraRootName = "PlayerCameraRoot";

    [Header("Scene Spawn")]
    public Transform startPosition;     // Will be assigned in OnSceneLoaded if the new scene has a "StartPosition"

    public event System.Action SwitchPlayer;

    private void Awake()
    {
        // Singleton check
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // Keep this CharacterManager alive across scene loads
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        // Unsubscribe when disabled/destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Start()
    {
        // Deactivate all characters initially
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // Activate the first character
        ActivateInitialCharacter();
        
        // If you have a coroutine to check for all players dead, start it
        StartCoroutine(CheckAllPlayersDead());
    }

    /// <summary>
    /// This is called automatically by Unity after a scene finishes loading,
    /// because we subscribed in OnEnable().
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[CharacterManager] OnSceneLoaded called for scene: " + scene.name);

        // 1) Find the Cinemachine virtual camera in the new scene
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogWarning("No Cinemachine Virtual Camera found in the new scene!");
        }
        else
        {
            // Reconnect the camera to the currently active character
            GameObject activeCharacter = characters[currentCharacterIndex];
            Transform cameraRoot = GetCameraRoot(activeCharacter);
            if (cameraRoot != null)
            {
                virtualCamera.Follow = cameraRoot;
                virtualCamera.LookAt = cameraRoot;
            }
        }

        // 2) Find the StartPosition object in the new scene (tagged “StartPosition”)
        GameObject startPosObj = GameObject.FindGameObjectWithTag("StartPosition");
        if (startPosObj != null)
        {
            startPosition = startPosObj.transform;

            // Example: Place *all* characters at startPosition
            // (If you only want to move the currently active character, remove the foreach.)
                characters[currentCharacterIndex].transform.position = startPosition.position;
                characters[currentCharacterIndex].transform.rotation = startPosition.rotation;
        }
        else
        {
            Debug.LogWarning("No 'StartPosition' tag found in the new scene!");
        }

        // 3) (Optional) Scene-specific scaling example
        float newQuadSize = 1f;
        if (scene.name == "Temple")       newQuadSize = 4f;
        else if (scene.name == "PureNature") newQuadSize = 15f;

        // If you have a child named "Player_Quad" on each character that needs scaling:
        foreach (GameObject player in characters)
        {
            Transform quad = player.transform.Find("Player_Quad");
            if (quad != null)
            {
                quad.localScale = new Vector3(newQuadSize, newQuadSize, newQuadSize);
            }
        }
    }
    public void OnSceneSwitchComplete(int sceneIndex)
{
    // 1) Find or create the Virtual Camera in the new scene:
    CinemachineVirtualCamera newVcam = FindObjectOfType<CinemachineVirtualCamera>();
    if (newVcam != null)
    {
        virtualCamera = newVcam; 
        // reassign Follow/LookAt
        GameObject activeCharacter = characters[currentCharacterIndex];
        Transform cameraRoot = GetCameraRoot(activeCharacter);
        if (cameraRoot != null)
        {
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
    }

    // 2) Move the player(s) to the new scene’s StartPosition
    GameObject spawnObj = GameObject.FindWithTag("StartPosition");
    if (spawnObj != null)
    {
        startPosition = spawnObj.transform;
        foreach (GameObject player in characters)
        {
            player.transform.position = startPosition.position;
            player.transform.rotation = startPosition.rotation;
        }
    }
    else{
        foreach (GameObject player in characters)
        {
            player.transform.position = fallbackPosition;
            player.transform.rotation = fallbackRotation;
        }
    }
    
    // 3) Scene-specific logic (e.g., scaling, UI, etc.)
    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
    if (scene.name == "Temple")
    {
        // scale quads, etc.
    }
    else if (scene.name == "PureNature")
    {
        // ...
    }
}


    /// <summary>
    /// Activates the first character in the list.
    /// </summary>
    public void ActivateInitialCharacter()
    {
        currentCharacterIndex = 0;
        GameObject initialPlayer = characters[currentCharacterIndex];
        initialPlayer.SetActive(true);

        // If we already have a virtual camera (e.g. in the start scene),
        // attach it to the initial player's camera root
        Transform cameraRoot = GetCameraRoot(initialPlayer);
        if (cameraRoot != null && virtualCamera != null)
        {
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
    }

    /// <summary>
    /// Switches between the first and second character (or cycles through).
    /// </summary>
    private void SwitchCharacter()
    {
        // Store current char’s position/rotation
        GameObject currentChar = characters[currentCharacterIndex];
        Vector3 oldPos = currentChar.transform.position;
        Quaternion oldRot = currentChar.transform.rotation;

        // Deactivate old
        currentChar.SetActive(false);

        // Next index
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        GameObject newChar = characters[currentCharacterIndex];
        newChar.transform.position = oldPos;
        newChar.transform.rotation = oldRot;
        newChar.SetActive(true);

        // Update the camera’s Follow / LookAt
        if (virtualCamera != null)
        {
            Transform newCameraRoot = GetCameraRoot(newChar);
            if (newCameraRoot != null)
            {
                virtualCamera.Follow = newCameraRoot;
                virtualCamera.LookAt = newCameraRoot;
            }
        }

        // Fire event so UI or others can respond
        SwitchPlayer?.Invoke();
    }

    /// <summary>
    /// Returns the "PlayerCameraRoot" child from a character, used for Cinemachine Follow/LookAt.
    /// </summary>
    Transform GetCameraRoot(GameObject character)
    {
        return character.transform.Find(cameraRootName);
    }

    /// <summary>
    /// Example: checks if all players are dead, then resets them.
    /// (Your existing code can remain the same.)
    /// </summary>
    private IEnumerator CheckAllPlayersDead()
    {
        while (true)
        {
            bool allDead = true;
            foreach (GameObject character in characters)
            {
                PlayerHealth ph = character.GetComponent<PlayerHealth>();
                if (ph != null && ph.CurrentHealth > 0)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                Debug.Log("All players dead. Resetting health and moving to startPosition.");
                foreach (GameObject character in characters)
                {
                    if (startPosition)
                    {
                        character.transform.position = startPosition.position;
                        character.transform.rotation = startPosition.rotation;
                    }
                    PlayerHealth ph = character.GetComponent<PlayerHealth>();
                    if (ph != null)
                    {
                        ph.ResetHealth();
                    }
                    character.SetActive(true);
                }

                // Make sure the camera is following the first character now
                currentCharacterIndex = 0;
                GameObject newChar = characters[currentCharacterIndex];
                if (virtualCamera != null)
                {
                    Transform newCameraRoot = GetCameraRoot(newChar);
                    if (newCameraRoot != null)
                    {
                        virtualCamera.Follow = newCameraRoot;
                        virtualCamera.LookAt = newCameraRoot;
                    }
                }
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        // Switch character with “C” key, for testing
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCharacter();
        }
    }
}
