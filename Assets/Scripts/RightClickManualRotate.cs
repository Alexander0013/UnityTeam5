using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class RightClickManualRotate : MonoBehaviour
{
    public float rotationSpeed = 1.5f;
    private CinemachineInputProvider inputProvider;
    void Start()
    {
        inputProvider = GetComponent<CinemachineInputProvider>();
    }

    void Update()
    {
        bool isRotating = Mouse.current.rightButton.isPressed;
        if (inputProvider != null)
            inputProvider.enabled = isRotating;
        if (isRotating && CharacterManager.instance != null)
        {
            Debug.Log("right click rotate camera");
            GameObject currentPlayer = CharacterManager.instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                float yaw = mouseDelta.x * rotationSpeed;

                Transform camRoot = currentPlayer.transform.Find("PlayerCameraRoot");
                if (camRoot != null)
                {
                    camRoot.Rotate(Vector3.up, yaw, Space.World);
                }
            }
        }
    }
}
