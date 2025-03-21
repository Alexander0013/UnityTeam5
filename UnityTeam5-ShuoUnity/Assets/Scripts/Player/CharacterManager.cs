using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityChan; // For SpringManager

public class CharacterManager : MonoBehaviour
{
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
    public float hairRampUpDuration = 0.5f;

    void Start()
    {
        // Initially deactivate all characters.
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // Delay activation of the initial player.
        StartCoroutine(ActivateInitialCharacter());
    }

    IEnumerator ActivateInitialCharacter()
    {
        yield return new WaitForSeconds(initialActivationDelay);

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
        StartCoroutine(RampUpHairSimulation(initialPlayer));
    }

    void Update()
    {
        // Check for the key press to switch characters (e.g., using the C key).
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        // Store transform data from the current active character.
        GameObject currentCharacter = characters[currentCharacterIndex];
        Vector3 currentPos = currentCharacter.transform.position;
        Quaternion currentRot = currentCharacter.transform.rotation;

        // Deactivate the current character.
        currentCharacter.SetActive(false);

        // Determine the next character index (wrap around if needed).
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        GameObject newCharacter = characters[currentCharacterIndex];

        // Match the new character's transform to the previous one.
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

        // Gradually ramp up hair simulation for the new character.
        StartCoroutine(RampUpHairSimulation(newCharacter));
    }

    // Coroutine that gradually ramps up the hair simulation.
    IEnumerator RampUpHairSimulation(GameObject character)
    {
        // Try to get the SpringManager component from the character's children.
        SpringManager springManager = character.GetComponentInChildren<SpringManager>();
        if (springManager != null)
        {
            // Store the target dynamic ratio (assumed to be 1.0f, adjust if needed).
            float targetRatio = 1.0f;
            // Start with simulation disabled.
            springManager.dynamicRatio = 0f;
            float elapsed = 0f;
            while (elapsed < hairRampUpDuration)
            {
                elapsed += Time.deltaTime;
                springManager.dynamicRatio = Mathf.Lerp(0f, targetRatio, elapsed / hairRampUpDuration);
                yield return null;
            }
            springManager.dynamicRatio = targetRatio;
        }
        else
        {
            yield break;
        }
    }

    // Helper function to find the PlayerCameraRoot transform in a character.
    Transform GetCameraRoot(GameObject character)
    {
        return character.transform.Find(cameraRootName);
    }
}
