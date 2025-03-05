using UnityEngine;

public class PlayerTransparencyController : MonoBehaviour
{
    public Camera mainCamera;               // Assign in Inspector or defaults to Camera.main
    public float transparencyDistance = 1f; // Distance threshold: if the camera is closer than this, player becomes fully transparent.
    public float fadeSpeed = 5.0f;            // How quickly the alpha transitions

    private Renderer[] renderers;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        // Cache renderers at start.
        UpdateRenderers();
    }

    void Update()
    {
        // Use the cached renderers.
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        float targetAlpha = distance < transparencyDistance ? 0f : 1f;

        // Process each renderer in the cached list.
        foreach (Renderer rend in renderers)
        {
            if (rend == null)
                continue;
            foreach (Material mat in rend.materials)
            {
                // Check for _BaseColor or _Color property.
                if (mat.HasProperty("_BaseColor"))
                {
                    Color col = mat.GetColor("_BaseColor");
                    col.a = Mathf.Lerp(col.a, targetAlpha, Time.deltaTime * fadeSpeed);
                    mat.SetColor("_BaseColor", col);
                }
                else if (mat.HasProperty("_Color"))
                {
                    Color col = mat.GetColor("_Color");
                    col.a = Mathf.Lerp(col.a, targetAlpha, Time.deltaTime * fadeSpeed);
                    mat.SetColor("_Color", col);
                }
            }
        }
    }

    // Call this function to update the cached renderer list (e.g., after a weapon switch).
    public void UpdateRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
}
