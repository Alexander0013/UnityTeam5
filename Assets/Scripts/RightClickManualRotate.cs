using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightClickManualRotate : MonoBehaviour
{
    public float rotationSpeed = 1.5f;
    private bool isRotating = false;

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }

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
