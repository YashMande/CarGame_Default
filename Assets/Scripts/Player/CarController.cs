using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Cinemachine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class CarController : MonoBehaviourPunCallbacks , IDamageble
{
    [SerializeField]
    PhotonView myPhotonView;
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    [SerializeField]
    private CinemachineVirtualCamera cam;

    public float fwdAccel, bckAccel, maxSpeed, turnStrength, gravityModifier, dragOnGround;
    public float speedInput, turnInput;
    public Vector3 offset;
    public float fireRate;
    private float fRate;

    public bool isGrounded;
    public LayerMask whatIsGround;
    public float groundRayLength;
    public Transform groundRayPoint;
    public GameObject hitEffect;
    public GameObject[] thrustEffects;
    public GameObject barrelEffect;

    private float pvtFwdAccel;
    public bool isBoosted;
    bool isShooting;
    LookAtMouse hitRaycast;
    public float currentHealth;
    [SerializeField]
    public float maxHealth;
    public Slider healthbarSlider;
    public TextMeshProUGUI healthtext;
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private GameObject OGCanvas;
    public Slider OSCanvasSLider;
    public TextMeshProUGUI nickName;
    [SerializeField]
    private PlayerManager pM;


    //Abilities
    public float ability1Timer,ability2Timer;
    private  float maxAbility1Timer, maxAbility2Timer;
    public Image Overlay1,Overlay2,speedBoost,jumpIMG;
    public bool canUse1, canUse2;
    public int coolDown1, coolDown2;
    public bool doOnce, jumping;
    bool startOverlay1,startOverlay2;
    public GameObject abilities;

 
    public MainCanvas mC;
    //Audio
    AudioSource engineSound;
    AudioSource shootSound;
    float minimunPitch = 0.4f;
    float maxPitach = 3.5f;
    float boostPitchMax = 5;
    private void Awake()
    {
        mC = GameObject.FindObjectOfType<MainCanvas>();
        
    }
    public void Start()
    {   

        currentHealth = maxHealth;
        pM = gameObject.GetComponentInParent<PlayerManager>();     
        myPhotonView = gameObject.GetComponent<PhotonView>();
        nickName.text = photonView.Owner.NickName;
       
        if (!myPhotonView.IsMine)
        {         
            cam.enabled = false;
            healthBar.SetActive(false);
            OGCanvas.SetActive(true);
            Overlay1.enabled = false;
            Overlay2.enabled = false;
            abilities.SetActive(false);
         
            return;
        }
        else
        {
            OGCanvas.SetActive(false);
            gameObject.GetComponent<Outline>().enabled = false;
            engineSound = FindObjectOfType<SoundManager>().sounds[0].source;
            shootSound = FindObjectOfType<SoundManager>().sounds[1].source;
            engineSound.Play();
        }        
        hitRaycast = gameObject.GetComponent<LookAtMouse>();
        pvtFwdAccel = fwdAccel;
        fRate = fireRate;
        sphereRB.transform.parent = null;
        Overlay1.fillAmount = 0;
        Overlay2.fillAmount = 0;
        maxAbility1Timer = ability1Timer;
        maxAbility2Timer = ability2Timer;
        canUse1 = true;
        canUse2 = true;
        transform.position = sphereRB.transform.position;
        engineSound.pitch = minimunPitch;
    }

    public void Update()
    {       
        if (myPhotonView.IsMine)
        {
            GetComponent<BoxCollider>().enabled = false;
      
            speedInput = 0f;
            transform.position = sphereRB.transform.position;
            if (mC.gameStarted == true)
            {
                turnInput = Input.GetAxis("Horizontal");
                Movement();
                Shooting();
                UpdateHealthBar();
                if (canUse1)
                {
                    Boosting();
                }
                if (ability1Timer <= 0)
                {
                    ability1Timer = maxAbility1Timer;
                    Overlay1.fillAmount = 1;
                    speedBoost.fillAmount = 1;
                    canUse1 = false;
                    startOverlay1 = true;
                    fwdAccel = pvtFwdAccel;
                    maxPitach = 3.5f;
                    StartCoroutine(WaitTimerAB1());
                }
                if (startOverlay1)
                {
                    Overlay1Timer();
                }

                if (ability2Timer <= 0)
                {
                    ability2Timer = maxAbility2Timer;
                    Overlay2.fillAmount = 1;
                    jumpIMG.fillAmount = 1;
                    canUse2 = false;
                    jumping = false;
                    startOverlay2 = true;
                    StartCoroutine(WaitTimerAB2());
                }
                if (startOverlay2)
                {
                    Overlay2Timer();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    jumping = false;
                }

              
            }
            
        }
            
    }
    void Overlay1Timer()
    {      
        Overlay1.fillAmount = Overlay1.fillAmount - 0.2f * Time.deltaTime;
    }
    void Overlay2Timer()
    {
        Overlay2.fillAmount = Overlay2.fillAmount - 0.2f * Time.deltaTime;
    }
   IEnumerator WaitTimerAB1()
    {       
        yield return new WaitForSeconds(coolDown1);
        canUse1 = true;
        isBoosted = false;
    }

    IEnumerator WaitTimerAB2()
    {
        yield return new WaitForSeconds(coolDown2);
        canUse2 = true;
    }
   public void Jump()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            jumping = true;
            sphereRB.AddForce(Vector3.up * 8000);
            ability2Timer = ability2Timer - 1 * Time.deltaTime;
            jumpIMG.fillAmount = jumpIMG.fillAmount - 0.2f * Time.deltaTime;
            //if (Mathf.Abs(speedInput) > 0)
            //{
            //    sphereRB.AddForce(transform.forward * speedInput / 100);
            //}
        }
     
       
    }
    public void UpdateHealthBar()
    {
        healthbarSlider.value = currentHealth / 100;
        healthtext.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        OSCanvasSLider.value = currentHealth / 100;

    }

    public void Movement()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * 2, 0f));
            thrustEffects[0].SetActive(false);
            thrustEffects[1].SetActive(false);
            if(engineSound.pitch >= minimunPitch)
            {
                engineSound.pitch = engineSound.pitch - 1 * Time.deltaTime;
            }
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * fwdAccel * 1000f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * 2, 0f));
            thrustEffects[0].SetActive(true);
            thrustEffects[1].SetActive(true);
            thrustEffects[0].transform.localScale = new Vector3(3,3,3);
            thrustEffects[1].transform.localScale = new Vector3(3,3,3);
            if(engineSound.pitch < maxPitach)
            {
                engineSound.pitch = engineSound.pitch + 0.5f * Time.deltaTime;
            }
            else if(engineSound.pitch > maxPitach)
            {
                engineSound.pitch = engineSound.pitch - 1 * Time.deltaTime;
            }

        }
        else if (Input.GetAxis("Vertical") < 0)
        {

            speedInput = Input.GetAxis("Vertical") * bckAccel * 1000f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, -turnInput * turnStrength * Time.deltaTime * 2, 0f));
            thrustEffects[0].SetActive(false);
            thrustEffects[1].SetActive(false);
            if (engineSound.pitch >= minimunPitch)
            {
                engineSound.pitch = engineSound.pitch - 1 * Time.deltaTime;
            }
        }

    }
    public void Boosting()
    {
        //Boosting        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isBoosted = true;
            maxPitach = boostPitchMax;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isBoosted = false;
            fwdAccel = pvtFwdAccel;
            maxPitach = 3.5f;

        }
        if (isBoosted)
        {
            fwdAccel = fwdAccel + 40;
            thrustEffects[0].transform.localScale = new Vector3(5, 5, 5);
            thrustEffects[1].transform.localScale = new Vector3(5, 5, 5);         
            ability1Timer = ability1Timer - 1 * Time.deltaTime;
            speedBoost.fillAmount = speedBoost.fillAmount - 0.2f * Time.deltaTime;           
        }
        if (fwdAccel > 70)
        {
            fwdAccel = 70;
        }
        if (fwdAccel < pvtFwdAccel)
        {
            fwdAccel = pvtFwdAccel;
        }    
    }

    public void Shooting()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            shootSound.Play();
        }
        if(Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            shootSound.Stop();
        }

        if(isShooting)
        {
            barrelEffect.SetActive(true);
            if (hitRaycast.hitInfo.collider.gameObject != this.gameObject)
            {
                if(hitRaycast.hitInfo.collider.gameObject!= null)
                {
                    fireRate = fireRate - 1 * Time.deltaTime;
                    if (fireRate <= 0)
                    {
                        fireRate = fRate;
                        PhotonNetwork.Instantiate("HitEffect", hitRaycast.hitInfo.point, Quaternion.identity);
                        
                        if(hitRaycast.hitInfo.collider.gameObject.tag == "Player")
                        {                           
                                hitRaycast.hitInfo.collider.gameObject.GetComponent<IDamageble>()?.TakeDamage(10);                            
                        }
                       
                       
                    }
                   
                }
              
            }       
        }
        else
        {
            barrelEffect.SetActive(false);
        }
    }


    public void FixedUpdate()
    {      
        if(myPhotonView.IsMine)
        {
            isGrounded = false;
            RaycastHit hit;

            if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                isGrounded = true;

            }
            if (mC.gameStarted)
            {
             

                //if (isGrounded || jumping)
                //{
                //    sphereRB.drag = dragOnGround;
                //    if (Mathf.Abs(speedInput) > 0)
                //    {
                //        sphereRB.AddForce(transform.forward * speedInput);
                //    }
                //}
                if (!isGrounded && !jumping)
                {
                    //sphereRB.drag = 0.1f;
                    sphereRB.AddForce(Vector3.up * -gravityModifier * 100f);

                }
                if (canUse2)
                {
                    Jump();
                }
                if (Mathf.Abs(speedInput) > 0)
                {
                    //sphereRB.drag = dragOnGround;
                    sphereRB.AddForce(transform.forward * speedInput);
                }
            }
           

        }  
           
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if(changedProps.ContainsKey("itemIndex") && myPhotonView.IsMine && targetPlayer == myPhotonView.Owner)
        {
            //Debug.Log("jj");
        }
    }


    public void TakeDamage(float damage)
    {
        if(currentHealth>=1)
        {
            myPhotonView.RPC("RPC_TakeDamage", myPhotonView.Owner, damage);
            FindObjectOfType<SoundManager>().Play("Hit");
            //Debug.Log("callonce");
        }
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {      
        currentHealth -= damage;
        myPhotonView.RPC("RPC_UpdateHealthBar", RpcTarget.All, currentHealth);
        //Debug.Log("takingdamage");
        FindObjectOfType<SoundManager>().Play("Hit");
        if (currentHealth <=0)
        {
         
            if(doOnce == false)
            {
                //Debug.Log("gg" + info.Sender);
                //Debug.Log("ff" + info.photonView.Owner);
                FindObjectOfType<SoundManager>().Play("Blast");
                //mC.PlayerKilled(info.Sender.NickName, info.photonView.Owner.NickName);
                doOnce = true;
                mC.ResPText();
                mC.gameObject.GetComponent<PhotonView>().RPC("PlayerKilled", RpcTarget.All, info.Sender.NickName, info.photonView.Owner.NickName);
                pM.Die();
                //Debug.Log("healthlow");
                if(PlayerManager.Find(info.Sender)!=null)
                {
                    PlayerManager.Find(info.Sender).GetKill(info);
                }
                if (PlayerManager2.Find(info.Sender) != null)
                {
                    PlayerManager2.Find(info.Sender).GetKill(info);
                }

                //PlayerManager2.Find(info.Sender).GetKill(info);
                //currentHealth = 0;         
                
                gameObject.SetActive(false);
            }     
        }
    }

    [PunRPC]
    public void RPC_UpdateHealthBar(float currentHealths)
    {
        OSCanvasSLider.value = currentHealths / 100;
        if(OSCanvasSLider.value <=0)
        {
            pM.RespawnS();
            gameObject.SetActive(false);
        }
       
    }

 


}



