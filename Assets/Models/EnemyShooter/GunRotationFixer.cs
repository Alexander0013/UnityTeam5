using UnityEngine;

public class GunRotationFixer : MonoBehaviour
{
    // The offset in degrees you want to apply (negative rotates left)
    public float rotationOffsetY = -45f;
    
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // OnAnimatorIK is called after the Animator has computed IK.
    // It is only called if the Animator component is set to use IK.
    private void OnAnimatorIK(int layerIndex)
    {
        if(animator == null)
            return;
        
        // Ensure we are in Humanoid rig mode.
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        if(rightHand != null)
        {
            // Get the current local rotation of the right hand.
            Quaternion currentLocalRot = rightHand.localRotation;
            // Create an offset rotation (45° to the left).
            Quaternion offset = Quaternion.Euler(0, rotationOffsetY, 0);
            // Apply the offset on top of the existing local rotation.
            // Note: animator.SetBoneLocalRotation works on Unity 2019.3 and later.
            animator.SetBoneLocalRotation(HumanBodyBones.RightHand, offset * currentLocalRot);
        }
    }
}
