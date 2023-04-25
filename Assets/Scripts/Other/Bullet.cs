using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public GameObject notHitOBJ;
    PhotonView pv;
    private void Start()
    {
        pv = gameObject.GetPhotonView();
      
        Destroy(gameObject, 3f);
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 50000);
        //StartCoroutine(SphereColActivate());
    }
    private void FixedUpdate()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {


       
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<IDamageble>()?.TakeDamage(20);
            }


        pv.RPC("RPC_DestroyBullet", RpcTarget.All);




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
