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
    public LayerMask[] ignoreme;
    
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, ~ignoreme[0]))
            {
               
                    Vector3 direction = hitInfo.point - _turretBarrel.position;
                    _turretBarrel.rotation = Quaternion.LookRotation(direction);
                    //Debug.Log(hitInfo.collider.gameObject.name);
                    //Debug.DrawLine(_turretBarrel.position, hitInfo.point);
                    if (hitInfo.collider != null)
                    {






                    }
                
            }
        }
      
   
    }

}
