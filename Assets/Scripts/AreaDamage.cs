using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public float damage;
    GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.GetComponentInParent<CarController3>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject != parent)
        {
            if (other.gameObject.tag == "Player")
            {

                other.gameObject.GetComponent<IDamageble>()?.TakeDamage(damage);
            }
        }
        else
        {
            return;
        }
   
  
   
    }
}
