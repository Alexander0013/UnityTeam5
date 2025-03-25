//using UnityEngine;

//public class MinimapWorldObject : MonoBehaviour
//{
//    [SerializeField]
//    private bool followObject = false; // 是否要跟隨物體
//    [SerializeField]
//    private Sprite minimapIcon; // 物體的迷你地圖圖示
//    public Sprite MinimapIcon => minimapIcon; // 取得物體的迷你地圖圖示

//    private void Start()
//    {
//        if (MinimapController.Instance != null)
//        {
//            MinimapController.Instance.RegisterMinimapWorldObject(this, followObject);
//        }
//        else
//        {
//            Debug.LogError("MinimapController instance is not initialized.");
//        }
//    }

//    private void OnDestroy()
//    {
//        // 在物體銷毀時移除此物體的圖示
//        MinimapController.Instance.RemoveMinimapWorldObject(this);
//    }
//}
