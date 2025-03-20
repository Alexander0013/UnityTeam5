using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : ItemGiver
{
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            AddNewItem();
            Destroy(gameObject);
        }
    }
}
