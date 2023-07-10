using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Cinemachine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Analytics;
public class CarController2 : MonoBehaviourPunCallbacks, IDamageble
{
    [SerializeField]
    PhotonView myPhotonView;
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    [SerializeField]
    private CinemachineVirtualCamera cam;

    public float fwdAccel, bckAccel, maxSpeed, turnStrength, gravityModifier, dragOnGround, maxfirep,maxDamage,maxbulletSpeed;
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
    bool canShoot;
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
    private PlayerManager2 pM;

    public Material[] materials;
    public float damage;
    //Abilities
    public float ability1Timer, ability2Timer;

    public Image Overlay1, Overlay2, speedBoost, jumpIMG;
    public bool canUse1, canUse2;
    public int coolDown1, coolDown2;
    public bool doOnce;
    bool startOverlay1, startOverlay2;
    public GameObject abilities;
    GameObject portal;
    bool onceOnly1, onceOnly2;
    public MainCanvas mC;

    //Audio
    AudioSource engineSound;
    AudioSource shootSound;
    float minimunPitch = 0.4f;
    float maxPitach = 3.5f;
    public GameObject fPoint;
    public GameObject HealEffect;
  
    PostProcessVolume PPV;
    Vignette vignette;
    public TMP_Text info;
    bool allowMove;
    public GameObject turret;
    bool healing;
    bool stand;
    bool cannotShoot;
    public int ability1Used;
    public int ability2Used;
    private void Awake()
    {
        mC = GameObject.FindObjectOfType<MainCanvas>();
        canUse1 = true;
        canUse2 = true;

    }
    public void Start()
    {
        PPV = FindObjectOfType<PostProcessVolume>();
        PPV.profile.TryGetSettings(out vignette);
        currentHealth = maxHealth;
        pM = gameObject.GetComponentInParent<PlayerManager2>();
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
            info.gameObject.SetActive(false);

        }
        hitRaycast = gameObject.GetComponent<LookAtMouse>();
        pvtFwdAccel = fwdAccel;
        fRate = fireRate;
        sphereRB.transform.parent = null;
        Overlay1.fillAmount = 0;
        Overlay2.fillAmount = 0;
        gameObject.GetComponent<MeshRenderer>().material = materials[0];
        transform.position = sphereRB.transform.position;
        engineSound.pitch = minimunPitch;
        canShoot = true;
    

    }

    public void Update()
    {
        if (myPhotonView.IsMine)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;

            speedInput = 0f;
            transform.position = sphereRB.transform.position;
            if (mC.gameStarted == true)
            {
                turnInput = Input.GetAxis("Horizontal");
                Movement();
                Shooting();
                UpdateHealthBar();
                if (canUse1 && Overlay2.fillAmount !=1)
                {
                   
                    Invisible();
                }
                if (canUse2 &&Overlay1.fillAmount != 1)
                {
                    EnergyShot();
                }
                if (canUse1 == false && onceOnly1 == false)
                {
                    onceOnly1 = true;
                    Overlay1.fillAmount = 1;
                    speedBoost.fillAmount = 1;
                    StartCoroutine(WaitTimerAB1());
                }
                if (canUse2 == false && onceOnly2 == false)
                {
                    onceOnly2 = true;
                    Overlay2.fillAmount = 1;
                    jumpIMG.fillAmount = 1;
                    StartCoroutine(WaitTimerAB2());
                }
                if (startOverlay1)
                {
                    Overlay1Timer();
                }
                if (startOverlay2)
                {
                    Overlay2Timer();
                }
                if(currentHealth<=200)
                {
                    if (healing)
                    {
                        currentHealth = currentHealth + 10 * Time.deltaTime;
                        myPhotonView.RPC("RPC_UpdateHealthBar", RpcTarget.All, currentHealth);
                    }
                }
              

            }

        }

    }
    void Overlay1Timer()
    {
        Overlay1.fillAmount = Overlay1.fillAmount - 0.07f * Time.deltaTime;
    }
    void Overlay2Timer()
    {
        Overlay2.fillAmount = Overlay2.fillAmount - 0.055f * Time.deltaTime;
    }
    IEnumerator WaitTimerAB1()
    {
        yield return new WaitForSeconds(ability1Timer);
        damage = 20;
        fRate = 0.1f;
        myPhotonView.RPC("RPC_Invisible", RpcTarget.Others, 0);
        gameObject.GetComponent<MeshRenderer>().material = materials[0];
        startOverlay1 = true;
        StartCoroutine(CoolDownTimer());
    }
    IEnumerator CoolDownTimer()
    {
        yield return new WaitForSeconds(coolDown1);
        canUse1 = true;
        onceOnly1 = false;
        startOverlay1 = false;

    }
    public void ResetAfterDead()
    {
        canUse1 = true;
        onceOnly1 = false;
        startOverlay1 = false;
        fwdAccel = pvtFwdAccel;
        bckAccel = pvtFwdAccel;
        damage = 20;
        fRate = 0.1f;
        engineSound.Play();
        myPhotonView.RPC("RPC_Invisible", RpcTarget.Others, 0);
        gameObject.GetComponent<MeshRenderer>().material = materials[0];
    }
    IEnumerator WaitTimerAB2()
    {

        yield return new WaitForSeconds(ability2Timer);
        startOverlay2 = true;
        StartCoroutine(CoolDownTimer2());
    }
    IEnumerator CoolDownTimer2()
    {
        yield return new WaitForSeconds(coolDown2);
        canUse2 = true;
        onceOnly2 = false;
        startOverlay2 = false;


    }

    public void UpdateHealthBar()
    {
        healthbarSlider.value = currentHealth / 100;
        healthtext.text = currentHealth.ToString("00") + "/" + maxHealth.ToString();
        OSCanvasSLider.value = currentHealth / 100;

    }

    public void Movement()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * 2, 0f));
            thrustEffects[0].SetActive(false);
            thrustEffects[1].SetActive(false);
            if (engineSound.pitch >= minimunPitch)
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
            thrustEffects[0].transform.localScale = new Vector3(3, 3, 3);
            thrustEffects[1].transform.localScale = new Vector3(3, 3, 3);
            if (engineSound.pitch < maxPitach)
            {
                engineSound.pitch = engineSound.pitch + 0.5f * Time.deltaTime;
            }
            else if (engineSound.pitch > maxPitach)
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
    public void Invisible()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ability1Used++;

            canUse1 = false;
            myPhotonView.RPC("RPC_Invisible", RpcTarget.Others, 1);
            FindObjectOfType<SoundManager>().Play("BulkUp");
            gameObject.GetComponent<MeshRenderer>().material = materials[1];
            fRate = maxfirep;
            damage = maxDamage;


        }
    }
    [PunRPC]
    void RPC_Invisible(int mat)
    {

       
        gameObject.GetComponent<MeshRenderer>().material = materials[mat];
    }

    public void EnergyShot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ability2Used++;

            canUse2 = false;
            healing = true;
            HealEffect.SetActive(true);
            StartCoroutine(Healing());
            myPhotonView.RPC("RPC_Heal", RpcTarget.Others, true);

        }
    }
    [PunRPC]
    void RPC_Heal(bool heal)
    {

        HealEffect.SetActive(heal);
    }

    IEnumerator Healing()
    {
        yield return new WaitForSeconds(ability2Timer);
        healing = false;
        myPhotonView.RPC("RPC_Heal", RpcTarget.Others, false);
        HealEffect.SetActive(false);
    }
    public void Shooting()
    {
        if (canShoot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isShooting = true;
                shootSound.Play();
            }
            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
                shootSound.Stop();
            }

            if (isShooting)
            {
                barrelEffect.SetActive(true);
                if (hitRaycast.hitInfo.collider.gameObject != this.gameObject)
                {
                    if (hitRaycast.hitInfo.collider.gameObject != null)
                    {
                
               
                        fireRate = fireRate - 1 * Time.deltaTime;
                        if (fireRate <= 0)
                        {
                            if (gameObject.GetComponent<MeshRenderer>().material == materials[1])
                            {
                                fireRate = fRate;
                                GameObject obj = PhotonNetwork.Instantiate("FireBullet", fPoint.transform.position, turret.transform.rotation);
                                obj.GetComponent<SphereCollider>().enabled = true;
                                obj.GetComponent<Bullet>().notHitOBJ = this.gameObject;
                                obj.GetComponent<Bullet>().speed = maxbulletSpeed;
                                obj.GetComponent<Bullet>().damm = damage;
                            }
                            else
                            {
                                fireRate = fRate;
                                GameObject obj = PhotonNetwork.Instantiate("FireBullet", fPoint.transform.position, turret.transform.rotation);
                                obj.GetComponent<SphereCollider>().enabled = true;
                                obj.GetComponent<Bullet>().notHitOBJ = this.gameObject;
                                obj.GetComponent<Bullet>().damm = damage;
                            }
                           
                     
               
                            if (hitRaycast.hitInfo.collider.gameObject.tag == "Bot")
                            {
                                hitRaycast.hitInfo.collider.gameObject.GetComponent<IDamageble>()?.TakeDamage(damage);
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


    }


    public void FixedUpdate()
    {
        if (myPhotonView.IsMine)
        {
            isGrounded = false;
            RaycastHit hit;

            if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                isGrounded = true;
                stand = false;
                myPhotonView.RPC("RPC_CannotTakeDamage2", RpcTarget.All, false);

            }

            if (mC.gameStarted)
            {
                if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit))
                {

                    if (hit.collider.gameObject.tag == "Stand")
                    {
                        myPhotonView.RPC("RPC_CannotTakeDamage2", RpcTarget.All, true);
                        stand = true;
                        cannotShoot = false;
                        canShoot = false;
                        info.gameObject.SetActive(true);
                        info.text = "Reach to the ground to start Shooting";
                        onceOnly1 = true;
                        onceOnly2 = true;
                        canUse1 = false;
                        canUse2 = false;

                    }
                    if (stand)
                    {
                        sphereRB.AddForce(Vector3.up * -gravityModifier * 250f);
                    }
                    else
                    {
                        sphereRB.AddForce(Vector3.up * -gravityModifier * 100f);
                        if (cannotShoot == false)
                        {
                            cannotShoot = true;
                            canShoot = true;
                            info.text = "Shooting Active";
                            Invoke("StopInfoText", 1.5f);
                            onceOnly1 = false;
                            onceOnly2 = false;
                            canUse1 = true;
                            canUse2 = true;
                        }
                    }

                }


                if (Mathf.Abs(speedInput) > 0)
                {

                    sphereRB.AddForce(transform.forward * speedInput);
                }
            }


        }
    }
    void StopInfoText()
    {
        info.gameObject.SetActive(false);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey("itemIndex") && myPhotonView.IsMine && targetPlayer == myPhotonView.Owner)
        {

        }
    }


    public void TakeDamage(float damage)
    {
        if(stand == false)
        {
            if (currentHealth >= 1)
            {

                myPhotonView.RPC("RPC_TakeDamage2", myPhotonView.Owner, damage);
                FindObjectOfType<SoundManager>().Play("Hit");

            }
        }
  
    }

    [PunRPC]
    void RPC_TakeDamage2(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;
        vignette.intensity.Override(Random.Range(0.25f, 0.40f));
        Invoke("ResetVignette", 0.3f);
        myPhotonView.RPC("RPC_UpdateHealthBar", RpcTarget.All, currentHealth);
        FindObjectOfType<SoundManager>().Play("Hit");
        if (currentHealth <= 0)
        {

            if (doOnce == false)
            {

                FindObjectOfType<SoundManager>().Play("Blast");
                shootSound.Stop();
                engineSound.Stop();
                barrelEffect.SetActive(false);
                isShooting = false;
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
    void ResetVignette()
    {
        vignette.intensity.Override(0.15f);
    }

    public void FwdAccel(float speed)
    {
        myPhotonView.RPC("RPC_ForwardAccel", myPhotonView.Owner, speed);

    }
    [PunRPC]
    void RPC_ForwardAccel(float speed)
    {
        fwdAccel = speed;
        bckAccel = speed;
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
            myPhotonView.RPC("RPC_ForwardAccel", myPhotonView.Owner, pvtFwdAccel);
            allowMove = false;
            StopCoroutine(AllowMoving());
        }


    }
    [PunRPC]
    void RPC_CannotTakeDamage2(bool stands)
    {
        stand = stands;
    }

}



