using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bot : MonoBehaviourPun,IDamageble
{
    public float health;
    public GameObject turret;
    public void TakeDamage(float damage)
    {
        health -= damage;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <=0)
        {
            StartCoroutine(Respawn());
        }
        
    }

    IEnumerator Respawn()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Outline>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        turret.SetActive(false);
        yield return new WaitForSeconds(2f);
        health = 100;
   
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<Outline>().enabled = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        turret.SetActive(true);

    }
}
