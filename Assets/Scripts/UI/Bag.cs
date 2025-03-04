using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public GameObject myBag;
    public bool isOpen;
   
    void Start()
    {
        myBag.SetActive(isOpen);
    }
    void Update()
    {
        OpenBag();
    } 
    public void OpenBag()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            myBag.SetActive(isOpen);
        }
    }
}
