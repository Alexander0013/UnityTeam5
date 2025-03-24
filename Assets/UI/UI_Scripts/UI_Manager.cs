using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;
//using static UnityEditor.Progress;
using UnityEngine.SceneManagement;
using System;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public Camera mainCamera;
    public Canvas canvas;
    public RectTransform rectTransform;
    public GameObject player;
    public CharacterManager characterManager;
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

    //EnemyHealthBar
    public GameObject healthBarPrefab;
    public Transform EnemyHealthBarSpqwn;
    public GameObject bossHealthBar;

    private Dictionary<GameObject, EnemyHealthBar> healthBars = new Dictionary<GameObject, EnemyHealthBar>();

    //Alex
    private StarterAssetsInputs inputController;
    private PlayerInput playerInputController;

    //Task Tip
    public GameObject taskTip;
    TextMeshProUGUI taskTipText;
    private List<ItemGiver> itemGivers = new List<ItemGiver>();

    //Dialogue
    public bool inDialogueRange = false;
    public Dialogue dialogue;
    public bool isTriggered = false;

    //Protal
    public GameObject interactionText; 
    public bool inProtalRange = false;
    public int targetSceneIndex;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject); // �קK���������ɺR������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(WaitForDM());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        if (characterManager != null)
        {
            characterManager.SwitchPlayer -= SwitchPlayerHealthBar;
            characterManager.SwitchPlayer -= UpdatePlayerReference;
        }
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.missonStart -= GetMission;
        }
        //characterManager.SwitchPlayer -= SwitchPlayerHealthBar;
        //characterManager.SwitchPlayer -= UpdatePlayerReference;
        //DialogueManager.instance.missonStart -= GetMission;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        IsReady = false;

        canvasGroup_A = playerHealthBar_A.GetComponent<CanvasGroup>();
        canvasGroup_B = playerHealthBar_B.GetComponent<CanvasGroup>();
        miniCanvasGroup_A = miniBar_A.GetComponent<CanvasGroup>();
        miniCanvasGroup_B = miniBar_B.GetComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        InitializeSceneObjects();
        SetPlayerHealthBar();

        IsReady = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsReady = false;
        
        if (scene.name == "Temple")
        {
            taskTipText = taskTip.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (scene.name =="PureNature")
        {
            StartCoroutine(GenerateHealthBarsForEnemies());
            bossHealthBar.SetActive(true);
        }

        IsReady = true;
    }
    void InitializeSceneObjects()
    {
        //mainCamera = FindObjectOfType<Camera>();
        mainCamera = Camera.main;
        characterManager = CharacterManager.instance;
        characterManager.SwitchPlayer += SwitchPlayerHealthBar;
        characterManager.SwitchPlayer += UpdatePlayerReference;

        StartCoroutine(WaitForPlayerReady());
    }
    void Update()
    {
        UpdatePlayerPosition();
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }

        if (inProtalRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneController.instance.StartCoroutine(SceneController.instance.FadeOutAndLoadSingle(targetSceneIndex));
            inProtalRange = false;
            UI_Manager.instance.HideInteractionText();
        }

        if (inDialogueRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue(dialogue,isTriggered);
            isTriggered = true;
            UI_Manager.instance.HideInteractionText();
        }
    }
    private void FixedUpdate()
    {
        foreach (var healthBar in healthBars.Values)
        {
            healthBar.UpdateHealthBarPos();
        }
    }
    public void UpdatePlayerPosition()
    {
        if (player != null)
        {
            playerPosition = player.transform.position;
        }
    }
    //Switch Bag & Equipment
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
    //Right Click Menu
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
    //Switch Player HealthBar
    public void SetPlayerHealthBar()
    {
        PlayerHealth playerHealth_A = characterManager.characters[0].GetComponent<PlayerHealth>();
        PlayerHealth playerHealth_B = characterManager.characters[1].GetComponent<PlayerHealth>();
        playerHealthBar_A.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_A;
        miniBar_A.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_A;
        playerHealthBar_B.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_B;
        miniBar_B.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_B;
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
    //Enemy HealthBar
    public void CreateEnemyHealthBar(GameObject enemy)
    {
        // �Ыئ������
        GameObject healthBarObject = Instantiate(healthBarPrefab, EnemyHealthBarSpqwn);
        healthBarObject.transform.localScale = new Vector3(1, 1, 1);
        healthBarObject.transform.localRotation = Quaternion.identity;

        // ��l�Ʀ�����
        EnemyHealthBar healthBarScript = healthBarObject.GetComponent<EnemyHealthBar>();
        healthBarScript.InitializeHealthBar(enemy);

        // ���U�� UIManager
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
    
    //Get mouse from player
    IEnumerator WaitForPlayerReady()
    {
        yield return null;
        UpdatePlayerReference();
        while (inputController == null || playerInputController == null)
        {
            yield return null;
            UpdatePlayerReference();
            Debug.Log("Waiting for player to be ready...");
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

    //Task Tip

    IEnumerator WaitForDM()
    {       
        while (DialogueManager.instance == null)
        {
            yield return null;
        }
        DialogueManager.instance.missonStart += GetMission;        
    }
    void GetMission()
    {
        taskTip.SetActive(true);
        taskTipText.text = "�������ȹD��]0/5�^";

    }
    public void RegisterItemGiver(ItemGiver itemGiver)
    {
        // ���U���~�ƥ�
        itemGivers.Add(itemGiver);
        itemGiver.ItemAdded += OnItemAdded;
    }
    public void UnregisterItemGiver(ItemGiver itemGiver)
    {
        // �����q�\���~�ƥ�
        itemGiver.ItemAdded -= OnItemAdded;
        itemGivers.Remove(itemGiver);
    }
    private void OnItemAdded(Item item)
    {
        if (item.itemType == Item.ItemType.Other)
        {
            
            taskTipText.text = "�������ȹD��]"+item.itemHeld+"/5�^";
        }
    }

    //For Protal & Pick up hint
    
    public void ShowInteractionText(string text)
    {
        interactionText.SetActive(true);
        interactionText.GetComponentInChildren<TextMeshProUGUI>().text = text;        
    }

    public void HideInteractionText()
    {
        interactionText.SetActive(false);  
    }

    //Dialogue
    public void TriggerDialogue(Dialogue dialogue,bool isTriggered)
    {
        if (!isTriggered)
        {
            DialogueManager.instance.StartDialogue(dialogue);
            isTriggered = true;
        }
        else
        {
            DialogueManager.instance.DisplayNextSentence();
        }
    }
}
