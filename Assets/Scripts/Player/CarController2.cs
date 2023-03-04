using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Cinemachine;


public class CarController2 : MonoBehaviourPunCallbacks, IDamageble
{
    PhotonView myPhotonView;
    public Rigidbody sphereRB;
    [SerializeField]
    private CinemachineVirtualCamera cam;

    public float fwdAccel, bckAccel, maxSpeed, turnStrength, gravityModifier, dragOnGround;
    public float speedInput, turnInput;
    public float fireRate;
    private float fRate;

    public bool isGrounded;
    public LayerMask whatIsGround;
    public float groundRayLength;
    public Transform groundRayPoint;


    public float currentHealth;
    [SerializeField]
    private float maxHealth;

    public MainCanvas mC;

    bool isShooting;
    LookAtMouse hitRaycast;
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;
    // Start is called before the first frame update

    private void Awake()
    {
        mC = GameObject.FindObjectOfType<MainCanvas>();

    }
    void Start()
    {
        myPhotonView = gameObject.GetComponent<PhotonView>();
        if(!myPhotonView.IsMine)
        {
            cam.enabled = false;
            return;
        }
        sphereRB.transform.parent = null;
        transform.position = sphereRB.transform.position;
        currentHealth = maxHealth;
        hitRaycast = gameObject.GetComponent<LookAtMouse>();
        fRate = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(myPhotonView.IsMine)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            speedInput = 0;
            transform.position = sphereRB.transform.position;
            if(mC.gameStarted)
            {
                turnInput = Input.GetAxis("Horizontal");
                Movement();
                Shooting();
            }
         
        }
             
    }

    void Movement()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * 2, 0f));
  
        }

        if (Input.GetAxis("Vertical") > 0)
        {

            speedInput = Input.GetAxis("Vertical") * fwdAccel * 1000f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * 2, 0f));
 

        }
        else if (Input.GetAxis("Vertical") < 0)
        {

            speedInput = Input.GetAxis("Vertical") * bckAccel * 1000f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -turnInput * turnStrength * Time.deltaTime * 2, 0f));

        }

    }

    private void FixedUpdate()
    {
        if(myPhotonView.IsMine)
        {
            isGrounded = false;
            RaycastHit hit;

            if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                isGrounded = true;

            }
            if(mC.gameStarted)
            {
                if (Mathf.Abs(speedInput) > 0)
                {
                    //sphereRB.drag = dragOnGround;
                    sphereRB.AddForce(transform.forward * speedInput);
                }
                if (!isGrounded)
                {
                    //sphereRB.drag = 0.1f;
                    sphereRB.AddForce(Vector3.up * -gravityModifier * 100f);

                }
            }
      
        }
 
    }

    void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
         
        }
        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
          
        }

        if (isShooting)
        {          
           fireRate = fireRate - 1 * Time.deltaTime;
           if (fireRate <= 0)
           {
               fireRate = fRate;
               GameObject obj = PhotonNetwork.Instantiate("FireBullet", firePoint.position, Quaternion.identity);
               obj.GetComponent<Rigidbody>().velocity = (hitRaycast.hitInfo.point - firePoint.position).normalized * bulletSpeed;
           }
             
            
        }
    }
    public void TakeDamage(float damage)
    {
        Debug.Log("talomg dam");
   
        myPhotonView.RPC("RPC_TakeDamage2", myPhotonView.Owner, damage);
    }
    [PunRPC]
    void RPC_TakeDamage2(float damage)
    {
        currentHealth -= damage;
        Debug.Log("TakingDamage");
    }

}
