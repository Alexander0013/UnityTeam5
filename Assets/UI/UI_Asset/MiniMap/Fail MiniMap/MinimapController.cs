using System.Collections.Generic;
using UnityEngine;

public enum MinimapMode
{
    Mini, Fullscreen
}

public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance;

    // 地圖範圍大小
    [SerializeField]
    Vector2 worldSize;

    // 當全屏顯示時，地圖的尺寸
    [SerializeField]
    Vector2 fullScreenDimensions = new Vector2(1000, 1000);

    // 缩放速度
    [SerializeField]
    float zoomSpeed = 0.1f;

    // 最大縮放比例
    [SerializeField]
    float maxZoom = 10f;

    // 最小縮放比例
    [SerializeField]
    float minZoom = 1f;

    // 地圖的 RectTransform (滾動視圖的大小和位置)
    [SerializeField]
    RectTransform scrollViewRectTransform;

    // 地圖內容的 RectTransform (包含所有圖示)
    [SerializeField]
    RectTransform contentRectTransform;

    // 地圖圖示預設
    [SerializeField]
    MiniMapIcon minimapIconPrefab;

    // 用來儲存地圖的轉換矩陣
    Matrix4x4 transformationMatrix;

    // 當前地圖模式
    private MinimapMode currentMiniMapMode = MinimapMode.Mini;
    // 跟隨物體的圖示
    private MiniMapIcon followIcon;
    // 初始的滾動視圖大小和位置
    private Vector2 scrollViewDefaultSize;
    private Vector2 scrollViewDefaultPosition;
    // 儲存所有地圖物件與其圖示的對應關係
    Dictionary<MinimapWorldObject, MiniMapIcon> miniMapWorldObjectsLookup = new Dictionary<MinimapWorldObject, MiniMapIcon>();

    private void Awake()
    {
        Instance = this;
        // 記錄滾動視圖的預設大小和位置
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
    }

    private void Start()
    {
        CalculateTransformationMatrix(); // 計算轉換矩陣
    }

    private void Update()
    {
        //// 按下 M 鍵切換地圖模式（迷你模式或全屏模式）
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    SetMinimapMode(currentMiniMapMode == MinimapMode.Mini ? MinimapMode.Fullscreen : MinimapMode.Mini);
        //}

        //// 滾輪縮放地圖
        //float zoom = Input.GetAxis("Mouse ScrollWheel");
        //ZoomMap(zoom);
        UpdateMiniMapIcons();  // 更新地圖圖示的位置和狀態
        CenterMapOnIcon();     // 將地圖中心定位到跟隨圖示的位置
        //Debug.Log(contentRectTransform.anchoredPosition);
    }

    // 註冊地圖物件，並可選擇是否跟隨該物體
    public void RegisterMinimapWorldObject(MinimapWorldObject miniMapWorldObject, bool followObject = false)
    {
        var minimapIcon = Instantiate(minimapIconPrefab);
        minimapIcon.transform.SetParent(contentRectTransform);
        minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        miniMapWorldObjectsLookup[miniMapWorldObject] = minimapIcon;

        if (followObject)
            followIcon = minimapIcon;
    }

    // 移除地圖物件
    public void RemoveMinimapWorldObject(MinimapWorldObject minimapWorldObject)
    {
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out MiniMapIcon icon))
        {
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);
            Destroy(icon.gameObject);
        }
    }

    //// 設置地圖模式（迷你模式或全屏模式）
    //private void SetMinimapMode(MinimapMode mode)
    //{
    //    const float defaultScaleWhenFullScreen = 1.3f;

    //    if (mode == currentMiniMapMode)
    //        return;

    //    switch (mode)
    //    {
    //        case MinimapMode.Mini:
    //            scrollViewRectTransform.sizeDelta = scrollViewDefaultSize;
    //            scrollViewRectTransform.anchorMin = Vector2.one;
    //            scrollViewRectTransform.anchorMax = Vector2.one;
    //            scrollViewRectTransform.pivot = Vector2.one;
    //            scrollViewRectTransform.anchoredPosition = scrollViewDefaultPosition;
    //            currentMiniMapMode = MinimapMode.Mini;
    //            break;
    //        case MinimapMode.Fullscreen:
    //            scrollViewRectTransform.sizeDelta = fullScreenDimensions;
    //            scrollViewRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    //            scrollViewRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    //            scrollViewRectTransform.pivot = new Vector2(0.5f, 0.5f);
    //            scrollViewRectTransform.anchoredPosition = Vector2.zero;
    //            currentMiniMapMode = MinimapMode.Fullscreen;
    //            contentRectTransform.transform.localScale = Vector3.one * defaultScaleWhenFullScreen;
    //            break;
    //    }
    //}

    //// 地圖縮放
    //private void ZoomMap(float zoom)
    //{
    //    if (zoom == 0)
    //        return;

    //    float currentMapScale = contentRectTransform.localScale.x;
    //    float zoomAmount = (zoom > 0 ? zoomSpeed : -zoomSpeed) * currentMapScale;
    //    float newScale = currentMapScale + zoomAmount;
    //    float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
    //    contentRectTransform.localScale = Vector3.one * clampedScale;
    //}

    // 將地圖中心定位到跟隨的物體
    private void CenterMapOnIcon()
    {
        if (followIcon != null)
        {
            float mapScale = contentRectTransform.transform.localScale.x;
            contentRectTransform.anchoredPosition = (-followIcon.RectTransform.anchoredPosition * mapScale);
        }
        Debug.Log(followIcon.RectTransform.anchoredPosition);
    }

    // 更新所有地圖圖示的位置、旋轉和縮放
    private void UpdateMiniMapIcons()
    {
        float iconScale = 1 / contentRectTransform.transform.localScale.x;
        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;
            var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

            miniMapIcon.RectTransform.anchoredPosition = mapPosition;
            var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
            miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
            miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
        }
    }

    // 將世界座標轉換為地圖座標
    private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        var pos = new Vector2(worldPos.x, worldPos.z);
        var mapPosition = transformationMatrix.MultiplyPoint3x4(pos);
        Debug.Log($"World Pos: {worldPos}, Map Pos: {mapPosition}");
        return transformationMatrix.MultiplyPoint3x4(pos);
    }

    // 計算地圖的轉換矩陣，用來將世界座標轉換為地圖座標
    private void CalculateTransformationMatrix()
    {
        var minimapSize = contentRectTransform.rect.size;
        var worldSize = new Vector2(this.worldSize.x, this.worldSize.y);


        var translation = -minimapSize / 2;
        var scaleRatio = minimapSize / worldSize;

        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);
    }
}
