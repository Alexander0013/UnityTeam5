using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

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

    bool bagIsOpen;
    bool equipAIsOpen_A;
    bool equipBIsOpen_B;
    public bool playerAonUsed = true;
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

    public GameObject bossHealthBarPrefab;
    public Transform bossHealthBarSpqwn;
    //public GameObject bossHealthBar;

    private Dictionary<GameObject, EnemyHealthBar> healthBars = new Dictionary<GameObject, EnemyHealthBar>();

    //Alex
    public StarterAssetsInputs inputController;
    public PlayerInput playerInputController;

    //Task Tip
    public GameObject taskTip;
    TextMeshProUGUI taskTipText;
    private List<ItemGiver> itemGivers = new List<ItemGiver>();

    //Dialogue
    public GameObject dialogueBox;
    public DialogueTrigger npcDT;
    public bool inDialogueRange = false;
    public Dialogue dialogue;
    public bool getMission = false;
    public bool missionDone = false;
    public bool startTalking = false; //control if talking or not

    public delegate void StartDialogue();
    public StartDialogue startDialogue;

    //Protal&Tip
    public GameObject interactionText; 
    public bool inProtalRange = false;
    public int targetSceneIndex;

    //Treasure
    public bool treasureCanOpen = false;
    public TreasureTrigger currentTreasure;

    //Item Add
    public GameObject itemDisplayPrefab;
    public Transform itemDisplayContainer;
    private List<Item> displayedItems = new List<Item>();

    //Guide
    public GameObject guide;
    private Animator guideAnimator;
    bool GuideIsOn = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void OnDisable()
    {
        characterManager.SwitchPlayer -= SwitchPlayerHealthBar;
        characterManager.SwitchPlayer -= UpdatePlayerReference;
        DialogueManager.instance.missonStart -= GetMission;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void Start()
    {
        IsReady = false;
        this.GetComponent<CanvasGroup>().alpha = 0;

        interactionText.SetActive(false);

        canvasGroup_A = playerHealthBar_A.GetComponent<CanvasGroup>();
        canvasGroup_B = playerHealthBar_B.GetComponent<CanvasGroup>();
        miniCanvasGroup_A = miniBar_A.GetComponent<CanvasGroup>();
        miniCanvasGroup_B = miniBar_B.GetComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        IsReady = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
            return;

        IsReady = false;

        if(this.GetComponent<CanvasGroup>().alpha == 0)
        {
            this.GetComponent<CanvasGroup>().alpha = 1;
        }

        InitializeSceneObjects();
        if (!playerHealthBar_A.activeSelf&&player!=null)
        {
            playerHealthBar_A.SetActive(true);
            playerHealthBar_B.SetActive(true);
            miniBar_A.SetActive(true);
            miniBar_B.SetActive(true);
        }

        if (scene.buildIndex == 1 )
        {
            StartCoroutine(WaitForDM());
            if (!GuideIsOn)
            {
                StartCoroutine(WaitForGuide());
                GuideIsOn = true;
            }            
            taskTipText = taskTip.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (scene.buildIndex == 2)
        {
            StartCoroutine(GenerateHealthBarsForEnemies());
            CreateBossHealthBar();
            //bossHealthBar.SetActive(true);
            //bossHealthBar.GetComponent<CanvasGroup>().alpha = 0;
        }       

        IsReady = true;
    }
    void OnSceneUnloaded(Scene scene)
    {
        if (scene.buildIndex == 2)
        {
            var keys = new List<GameObject>(healthBars.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                var enemy = keys[i];
                UnregisterHealthBar(enemy);
            }
        }        
    }
    void InitializeSceneObjects()
    {       
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (characterManager == null)
        {
            characterManager = CharacterManager.instance;
            characterManager.SwitchPlayer += SwitchPlayerHealthBar;
            characterManager.SwitchPlayer += UpdatePlayerReference;
        }
        if(player == null)
        {
            StartCoroutine(WaitForPlayerReady());
        }
    }
    void Update()
    {
        UpdatePlayerPosition();
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenBag();
        }
        //Protal
        if (inProtalRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneController.instance.StartCoroutine(SceneController.instance.FadeOutAndLoadSingle(targetSceneIndex));
            inProtalRange = false;
            UI_Manager.instance.HideInteractionText();
        }
        //Dialogue
        if (inDialogueRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue(dialogue,startTalking);
            UI_Manager.instance.HideInteractionText();
        }
        //Treasure
        if (treasureCanOpen && Input.GetKeyDown(KeyCode.E) && currentTreasure != null)
        {
            currentTreasure.AddItemsToInventory();  
            UI_Manager.instance.HideInteractionText();
            Destroy(currentTreasure.gameObject);
        }
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            InventoryManager.instance.DropAll();
        }
    }
    private void FixedUpdate()
    {
        //Enemy HealthBar
        foreach (var healthBar in healthBars.Values)
        {
            healthBar.UpdateHealthBarPos();
        }
    }
    IEnumerator WaitUntilUIIsReady()
    {
        while (UI_Manager.instance.IsReady!=true)
        {
            yield return null;
        }
        yield return null;
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
    public void SetPlayerHealthBar()
    {
        PlayerHealth playerHealth_A = characterManager.characters[0].GetComponent<PlayerHealth>();
        PlayerHealth playerHealth_B = characterManager.characters[1].GetComponent<PlayerHealth>();
        playerHealthBar_A.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_A;
        miniBar_A.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_A;
        playerHealthBar_B.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_B;
        miniBar_B.GetComponent<PlayerHealthBar>().playerHealth = playerHealth_B;
    }
    //Enemy HealthBar
    public void CreateEnemyHealthBar(GameObject enemy)
    {
        GameObject healthBarObject = Instantiate(healthBarPrefab, EnemyHealthBarSpqwn);
        healthBarObject.transform.localScale = new Vector3(1, 1, 1);
        healthBarObject.transform.localRotation = Quaternion.identity;

        EnemyHealthBar healthBarScript = healthBarObject.GetComponent<EnemyHealthBar>();
        healthBarScript.InitializeHealthBar(enemy);

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

    //Boss Health Bar
    public void CreateBossHealthBar()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (bossHealthBarSpqwn == null)
        {
            Debug.Log("bossHealthBarSpqwn is null");
        }
        GameObject healthBarObject = Instantiate(bossHealthBarPrefab, bossHealthBarSpqwn);
        RectTransform rectTransform = healthBarObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3 (0,0,0);
        
        BossHealthBar bossBarScript = healthBarObject.GetComponent<BossHealthBar>();
        bossBarScript.InitializeHealthBar(boss);
    }
    //Get mouse from player
    IEnumerator WaitForPlayerReady()
    {
        if (player == null)
        {
            yield return null;
            UpdatePlayerReference();
            while (inputController == null || playerInputController == null)
            {
                yield return null;
                UpdatePlayerReference();
                Debug.Log("Waiting for player to be ready...");
            }
            SetPlayerHealthBar();
            yield break;
        }
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
    public void UpdateGameStateForUI(bool uiOpen)
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
        DialogueManager.instance.missonStart += GetItemFromNPC;
    }
    void GetMission()
    {
        taskTip.SetActive(true);
        taskTipText.text = "《幻界之鑰》(0/5)";
    }
    void GetItemFromNPC()
    {
        npcDT.GetItemFromNPC();
    }
    public void MissionDone()
    {
        taskTip.SetActive(false);
        //完成音效
    }
    public void RegisterItemGiver(ItemGiver itemGiver)
    {
        itemGivers.Add(itemGiver);
        itemGiver.ItemAdded += OnItemAdded;
    }
    public void UnregisterItemGiver(ItemGiver itemGiver)
    {
        itemGiver.ItemAdded -= OnItemAdded;
        itemGivers.Remove(itemGiver);
    }
   
    //hint    
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
    public void TriggerDialogue(Dialogue dialogue,bool starttalking)
    {
        if (!starttalking)
        {
            DialogueManager.instance.StartDialogue(dialogue);
            startDialogue?.Invoke();
            startTalking = true;
        }
        else
        {
            DialogueManager.instance.DisplayNextSentence();
        }
    }
    //Item Add
    public void OnItemAdded(Item item)
    {
        //Debug.Log("Item added: " + item.itemName);
        displayedItems.Add(item);
        if (getMission)
        {
            if (InventoryManager.instance.GetItemAmount(Item.ItemType.Other) == 5)
            {
                missionDone = true;
            }
        }
        StartCoroutine(DisplayItemWithDelay(item));               
    }
    IEnumerator DisplayItemWithDelay(Item item)
    {
        if (displayedItems.Count > 0)
        {
            yield return new WaitForSeconds(0.6f * displayedItems.Count); // 設定顯示物品之間的間隔，這裡是0.5秒
        }
        GameObject itemDisplay = Instantiate(itemDisplayPrefab, itemDisplayContainer);
        itemDisplay.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
        Image itemImage = itemDisplay.transform.Find("Mask/Item Image").GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.itemImage;  // 設置物品圖片
        }
        RectTransform rectTransform = itemDisplay.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);

        StartCoroutine(AnimateItemDisplay(itemDisplay, rectTransform));

        if (item.itemType == Item.ItemType.Other)
        {

            taskTipText.text = "《幻界之鑰》(" + item.itemHeld + "/5)";
        }
        displayedItems.Remove(item);
    }
    private IEnumerator AnimateItemDisplay(GameObject itemDisplay, RectTransform rectTransform)
    {
        float elapsedTime = 0f;
        float duration = 1.5f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, startPos.y + 300f);  // 垂直上浮100單位
        CanvasGroup canvasGroup = itemDisplay.GetComponent<CanvasGroup>();
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 0f;

        // 使用 Lerp 來平滑地上浮
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime-3 / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha; 
        rectTransform.anchoredPosition = targetPos;  
        
        yield return new WaitForSeconds(1.5f);
       
        Destroy(itemDisplay);
    }
    //Guide
    void ShowGuide()
    {
        guide.SetActive(true);
        guideAnimator = guide.GetComponent<Animator>();
        guideAnimator.SetTrigger("SetGuide");
    }
    IEnumerator WaitForGuide()
    {
        yield return new WaitForSeconds(2f);
        ShowGuide();        
        yield return new WaitForSeconds(5f);
        guideAnimator.SetTrigger("GuideOff");
        yield break;
    }
   


}

