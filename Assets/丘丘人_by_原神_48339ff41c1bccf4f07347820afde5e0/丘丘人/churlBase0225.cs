using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public abstract class ChurlBase:MonoBehaviour
{
    protected Churl churl;
    protected Animator animator;
    public ChurlBase(Churl churl)
    {
        this.churl = churl;
        this.animator = churl.GetComponent<Animator>();
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    protected void SetAnimatorLayerWeight(string layerName, float weight)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Animator ¨S¦³§ä¨ì Layer¡G{layerName}");
            return;
        }
        animator.SetLayerWeight(layerIndex, weight);
    }
}

