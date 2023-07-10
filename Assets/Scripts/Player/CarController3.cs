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
public class CarController3 : MonoBehaviourPunCallbacks, IDamageble
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
    public float portalTimer;
    private float PortalTImer;
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
    private PlayerManager3 pM;

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
    public GameObject chargedShot;
    public GameObject chargedShotPrefab;
    PostProcessVolume PPV;
    Vignette vignette;
    public GameObject Vortex;
    public bool canDamage;
    public LineRenderer laserLine;

    public GameObject AreaDamager;
    public TMP_Text info;
    bool allowMove;
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
        laserLine.gameObject.SetActive(false);
        PPV = FindObjectOfType<PostProcessVolume>();
        PPV.profile.TryGetSettings(out vignette);
        currentHealth = maxHealth;
        pM = gameObject.GetComponentInParent<PlayerManager3>();
        myPhotonView = gameObject.GetComponent<PhotonView>();
        nickName.text = photonView.Owner.NickName;
        //PortalTImer = portalTimer;
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
            GetComponent<BoxCollider>().enabled = false;

            speedInput = 0f;
            transform.position = sphereRB.transform.position;
            if (mC.gameStarted == true)
            {
                laserLine.SetPosition(0, fPoint.transform.position);
                turnInput = Input.GetAxis("Horizontal");
                Movement();
                Shooting();
                UpdateHealthBar();
                if (canUse1 && Overlay2.fillAmount !=1)
                {
                    BulkUp();
                }
                if (canUse2 && Overlay1.fillAmount !=1)
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


            }

        }

    }
    void Overlay1Timer()
    {
        Overlay1.fillAmount = Overlay1.fillAmount - 0.083f * Time.deltaTime;
    }
    void Overlay2Timer()
    {
        Overlay2.fillAmount = Overlay2.fillAmount - 0.062f * Time.deltaTime;
    }
    IEnumerator WaitTimerAB1()
    {
        yield return new WaitForSeconds(ability1Timer);
        myPhotonView.RPC("VortexAblity", RpcTarget.All, false);
        canShoot = true;
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
        canUse2 = true;
        onceOnly2 = false;
        startOverlay1 = false;
        canShoot = true;
        fwdAccel = pvtFwdAccel;
        bckAccel = pvtFwdAccel;
        engineSound.Play();
        myPhotonView.RPC("VortexAblity", RpcTarget.All, false);
        myPhotonView.RPC("AreaDamageAbility", RpcTarget.All, false);
    }
    IEnumerator WaitTimerAB2()
    {

        yield return new WaitForSeconds(ability2Timer);
        myPhotonView.RPC("AreaDamageAbility", RpcTarget.All, false);
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
    public void BulkUp()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ability1Used++;

            canUse1 = false;
            canShoot = false;
            //FindObjectOfType<SoundManager>().Play("BulkUp");
            laserLine.gameObject.SetActive(false);
            barrelEffect.SetActive(false);
            shootSound.Stop();
            hitEffect.SetActive(false);
            laserLine.gameObject.SetActive(false);
            myPhotonView.RPC("VortexAblity", RpcTarget.All, true);


        }
    }
    [PunRPC]
    void VortexAblity(bool vortex)
    {
        canDamage = vortex;
        Vortex.SetActive(vortex);
    }
    public void EnergyShot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ability2Used++;
            canUse2 = false;
         
            myPhotonView.RPC("AreaDamageAbility", RpcTarget.All, true);


            //Invoke("ShootChagedShot", 2);
            // Instantiate(chargedShot,barrelEffect.transform.position,)
        }
    }
    [PunRPC]
    void AreaDamageAbility(bool ablity)
    {
       
        AreaDamager.SetActive(ablity);
        AreaDamager.GetComponentInChildren<BoxCollider>().enabled = ablity;
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
                hitEffect.SetActive(false);
                laserLine.gameObject.SetActive(false);
                myPhotonView.RPC("RPC_ShowLaser", RpcTarget.All, false, fPoint.transform.position, hitRaycast.hitInfo.point);
                shootSound.Stop();
            }

            if (isShooting)
            {
                laserLine.gameObject.SetActive(true);
                hitEffect.SetActive(true);
                barrelEffect.SetActive(true);
                hitEffect.transform.position = hitRaycast.hitInfo.point;
                if (hitRaycast.hitInfo.collider.gameObject != this.gameObject)
                {
                    if (hitRaycast.hitInfo.collider.gameObject != null)
                    {
                        fireRate = fireRate - 1 * Time.deltaTime;
                        if (fireRate <= 0)
                        {
                            fireRate = fRate;
                            myPhotonView.RPC("RPC_ShowLaser", RpcTarget.All,true, fPoint.transform.position, hitRaycast.hitInfo.point);
                            laserLine.SetPosition(0, fPoint.transform.position);
                            laserLine.SetPosition(1, hitRaycast.hitInfo.point);

                            if (hitRaycast.hitInfo.collider.gameObject.tag == "Player")
                            {
                                GameObject hitObj = hitRaycast.hitInfo.collider.gameObject;
                                if (hitObj.GetComponent<CarController3>() != null)
                                {
                                    if (hitObj.GetComponent<CarController3>().canDamage == true)
                                    {
                                        if (currentHealth >= 20)
                                        {
                                            gameObject.GetComponent<IDamageble>()?.TakeDamage(damage);
                                        }

                                    }
                                    else
                                    {
                                        hitRaycast.hitInfo.collider.gameObject.GetComponent<IDamageble>()?.TakeDamage(damage);
                                    }


                                }
                                else
                                {
                                    hitRaycast.hitInfo.collider.gameObject.GetComponent<IDamageble>()?.TakeDamage(damage);
                                }

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
                shootSound.Stop();
                hitEffect.SetActive(false);
                laserLine.gameObject.SetActive(false);
            }
        }


    }
    [PunRPC]
    void RPC_ShowLaser(bool bo,Vector3 FP,Vector3 HR)
    {
        laserLine.gameObject.SetActive(bo);
        laserLine.SetPosition(0, FP);
        laserLine.SetPosition(1, HR);
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
                myPhotonView.RPC("RPC_CannotTakeDamage3", RpcTarget.All, false);

            }

            if (mC.gameStarted)
            {
                if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit))
                {

                    if (hit.collider.gameObject.tag == "Stand")
                    {
                        myPhotonView.RPC("RPC_CannotTakeDamage3", RpcTarget.All, true);
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
            //Debug.Log("jj");
        }
    }


    public void TakeDamage(float damage)
    {
        if(stand == false)
        {
            if (currentHealth >= 1)
            {

                myPhotonView.RPC("RPC_TakeDamage3", myPhotonView.Owner, damage);
                FindObjectOfType<SoundManager>().Play("Hit");
                //Debug.Log("callonce");
            }
        }
  
    }

    [PunRPC]
    void RPC_TakeDamage3(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;
        vignette.intensity.Override(Random.Range(0.25f, 0.40f));
        Invoke("ResetVignette", 0.3f);
        myPhotonView.RPC("RPC_UpdateHealthBar", RpcTarget.All, currentHealth);
        //Debug.Log("takingdamage");
        FindObjectOfType<SoundManager>().Play("Hit");
        if (currentHealth <= 0)
        {

            if (doOnce == false)
            {
                shootSound.Stop();
                engineSound.Stop();
                barrelEffect.SetActive(false);
                isShooting = false;
                myPhotonView.RPC("RPC_ShowLaser", RpcTarget.All, false, fPoint.transform.position, hitRaycast.hitInfo.point);
                //Debug.Log("gg" + info.Sender);
                //Debug.Log("ff" + info.photonView.Owner);
                FindObjectOfType<SoundManager>().Play("Blast");
                //mC.PlayerKilled(info.Sender.NickName, info.photonView.Owner.NickName);
                doOnce = true;
                myPhotonView.RPC("AreaDamageAbility", RpcTarget.All, false);
                mC.ResPText();
                mC.gameObject.GetComponent<PhotonView>().RPC("PlayerKilled", RpcTarget.All, info.Sender.NickName, info.photonView.Owner.NickName);
                pM.Die();
                //StopAllCoroutines();
                //Debug.Log("healthlow");
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
        StartCoroutine(AllowMoving());

    }
    [PunRPC]
    void RPC_ForwardAccel(float speed)
    {
        fwdAccel = speed;
        bckAccel = speed;
        
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
    void RPC_CannotTakeDamage3(bool stands)
    {
        stand = stands;
    }
}



