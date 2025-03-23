using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    // 單例實例
    private static CameraManager _instance;

    // 公共的靜態屬性來訪問單例
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("CameraManager");
                    _instance = obj.AddComponent<CameraManager>();
                }
            }

            return _instance;
        }
    }

    // 用來存儲 Cinemachine 虛擬相機的變數
    private CinemachineVirtualCamera _virtualCamera;

    // 在 Awake 中初始化
    private void Awake()
    {
        // 確保在場景中只有一個 CameraManager 實例
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);  // 讓相機管理器在場景切換時不被銷毀
        }
        else if (_instance != this)
        {
            Destroy(gameObject);  // 如果已有實例，則銷毀自己
        }

        // 嘗試找到場景中的 Cinemachine Virtual Camera
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (_virtualCamera == null)
        {
            Debug.Log("Cinemachine Virtual Camera not found in the scene.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (_virtualCamera == null)
        {
            Debug.Log("Cinemachine Virtual Camera not found in the scene.");
        }
    }
}