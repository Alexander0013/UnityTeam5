using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    Transform playerTransform;
    Transform mainCamera;
    public GameObject Player_A;
    public GameObject Player_B;

    public CharacterManager characterManager;

    bool switchPlayer = true;

    public void OnEnable()
    {
        characterManager.SwitchPlayer += switchFollower;
    }

    public void OnDisable()
    {
        characterManager.SwitchPlayer -= switchFollower;
    }

    private void Start()
    {
        Player_A = characterManager.characters[0];
        Player_B = characterManager.characters[1];
        playerTransform = Player_A.transform;
        mainCamera = Camera.main.transform;
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
        base.transform.rotation = Quaternion.Euler(90f,mainCamera.eulerAngles.y, 0);
    }
}
