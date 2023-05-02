using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RotateCamWithCursor : MonoBehaviour
{
    public float speed = 5;
    float rotX,rotY;
    CarController cC;
    CinemachineVirtualCamera cmv;
    float cmSpeed;
    private float CMSpeed;
    public GameObject speed_particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        speed_particleSystem.SetActive(false);
        speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale = 200;
        CMSpeed = cmSpeed;
        cmv = gameObject.GetComponent<CinemachineVirtualCamera>();
        cC = gameObject.GetComponentInParent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        rotX += Input.GetAxis("Mouse X") * speed;
        rotY += Input.GetAxis("Mouse Y") * speed;
        rotY = Mathf.Clamp(rotY, -30f, 10f);
        rotX = Mathf.Clamp(rotX, -25f, 25f);
        transform.localRotation = Quaternion.Euler(-rotY, rotX, 0f);

       if(cC.speedInput >0)
        {
            if(cmv.m_Lens.FieldOfView <=110)
            {
                speed_particleSystem.SetActive(true);
                cmSpeed = cmSpeed + 1.5f * Time.deltaTime;
                cmv.m_Lens.FieldOfView = cmv.m_Lens.FieldOfView + cmSpeed * Time.deltaTime;
                if(speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale >=88)
                {
                    speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale -= 30 * Time.deltaTime;
                }
                
               
            }
            
        }
       if(cC.speedInput ==0)
        {
            if(cmv.m_Lens.FieldOfView >= 60)
            cmv.m_Lens.FieldOfView = cmv.m_Lens.FieldOfView - 6 * Time.deltaTime;
            if (speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale < 200)
            {
                speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale += 5 * Time.deltaTime;
            }
        }
       if(cC.speedInput <0)
        {
            if (cmv.m_Lens.FieldOfView >= 60)
                cmv.m_Lens.FieldOfView = cmv.m_Lens.FieldOfView - 6 * Time.deltaTime;
            if (speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale < 200)
            {
                speed_particleSystem.GetComponent<ParticleSystemRenderer>().lengthScale += 5 * Time.deltaTime;
            }
        }
        
        // transform.eulerAngles += speed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);


    }
}
