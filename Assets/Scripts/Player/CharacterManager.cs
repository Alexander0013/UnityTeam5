using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityChan; // For SpringManager

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    // List of all character GameObjects.
    public List<GameObject> characters;

    // Reference to the Cinemachine Virtual Camera.
    public CinemachineVirtualCamera virtualCamera;

    // Index to track the currently active character.
    private int currentCharacterIndex = 0;

    // Name of the child object used for the camera target.
    private string cameraRootName = "PlayerCameraRoot";

    // Delay before activating the initial character.
    public float initialActivationDelay = 0.1f;

    // Duration (in seconds) over which to ramp up hair simulation.
    public float hairRampUpDuration = 0.1f;
    public Transform startPosition;

    public event System.Action SwitchPlayer;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Called every time a new scene is loaded.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the new Cinemachine Virtual Camera in the scene.
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogWarning("No Cinemachine Virtual Camera found in the scene!");
        }
        else
        {
            // Reconnect the camera to the currently active character.
            GameObject activeCharacter = characters[currentCharacterIndex];
            Transform cameraRoot = GetCameraRoot(activeCharacter);
            if (cameraRoot != null)
            {
                virtualCamera.Follow = cameraRoot;
                virtualCamera.LookAt = cameraRoot;
            }
        }
        float newQuadSize = 1f; // default value, if needed.
        if (scene.name == "Temple")
        {
            newQuadSize = 4f;
        }
        else if (scene.name == "PureNature")
        {
            newQuadSize = 15f;
        }
        // Update startPosition:
        GameObject startPosObj = GameObject.FindGameObjectWithTag("StartPosition");
        if (startPosObj != null)
        {
            startPosition = startPosObj.transform;
            foreach (GameObject player in characters)
            {
                player.transform.position = startPosition.position;
                // Assumes each player has a child named "Player_Quad".
                Transform playerQuad = player.transform.Find("Player_Quad");
                if (playerQuad != null)
                {
                    playerQuad.localScale = new Vector3(newQuadSize, newQuadSize, newQuadSize);
                }
                else
                {
                    Debug.LogWarning("Player_Quad not found on " + player.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("No object with tag 'StartPosition' found in the scene!");
        }

    }

    void Start()
    {
        // Initially deactivate all characters.
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // activative  the initial player.
        ActivateInitialCharacter();
        // Start checking if all players are dead.
        StartCoroutine(CheckAllPlayersDead());
    }

    

    void Update()
    {
        // Check for the key press to switch characters (e.g., using the C key).
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCharacter();
        }
    }
    public void ActivateInitialCharacter()
    {

        // Activate the first character.
        currentCharacterIndex = 0;
        GameObject initialPlayer = characters[currentCharacterIndex];
        initialPlayer.SetActive(true);

        // Set the camera's follow and look-at targets.
        Transform cameraRoot = GetCameraRoot(initialPlayer);
        if (cameraRoot != null)
        {
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
        else
        {
            Debug.LogWarning("Camera root not found on " + initialPlayer.name);
        }

        // Gradually ramp up hair simulation to hide abrupt movements.
        //StartCoroutine(RampUpHairSimulation(initialPlayer));
    }

    void SwitchCharacter()
    {
        // Store current character's position and rotation.
        GameObject currentCharacter = characters[currentCharacterIndex];
        Vector3 currentPos = currentCharacter.transform.position;
        Quaternion currentRot = currentCharacter.transform.rotation;

        // Deactivate the current character.
        currentCharacter.SetActive(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        /*
        // We'll try to find a valid (alive) character.
        int startingIndex = currentCharacterIndex;
        bool foundValid = false;

        //find one with health > 0.
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        GameObject potentialCharacter = characters[currentCharacterIndex];
        PlayerHealth ph = potentialCharacter.GetComponent<PlayerHealth>();

        // If there is no health component, or health is above zero, consider it valid.
        if (ph.CurrentHealth > 0)
        {
            foundValid = true;
        }

        // If no valid character is found (all dead), re-activate the original character.
        if (!foundValid)
        {
            characters[startingIndex].SetActive(true);
            Debug.LogWarning("No valid (alive) character found for switching!");
            return;
        }
        */

        // Activate the new valid character.
        GameObject newCharacter = characters[currentCharacterIndex];
        newCharacter.transform.position = currentPos;
        newCharacter.transform.rotation = currentRot;
        newCharacter.SetActive(true);

        // Update the camera's follow and look-at targets.
        Transform cameraRoot = GetCameraRoot(newCharacter);
        if (cameraRoot != null)
        {
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
        else
        {
            Debug.LogWarning("Camera root not found on " + newCharacter.name);
        }

        // Trigger any events (if you have subscribers).
        SwitchPlayer?.Invoke();
    }


    // Helper function to find the PlayerCameraRoot transform in a character.
    Transform GetCameraRoot(GameObject character)
    {
        return character.transform.Find(cameraRootName);
    }
    private IEnumerator CheckAllPlayersDead()
    {
        while (true)
        {
            bool allDead = true;
            foreach (GameObject character in characters)
            {
                PlayerHealth ph = character.GetComponent<PlayerHealth>();
                // If any character has health above 0, they're alive.
                if (ph != null && ph.CurrentHealth > 0)
                {
                    allDead = false;
                    break;
                }
            }
            if (allDead)
            {
                // All players are dead.
                Debug.Log("All players are dead. Resetting health and moving to start position.");
                // Move all characters to start position and reset health.
                foreach (GameObject character in characters)
                {
                    character.transform.position = startPosition.position;
                    PlayerHealth ph = character.GetComponent<PlayerHealth>();
                    if (ph != null)
                    {
                        ph.ResetHealth();
                    }
                    // Optionally, you might want to activate all characters if they were deactivated.
                    character.SetActive(true);
                }
                // Optionally, switch the active character to the first in the list.
                currentCharacterIndex = 0;
                Transform cameraRoot = GetCameraRoot(characters[currentCharacterIndex]);
                if (cameraRoot != null)
                {
                    virtualCamera.Follow = cameraRoot;
                    virtualCamera.LookAt = cameraRoot;
                }
                // Once reset, break out or continue checking as needed.
                break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}