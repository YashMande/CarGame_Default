using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 2)
        {
           
            collision.gameObject.GetComponent<Rigidbody>().velocity = collision.gameObject.GetComponent<Rigidbody>().velocity + collision.gameObject.GetComponent<Rigidbody>().velocity + collision.gameObject.GetComponent<Rigidbody>().velocity;
            FindObjectOfType<SoundManager>().Play("BoostPad");
        }
    }
}
