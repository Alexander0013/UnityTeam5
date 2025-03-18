using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    Transform playerTransform;
    Transform mainCamera;
    public GameObject Player_A;
    public GameObject Player_B;

    public CharacterManager CharacterManager;

    bool switchPlayer = true;

    public void OnEnable()
    {
        CharacterManager.SwitchPlayer += switchFollower;
    }

    public void OnDisable()
    {
        CharacterManager.SwitchPlayer -= switchFollower;
    }

    private void Start()
    {
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
        base.transform.rotation = Quaternion.Euler(90f, mainCamera.eulerAngles.y, 0f);
    }
}
