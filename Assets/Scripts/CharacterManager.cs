using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterManager : MonoBehaviour
{
    // List of all character GameObjects
    public List<GameObject> characters;

    // Reference to the Cinemachine Virtual Camera
    public CinemachineVirtualCamera virtualCamera;

    // Index to track the currently active character
    private int currentCharacterIndex = 0;

    // Name of the child object used for the camera target
    private string cameraRootName = "PlayerCameraRoot";

    void Start()
    {
        // Activate only the first character by default and deactivate others
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(i == currentCharacterIndex);
        }

        // Set the camera's follow and look-at targets to the active character's camera root
        Transform cameraRoot = GetCameraRoot(characters[currentCharacterIndex]);
        if (cameraRoot != null)
        {
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
        else
        {
            Debug.LogWarning("Camera root not found on " + characters[currentCharacterIndex].name);
        }
    }

    void Update()
    {
        // Check for the key press to switch characters (e.g., using the C key)
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        // Get the current active character and store its transform data
        GameObject currentCharacter = characters[currentCharacterIndex];
        Vector3 currentPos = currentCharacter.transform.position;
        Quaternion currentRot = currentCharacter.transform.rotation;

        // Deactivate the current character
        currentCharacter.SetActive(false);

        // Determine the next character index (wrap around if needed)
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        GameObject newCharacter = characters[currentCharacterIndex];

        // Set the new character's position and rotation to match the previous character
        newCharacter.transform.position = currentPos;
        newCharacter.transform.rotation = currentRot;
        newCharacter.SetActive(true);

        // Get the new character's camera root transform
        Transform cameraRoot = GetCameraRoot(newCharacter);
        if (cameraRoot != null)
        {
            // Update the Cinemachine Virtual Camera's follow and look-at targets
            virtualCamera.Follow = cameraRoot;
            virtualCamera.LookAt = cameraRoot;
        }
        else
        {
            Debug.LogWarning("Camera root not found on " + newCharacter.name);
        }
    }

    // Helper function to find the PlayerCameraRoot transform in a character
    Transform GetCameraRoot(GameObject character)
    {
        return character.transform.Find(cameraRootName);
    }
}
