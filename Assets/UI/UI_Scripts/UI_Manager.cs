using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class UI_Manager : MonoBehaviour
{
    public GameObject myBag;
    public GameObject equipmentUI_A;
    public GameObject equipmentUI_B;

    public GameObject playerHealthBar_A;
    private CanvasGroup canvasGroup_A;
    public GameObject playerHealthBar_B;
    private CanvasGroup canvasGroup_B;

    public CharacterManager CharacterManager;

    public bool bagIsOpen;
    public bool equipAIsOpen_A;
    public bool equipBIsOpen_B;
    public bool playerAonUsed;

    private GameObject spawnedMenu;

    private StarterAssetsInputs inputController;
    private PlayerInput playerInputController;
    public void OnEnable()
    {
        CharacterManager.SwitchPlayer += SwitchPlayerHealthBar;
        CharacterManager.SwitchPlayer += UpdatePlayerReference;
    }

    public void OnDisable()
    {
        CharacterManager.SwitchPlayer -= SwitchPlayerHealthBar;
        CharacterManager.SwitchPlayer -= UpdatePlayerReference;
    }

    void Start()
    {
        myBag.SetActive(bagIsOpen);
        equipmentUI_A.SetActive(equipAIsOpen_A);
        equipmentUI_B.SetActive(equipBIsOpen_B);
        

        canvasGroup_A = playerHealthBar_A.GetComponent<CanvasGroup>();
        canvasGroup_B = playerHealthBar_B.GetComponent<CanvasGroup>();

        canvasGroup_A.alpha = 1.0f;
        canvasGroup_B.alpha = 0.0f;

        UpdatePlayerReference();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }
    }
    public void OpenBag()
    {
        if (equipAIsOpen_A)
        {
            equipAIsOpen_A = !equipAIsOpen_A;
            equipmentUI_A.SetActive(equipAIsOpen_A);
        }

        if (equipBIsOpen_B)
        {
            equipBIsOpen_B = !equipBIsOpen_B;
            equipmentUI_B.SetActive(equipBIsOpen_B);
        }
        bagIsOpen = !bagIsOpen;
        myBag.SetActive(bagIsOpen);
        MenuOff();

        UpdateGameStateForUI(bagIsOpen);
    }


    public void OpenEquipmentUI_A()
    {
        if (bagIsOpen)
        {
            bagIsOpen = !bagIsOpen;
            myBag.SetActive(bagIsOpen);
        }
        if (equipBIsOpen_B)
        {
            equipBIsOpen_B = !equipBIsOpen_B;
            equipmentUI_B.SetActive(equipBIsOpen_B);
        }
        equipAIsOpen_A = !equipAIsOpen_A;
        equipmentUI_A.SetActive(equipAIsOpen_A);

        MenuOff();
        UpdateGameStateForUI(equipAIsOpen_A);
    }

    public void OpenEquipmentUI_B()
    {
        if (bagIsOpen)
        {
            bagIsOpen = !bagIsOpen;
            myBag.SetActive(bagIsOpen);
        }
        if (equipAIsOpen_A)
        {
            equipAIsOpen_A = !equipAIsOpen_A;
            equipmentUI_A.SetActive(equipAIsOpen_A);
        }
        equipBIsOpen_B = !equipBIsOpen_B;
        equipmentUI_B.SetActive(equipBIsOpen_B);

        MenuOff();
        UpdateGameStateForUI(equipBIsOpen_B);
    }

    public void CloseUI()
    {
        bagIsOpen =false;
        equipAIsOpen_A = false;        
        equipBIsOpen_B = false;

        myBag.SetActive(bagIsOpen);
        equipmentUI_A.SetActive(equipAIsOpen_A);
        equipmentUI_B.SetActive(equipBIsOpen_B);
        UpdateGameStateForUI(false);
    }

    public void SetMenu(GameObject menu)
    {
        spawnedMenu = menu; // �O�����e�ͦ��� Menu
    }

    public void MenuOff()
    {
        if (spawnedMenu != null)  //�R���k����
        {
            Destroy(spawnedMenu);
            spawnedMenu = null;
        }
    }

    public void SwitchPlayerHealthBar()
    {
        playerAonUsed = !playerAonUsed;
        if (playerAonUsed)
        {
            canvasGroup_B.alpha = 0;
            canvasGroup_A.alpha = 1;
        }
        else
        {
            canvasGroup_B.alpha = 1;
            canvasGroup_A.alpha = 0;
        }
    }
    private void UpdatePlayerReference()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputController = player.GetComponent<StarterAssetsInputs>();
            playerInputController = player.GetComponent<PlayerInput>();
        }
        else
        {
            Debug.LogWarning("Active player not found! Ensure the active player is tagged 'Player'.");
        }
    }
    private void UpdateGameStateForUI(bool uiOpen)
    {
        if (uiOpen)
        {
            Time.timeScale = 0.05f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (inputController != null)
            {
                inputController.cursorLocked = false;
                inputController.enabled = false;
            }
            if (playerInputController != null)
            {
                playerInputController.enabled = false;
            }
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (inputController != null)
            {
                inputController.cursorLocked = true;
                inputController.enabled = true;
            }
            if (playerInputController != null)
            {
                playerInputController.enabled = true;
            }
        }
    }


}
