using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public GameObject myBag;
    public GameObject equipmentUI_A;
    public GameObject equipmentUI_B;
    public GameObject playerHealthBar_A;
    public GameObject playerHealthBar_B;
    public GameObject CharacterManager;

    public bool bagIsOpen;
    public bool equipAIsOpen_A;
    public bool equipBIsOpen_B;
    public bool playerAonUsed;
    private GameObject spawnedMenu;

    public void OnEnable()
    {
        //CharacterManager.OnHealthChanged += SwitchPlayerHealthBar;
    }

    public void OnDisable()
    {
        //CharacterManager.OnHealthChanged -= SwitchPlayerHealthBar;
    }

    void Start()
    {
        myBag.SetActive(bagIsOpen);
        equipmentUI_A.SetActive(equipAIsOpen_A);
        equipmentUI_B.SetActive(equipBIsOpen_B);
        playerHealthBar_A.SetActive(playerAonUsed);
        playerHealthBar_B.SetActive(!playerAonUsed);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (equipAIsOpen_A == true)
            {
                OpenEquipmentUI_B();
            }
            else
            {
                OpenEquipmentUI_A();
            }
        }
    }
    public void OpenBag()
    {        
        if (equipAIsOpen_A == true)
        {
            equipAIsOpen_A = false;
            equipmentUI_A.SetActive(equipAIsOpen_A);
        }

        if (equipBIsOpen_B == true)
        {
            equipBIsOpen_B = false;
            equipmentUI_B.SetActive(equipBIsOpen_B);
        }
        bagIsOpen = !bagIsOpen;
        myBag.SetActive(bagIsOpen);
        MenuOff();
    }


    public void OpenEquipmentUI_A()
    {
        if (bagIsOpen == true)
        {
            bagIsOpen = !bagIsOpen;
            myBag.SetActive(bagIsOpen);
        }
        if (equipBIsOpen_B == true)
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
        if (bagIsOpen == true)
        {
            bagIsOpen = !bagIsOpen;
            myBag.SetActive(bagIsOpen);
        }
        if (equipAIsOpen_A == true)
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
        bagIsOpen=false;
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
        playerHealthBar_A.SetActive(playerAonUsed);
        playerHealthBar_B.SetActive(!playerAonUsed);
    }
    
}
