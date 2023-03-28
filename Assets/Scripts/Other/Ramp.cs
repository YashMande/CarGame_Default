using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    public bool isBoostPad;
    public float jumpForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isBoostPad)
        {
            if (other.gameObject.layer == 2)
            {

                other.gameObject.GetComponent<Rigidbody>().velocity = other.gameObject.GetComponent<Rigidbody>().velocity * 7;
                FindObjectOfType<SoundManager>().Play("BoostPad");
            }
        }
        else
        {
            if (other.gameObject.layer == 2)
            {

                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * 100);
                FindObjectOfType<SoundManager>().Play("BoostPad");
            }
        }
    
    }
        
    
}
