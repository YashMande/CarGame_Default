using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ramp : MonoBehaviourPun
{
    public bool isBoostPad;
    public float jumpForce;
     Animator anim;
 
    // Start is called before the first frame update
    void Start()
    {

        anim = gameObject.GetComponent<Animator>();
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

                other.gameObject.GetComponent<Rigidbody>().velocity = other.gameObject.GetComponent<Rigidbody>().velocity * 5.5f;
                FindObjectOfType<SoundManager>().Play("BoostPad");
            }
        }
        else
        {
            if (other.gameObject.layer == 2)
            {

                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * 150);
                other.gameObject.GetComponent<Rigidbody>().velocity = other.gameObject.GetComponent<Rigidbody>().velocity * 1.8f;
                anim.SetTrigger("Open");
                FindObjectOfType<SoundManager>().Play("BoostPad");
            }
        }
    
    }



}
