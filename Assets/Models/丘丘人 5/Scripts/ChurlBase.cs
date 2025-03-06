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


}

