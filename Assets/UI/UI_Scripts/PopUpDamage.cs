using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    public Vector3 IntialVelocity;
    public Rigidbody Rigidbody;
    public float lifeTime = 1f; 

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody.velocity = IntialVelocity;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
