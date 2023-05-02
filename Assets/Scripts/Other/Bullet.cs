using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public GameObject notHitOBJ;
    PhotonView pv;
    public float speed;
    public float damm;
    private void Start()
    {
        pv = gameObject.GetPhotonView();
        
        Destroy(gameObject, 3f);
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //StartCoroutine(SphereColActivate());
    }
    private void FixedUpdate()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {


        pv.RPC("RPC_DestroyBullet", RpcTarget.All);

        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<CarController3>())
            {
                if (other.gameObject.GetComponent<CarController3>().canDamage == true)
                {


                    notHitOBJ.GetComponent<IDamageble>()?.TakeDamage(damm);

                }
                else
                {
                    other.gameObject.GetComponent<IDamageble>()?.TakeDamage(damm);
                }
            }
            else
            {
                other.gameObject.GetComponent<IDamageble>()?.TakeDamage(damm);
            }
           
        }






    }
    [PunRPC]
    void RPC_DestroyBullet()
    {
        Destroy(gameObject);
    }

    //IEnumerator SphereColActivate()
    //{
    //    yield return new WaitForSeconds(0.1f);
    
    //}
}
