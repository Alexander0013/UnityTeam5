using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnEnable()
    {
        CharacterManager.SwitchPlayer += SwitchPlayerHealthBar;
    }

    public void OnDisable()
    {
        CharacterManager.SwitchPlayer -= SwitchPlayerHealthBar;
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
    }

    public void CloseUI()
    {
        bagIsOpen =false;
        equipAIsOpen_A = false;        
        equipBIsOpen_B = false;

        myBag.SetActive(bagIsOpen);
        equipmentUI_A.SetActive(equipAIsOpen_A);
        equipmentUI_B.SetActive(equipBIsOpen_B);
    }

    public void SetMenu(GameObject menu)
    {
        spawnedMenu = menu; // 記錄當前生成的 Menu
    }

    public void MenuOff()
    {
        if (spawnedMenu != null)  //刪除右鍵選單
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
}
