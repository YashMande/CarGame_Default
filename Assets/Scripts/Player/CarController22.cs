using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Cinemachine;


public class CarController22 : MonoBehaviourPunCallbacks, IDamageble
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
    public float maxHealth;

    public MainCanvas mC;

    bool isShooting;
    LookAtMouse hitRaycast;
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed;

    public Slider healthbarSlider;
    public TextMeshProUGUI healthtext;
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private GameObject OGCanvas;
    public Slider OSCanvasSLider;
    public TextMeshProUGUI nickName;
    [SerializeField]
    private PlayerManager2 pM;
    public GameObject abilities;
    public bool doOnce;
    public GameObject turret;
    [SerializeField]
    GameObject shield;
    public Material[] materials;

    public float ability1Timer, ability2Timer;

    public Image Overlay1, Overlay2, InvisiIMG, jumpIMG;
    public bool canUse1, canUse2;
    public int coolDown1, coolDown2;
    bool onceOnly1, onceOnly2;
    bool startOverlay1, startOverlay2;
    public TMP_Text info;
    bool allowMove;
    // Start is called before the first frame update

    private void Awake()
    {
        mC = GameObject.FindObjectOfType<MainCanvas>();
        canUse1 = true;
        canUse2 = true;
    }
    void Start()
    {
        myPhotonView = gameObject.GetComponent<PhotonView>();
        
        pM = gameObject.GetComponentInParent<PlayerManager2>();
        nickName.text = myPhotonView.Owner.NickName;
        if (!myPhotonView.IsMine)
        {
            cam.enabled = false;
            healthBar.SetActive(false);
            abilities.SetActive(false);
            OGCanvas.SetActive(true);
            return;
        }
        else
        {
            gameObject.GetComponent<Outline>().enabled = false;
            OGCanvas.SetActive(false);
            //gameObject.layer = 13;
        }
        sphereRB.transform.parent = null;
        transform.position = sphereRB.transform.position;
        currentHealth = maxHealth;
        hitRaycast = gameObject.GetComponent<LookAtMouse>();
        fRate = fireRate;
        gameObject.GetComponent<MeshRenderer>().material = materials[0];
        Overlay1.fillAmount = 0;
        Overlay2.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(myPhotonView.IsMine)
        {
            //GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            speedInput = 0;
            transform.position = sphereRB.transform.position;
            if(mC.gameStarted)
            {
                turnInput = Input.GetAxis("Horizontal");
                Movement();
                Shooting();
                UpdateHealthBar();
                if(canUse1)
                {
                    Invisible();
                }
                if (canUse1 == false && onceOnly1 == false)
                {
                    onceOnly1 = true;
                    Overlay1.fillAmount = 1;
                    InvisiIMG.fillAmount = 1;
                    StartCoroutine(WaitTimerAB1());
                }
                if (startOverlay1)
                {
                    Overlay1Timer();
                }

            }
         
        }
             
    }

    void Invisible()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            myPhotonView.RPC("RPC_Invisible", RpcTarget.Others,false,1);
            canUse1 = false;
            gameObject.GetComponent<MeshRenderer>().material = materials[1];

        }
    }
    //[PunRPC]
    //void RPC_Invisible(bool invisible,int mat)
    //{
      
    //    gameObject.GetComponent<BoxCollider>().enabled = invisible;
    //    gameObject.GetComponent<MeshRenderer>().material = materials[mat];
    //}
    IEnumerator WaitTimerAB1()
    {
        yield return new WaitForSeconds(ability1Timer);
        myPhotonView.RPC("RPC_Invisible", RpcTarget.Others,true ,0);
        gameObject.GetComponent<MeshRenderer>().material = materials[0];
        startOverlay1 = true;
        StartCoroutine(CoolDownTimer1());
    }
    void Overlay1Timer()
    {
        Overlay1.fillAmount = Overlay1.fillAmount - 0.142f * Time.deltaTime;
    }

    IEnumerator CoolDownTimer1()
    {
        yield return new WaitForSeconds(coolDown1);
        canUse1 = true;
        onceOnly1 = false;
        startOverlay1 = false;
    }
    public void UpdateHealthBar()
    {
        healthbarSlider.value = currentHealth / 100;
        healthtext.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        OSCanvasSLider.value = currentHealth / 100;

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
               GameObject obj = PhotonNetwork.Instantiate("FireBullet", firePoint.position, turret.transform.rotation);
               // Instantiate(bullet, firePoint.position, turret.transform.rotation);
                //obj.GetComponent<Bullet>().notHitOBJ = this.gameObject;
               //obj.GetComponent<Rigidbody>().velocity = (hitRaycast.hitInfo.point - firePoint.position).normalized * bulletSpeed;
                obj.GetComponent<SphereCollider>().enabled = true;
                obj.GetComponent<Bullet>().notHitOBJ = this.gameObject;
           }
             
            
        }
    }
    public void TakeDamage(float damage)
    {
  
   
        myPhotonView.RPC("RPC_TakeDamage2", myPhotonView.Owner, damage);
    }
    [PunRPC]
    void RPC_TakeDamage55(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;
        myPhotonView.RPC("RPC_UpdateHealthBar", RpcTarget.All, currentHealth);
        FindObjectOfType<SoundManager>().Play("Hit");
        if (currentHealth <= 0)
        {

            if (doOnce == false)
            {
                FindObjectOfType<SoundManager>().Play("Blast");
                doOnce = true;
                mC.ResPText();
                mC.gameObject.GetComponent<PhotonView>().RPC("PlayerKilled", RpcTarget.All, info.Sender.NickName, info.photonView.Owner.NickName);
                pM.Die();
                if (PlayerManager.Find(info.Sender) != null)
                {
                    PlayerManager.Find(info.Sender).GetKill(info);
                }
                if (PlayerManager2.Find(info.Sender) != null)
                {
                    PlayerManager2.Find(info.Sender).GetKill(info);
                }
                if (PlayerManager3.Find(info.Sender) != null)
                {
                    PlayerManager3.Find(info.Sender).GetKill(info);
                }
                gameObject.SetActive(false);
            }
        }
     
    }
    [PunRPC]
    public void RPC_UpdateHealthBar(float currentHealths)
    {
        OSCanvasSLider.value = currentHealths / 100;
        if (OSCanvasSLider.value <= 0)
        {
            pM.RespawnS();
            gameObject.SetActive(false);
        }

    }

    public void FwdAccel(float speed)
    {
        myPhotonView.RPC("RPC_ForwardAccel", myPhotonView.Owner, speed);

    }
    [PunRPC]
    void RPC_ForwardAccel(float speed)
    {
        fwdAccel = speed;
        StartCoroutine(AllowMoving());
    }
    IEnumerator AllowMoving()
    {
        if (allowMove == false)
        {
            info.gameObject.SetActive(true);
            allowMove = true;

            info.text = "Engine Down, Can Move in 3";
            yield return new WaitForSeconds(1f);
            info.text = "Engine Down, Can Move in 2";
            yield return new WaitForSeconds(1f);
            info.text = "Engine Down, Can Move in 1";
            yield return new WaitForSeconds(1f);
            info.gameObject.SetActive(false);
            //myPhotonView.RPC("RPC_ForwardAccel", myPhotonView.Owner, pvtFwdAccel);
            allowMove = false;
            StopCoroutine(AllowMoving());
        }


    }
}
