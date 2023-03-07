using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject notHitOBJ;
    private void Start()
    {
        Destroy(gameObject, 3f);
    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject != notHitOBJ)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<IDamageble>()?.TakeDamage(20);
            }
           Destroy(gameObject);
        }
       
        
        
     
       
    }
}
