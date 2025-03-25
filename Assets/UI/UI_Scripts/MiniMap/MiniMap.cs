using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    Transform playerTransform;
    Camera mainCamera;
    GameObject Player_A;
    GameObject Player_B;

    CharacterManager characterManager;

    bool switchPlayer = true;

   
    public void OnDisable()
    {
        characterManager.SwitchPlayer -= switchFollower;
    }

    private void Start()
    {
        characterManager = CharacterManager.instance;
        characterManager.SwitchPlayer += switchFollower;

        Player_A = characterManager.characters[0];
        Player_B = characterManager.characters[1];
        playerTransform = Player_A.transform;
        mainCamera = Camera.main;

        if (characterManager == null) { Debug.Log("characterManager == null"); }
        if (Player_A == null) { Debug.Log("Player_A == null"); }
        if (Player_B == null) { Debug.Log("Player_B == null"); }
        if (mainCamera == null) { Debug.Log("mainCamera == null"); }
    }

    private void switchFollower()
    {
        Debug.Log("switch player to"+playerTransform);
        if (switchPlayer)
        {
            playerTransform = Player_B.transform;            
        }
        else 
        {
            playerTransform = Player_A.transform;
        }
        switchPlayer = !switchPlayer;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position;
        newPosition.y = base.transform.position.y;
        base.transform.position = newPosition;
        base.transform.rotation = Quaternion.Euler(90f,mainCamera.transform.eulerAngles.y, 0);
    }
}
