using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crosshair : MonoBehaviourPun
{
    PhotonView pv;

    MainCanvas mC;
    // Start is called before the first frame update

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start()
    {
        mC = FindObjectOfType<MainCanvas>();
       
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
          if(mC.gameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
      
    
    }

    
}
