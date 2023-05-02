using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ChargedShot : MonoBehaviour
{
    float maxScale;
    GameObject obj;
    public bool bullet;
    // Start is called before the first frame update
    void Start()
    {
        
        if(bullet)
        {
            Shoot();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!bullet)
        {
            maxScale = 5;
            if (gameObject.transform.localScale.x <= maxScale)
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + 2 * Time.deltaTime, gameObject.transform.localScale.y + 2 * Time.deltaTime, gameObject.transform.localScale.z + 2 * Time.deltaTime);
            }
        }
        else
        {
            maxScale = 10;
            
            if (gameObject.transform.localScale.x <= maxScale)
            {
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + 2 * Time.deltaTime, gameObject.transform.localScale.y + 2 * Time.deltaTime, gameObject.transform.localScale.z + 2 * Time.deltaTime);
            }
        }
     
    }

    void Shoot()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 50000);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
                obj = other.gameObject;
                other.gameObject.GetComponent<IDamageble>()?.FwdAccel(0);
                  
        }
        PhotonNetwork.Destroy(gameObject);
    }

}
