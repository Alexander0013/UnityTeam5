using UnityEngine;

public class GunAligner : MonoBehaviour
{
    // Offset rotation for the gun relative to the hand bone.
    public Vector3 localRotationOffset;
    
    private Transform parentTransform;
    
    void Start()
    {
        parentTransform = transform.parent;
    }
    
    void LateUpdate()
    {
        if (parentTransform != null)
        {
            // Ensure the gun's local rotation is always set to the offset.
            transform.localRotation = Quaternion.Euler(localRotationOffset);
        }
    }
}
