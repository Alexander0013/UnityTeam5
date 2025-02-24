using UnityEngine;

public class AttackLayerBlender : MonoBehaviour
{
    public Animator animator;
    // Index of the attack layer (set this in the Inspector or via code)
    public int attackLayerIndex = 1;
    // How fast to blend out the attack layer (tweak as needed)
    public float blendSpeed = 2f;

    private bool blendingOut = false;

    // Call this when you want to start blending out (e.g., at the end of an attack)
    public void StartBlendingOut()
    {
        blendingOut = true;
    }

    void Update()
    {
        if (blendingOut)
        {
            float currentWeight = animator.GetLayerWeight(attackLayerIndex);
            // Lerp the weight toward zero over time
            float newWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * blendSpeed);
            animator.SetLayerWeight(attackLayerIndex, newWeight);

            // Stop blending when weight is almost zero.
            if (Mathf.Abs(newWeight) < 0.01f)
            {
                animator.SetLayerWeight(attackLayerIndex, 0f);
                blendingOut = false;
            }
            Debug.Log("blending end");
        }
    }
}
