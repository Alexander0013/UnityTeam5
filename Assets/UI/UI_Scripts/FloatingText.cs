using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 2f;
    public Vector3 offset;
    public Vector3 randomizeIntensity = new Vector3(0.5f, 0, 0);

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
        Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
        Random.Range(-randomizeIntensity.z, randomizeIntensity.z));
    }

    private void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform.position);  // 朝向攝影機
            transform.Rotate(0f, 180f, 0f); // 避免文字反向（有時候 LookAt 會導致文字反轉）
        }
    }
}
