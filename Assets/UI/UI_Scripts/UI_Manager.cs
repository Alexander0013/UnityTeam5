using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public Camera mainCamera;
    public Canvas canvas;
    public RectTransform rectTransform;


    //Bag&Equipment
    public GameObject myBag;
    public GameObject equipmentUI_A;
    public GameObject equipmentUI_B;

    private GameObject spawnedMenu;

    public bool bagIsOpen;
    public bool equipAIsOpen_A;
    public bool equipBIsOpen_B;
    public bool playerAonUsed;

    //PlayerHealthBar
    public GameObject playerHealthBar_A;
    private CanvasGroup canvasGroup_A;
    public GameObject playerHealthBar_B;
    private CanvasGroup canvasGroup_B;
    public GameObject miniBar_A;
    private CanvasGroup miniCanvasGroup_A;
    public GameObject miniBar_B;
    private CanvasGroup miniCanvasGroup_B;
    public Button botton_A;
    public Button botton_B;

    public CharacterManager CharacterManager;

    //EnemyHealthBar
    public GameObject healthBarPrefab;    
    private Dictionary<GameObject, EnemyHealthBar> healthBars = new Dictionary<GameObject, EnemyHealthBar>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 避免場景切換時摧毀物件
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        botton_A.interactable = true;
        botton_B.interactable = false;

        canvasGroup_A = playerHealthBar_A.GetComponent<CanvasGroup>();
        canvasGroup_B = playerHealthBar_B.GetComponent<CanvasGroup>();
        miniCanvasGroup_A = miniBar_A.GetComponent<CanvasGroup>();
        miniCanvasGroup_B = miniBar_B.GetComponent<CanvasGroup>();

        canvasGroup_A.alpha = 1.0f;
        miniCanvasGroup_A.alpha = 0.0f;
        canvasGroup_B.alpha = 0.0f;
        miniCanvasGroup_B.alpha = 1.0f;

        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        StartCoroutine(GenerateHealthBarsForEnemies());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }        
    }
    
    private void FixedUpdate()
    {
        foreach (var healthBar in healthBars.Values)
        {
            healthBar.UpdateHealthBarPos();
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
            miniCanvasGroup_A.alpha = 0;
            miniCanvasGroup_B.alpha = 1;
            botton_A.interactable = true;
            botton_B.interactable = false;
        }
        else
        {
            canvasGroup_B.alpha = 1;
            canvasGroup_A.alpha = 0;
            miniCanvasGroup_A.alpha = 1;
            miniCanvasGroup_B.alpha = 0;
            botton_B.interactable = true;
            botton_A.interactable = false;
        }
    }

    public void CreateHealthBar(GameObject enemy)
    {
        // 創建血條物件
        GameObject healthBarObject = Instantiate(healthBarPrefab, canvas.transform);
        healthBarObject.transform.localScale = new Vector3(1, 1, 1);
        healthBarObject.transform.localRotation = Quaternion.identity;

        // 初始化血條控制器
        EnemyHealthBar healthBarScript = healthBarObject.GetComponent<EnemyHealthBar>();
        healthBarScript.InitializeHealthBar(enemy);

        // 註冊到 UIManager
        healthBars.Add(enemy, healthBarScript);
    }

    IEnumerator GenerateHealthBarsForEnemies()
     {
        // 查找所有帶有 "Enemy" 標籤的物件
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            yield return StartCoroutine(WaitForHealthInitialization(enemy));
            CreateHealthBar(enemy);  // 為每個敵人創建血條
        }
     }

    public void UnregisterHealthBar(GameObject enemy)
    {
        if (healthBars.ContainsKey(enemy))
        {
            Destroy(healthBars[enemy].gameObject);  // 銷毀血條物件
            healthBars.Remove(enemy);  // 從字典中移除
        }
    }
    IEnumerator WaitForHealthInitialization(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        // 確保 EnemyHealth 初始化完成，這裡等待直到生命值不為 0
        while (enemyHealth == null || enemyHealth.currentHealth == 0)
        {
            yield return null;  // 等待直到下一幀
        }

        // 此時敵人的 Health 已經初始化完成
        yield break;
    }
         
    //public void SetAllHealthBarsTransparent()
    //{
    //    foreach (var healthBar in healthBars.Values)
    //    {
    //        CanvasGroup canvasGroup = healthBar.GetComponent<CanvasGroup>();
    //        if (canvasGroup != null)
    //        {
    //            canvasGroup.alpha = 0; // 設定透明
    //        }
    //    }
    //}

    //public void SetAllHealthBarsOpaque()
    //{
    //    foreach (var healthBar in healthBars.Values)
    //    {
    //        CanvasGroup canvasGroup = healthBar.GetComponent<CanvasGroup>();
    //        if (canvasGroup != null)
    //        {
    //            canvasGroup.alpha = 1; // 恢復不透明
    //        }
    //    }
    //}

    }
