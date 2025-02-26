using UnityEngine;

public class PlayerTransparencyController : MonoBehaviour
{
    public Camera mainCamera;               // Assign in Inspector or defaults to Camera.main
    public float transparencyDistance = 1.5f; // Distance threshold¡Xif the camera is closer than this, the player becomes fully transparent.
    public float fadeSpeed = 5.0f;            // How quickly the alpha transitions

    private Renderer[] renderers;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        // If the camera is closer than the threshold, target alpha is 0 (completely transparent), otherwise 1 (fully opaque)
        float targetAlpha = distance < transparencyDistance ? 0f : 1f;

        // Update all materials on the player
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                // Check for a common color property. For URP Lit, it might be _BaseColor.
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
}
