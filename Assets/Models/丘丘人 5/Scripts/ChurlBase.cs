using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ChurlBase:MonoBehaviour
{
    protected Churl churl;
    protected GameObject churlObject;
    protected Animator animator;

    public void SetChurl(Churl churlInstance)
    {
        churl = churlInstance;
        if (churl != null)
        {
            churlObject = churl.gameObject;
            animator = churlObject.GetComponent<Animator>();
        }
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    
    protected void SetAnimatorLayerWeight(string layerName, float weight)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex == -1)
        {
            Debug.LogError($"SetAnimatorLayerWeight: �䤣��ʵe�h {layerName}");
            return;
        }
        animator.SetLayerWeight(layerIndex, weight);
    }

    protected IEnumerator SmoothSetAnimatorLayerWeight(string layerName, float targetWeight)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        float initialWeight = animator.GetLayerWeight(layerIndex);
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newWeight = Mathf.Lerp(initialWeight, targetWeight, elapsed / duration);
            animator.SetLayerWeight(layerIndex, newWeight);
            yield return null;
        }

        // Ensure the target value is set at the end.
        animator.SetLayerWeight(layerIndex, targetWeight);
    }


}

