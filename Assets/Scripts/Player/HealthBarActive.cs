using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarActive : MonoBehaviour
{
   public GameObject[] healthbar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthbar = GameObject.FindGameObjectsWithTag("OSCanvas");
        foreach (GameObject h in healthbar)
        {
            h.transform.LookAt(gameObject.transform);
        }
        
    }
}
