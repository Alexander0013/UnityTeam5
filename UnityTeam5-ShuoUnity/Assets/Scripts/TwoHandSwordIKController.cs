using UnityEngine;

public class TwoHandSwordIKController : MonoBehaviour
{
    public Animator animator;
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    // You can control the IK weights (0 means no IK, 1 means fully controlled by IK)
    [Range(0f, 1f)] public float rightHandIKWeight = 1f;
    [Range(0f, 1f)] public float leftHandIKWeight = 1f;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set right-hand IK target
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
            if (rightHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }

            // Set left-hand IK target
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
            if (leftHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }
        }
    }
}
