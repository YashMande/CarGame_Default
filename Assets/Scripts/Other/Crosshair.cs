using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crosshair : MonoBehaviourPun
{
    PhotonView pv;
  
 
    // Start is called before the first frame update

    private void Awake()
    {
  
    }
    void Start()
    {
        Cursor.visible = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
           
                transform.position = Input.mousePosition;
          
        }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = true;
            }
          
        
      
    
    }

    
}
