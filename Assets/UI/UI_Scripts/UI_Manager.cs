using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.InputSystem;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public Camera mainCamera;
    public Canvas canvas;
    public RectTransform rectTransform;
    public GameObject player;
    public Vector3 playerPosition;

    //Bag&Equipment
    public GameObject myBag;
    public GameObject equipmentUI_A;
    public GameObject equipmentUI_B;

    private GameObject spawnedMenu;

    public bool bagIsOpen;
    public bool equipAIsOpen_A;
    public bool equipBIsOpen_B;
    public bool playerAonUsed;

    public bool IsReady = false;

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
    public Transform EnemyHealthBarSpqwn;

    private Dictionary<GameObject, EnemyHealthBar> healthBars = new Dictionary<GameObject, EnemyHealthBar>();

    //Alex
    private StarterAssetsInputs inputController;
    private PlayerInput playerInputController;
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
        CharacterManager.SwitchPlayer += UpdatePlayerReference;
    }

    public void OnDisable()
    {
        CharacterManager.SwitchPlayer -= SwitchPlayerHealthBar;
        CharacterManager.SwitchPlayer -= UpdatePlayerReference;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        StartCoroutine(WaitForPlayerReady());
        IsReady = true;
    }
    void Update()
    {
        UpdatePlayerPosition();
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.instance.DisplayNextSentence();
        }
        
    }
    
    private void FixedUpdate()
    {
        foreach (var healthBar in healthBars.Values)
        {
            healthBar.UpdateHealthBarPos();
            //healthBar.UpdateVisible();
        }
    }
    public void UpdatePlayerPosition()
    {
        if (player != null)
        {
            playerPosition = player.transform.position;
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

        UpdateGameStateForUI(false);
    }

    public void SetMenu(GameObject menu)
    {
        spawnedMenu = menu; 
    }

    public void MenuOff()
    {
        if (spawnedMenu != null) 
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

    public void CreateEnemyHealthBar(GameObject enemy)
    {
        // 創建血條物件
        GameObject healthBarObject = Instantiate(healthBarPrefab, EnemyHealthBarSpqwn);
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            yield return StartCoroutine(WaitForHealthInitialization(enemy));
            CreateEnemyHealthBar(enemy);  
        }
    }

    public void UnregisterHealthBar(GameObject enemy)
    {
        if (healthBars.ContainsKey(enemy))
        {
            Destroy(healthBars[enemy].gameObject);  
            healthBars.Remove(enemy); 
        }
    }
    IEnumerator WaitForHealthInitialization(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        // make sure the enemyHealth is initialized
        while (enemyHealth == null || enemyHealth.currentHealth == 0)
        {
            yield return null;  
        }
        yield break;
    }
    
    IEnumerator WaitForPlayerReady()
    {
        yield return new WaitForSeconds(2f);
        UpdatePlayerReference();
        while (inputController == null || playerInputController == null)
        {
            yield return null;
            UpdatePlayerReference();
        }         
        yield break;
    }

    private void UpdatePlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
            Time.timeScale = 0.0f;
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
