using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LookAtMouse : MonoBehaviourPun
{
    [SerializeField]
    private Transform _turretBarrel;

    public Transform spaceShip;
    [SerializeField]
    public RaycastHit hitInfo;
   
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayOrigin, out hitInfo))
            {
                //Debug.Log(hitInfo.collider.gameObject.name);
                if (hitInfo.collider != null)
                {
                    
                    
                        Vector3 direction = hitInfo.point - _turretBarrel.position;
                        _turretBarrel.rotation = Quaternion.LookRotation(direction);
                    
                   

                }
            }
        }
        else
        {
            return;
        }
   
    }

}
