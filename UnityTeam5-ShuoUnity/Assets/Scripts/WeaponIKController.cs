using UnityEngine;

public class WeaponIKController : MonoBehaviour
{
    public Animator animator;
    // The target transform that represents the ideal grip position on the weapon.
    public Transform rightHandTarget;
    // Weight for position and rotation IK
    public float ikWeight = 1f;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set the IK position and rotation for the right hand
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

            if (rightHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                //Debug.Log("apply IK");
            }
        }
    }
}
