using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerManager2 : MonoBehaviour
{
    public Vector3 transformPM;
    PhotonView PV;
    [SerializeField]
    public int kills;
    public int deaths;
    public CarController2 cC;
    [SerializeField]
    int characterID;
    //public GameObject deathEffect;

    GameManager gm;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        cC = GetComponentInChildren<CarController2>();

        characterID = PV.OwnerActorNr;
        gameObject.transform.position = SpawnManager.Instance.spawnPoints[characterID].transform.position;
        gameObject.transform.rotation = SpawnManager.Instance.spawnPoints[characterID].transform.rotation;
        transformPM = this.gameObject.transform.position;
        //Debug.Log(PV.OwnerActorNr + "ggg");
    }
    // Start is called before the first frame update
    void Start()
    {
        cC.mC.playerCount++;
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (kills >= gm.maxKills)
        {
            gm.photonView.RPC("RPC_GameOver", RpcTarget.All, cC.nickName.text);

        }
        if (gm.gameEnded)
        {
            cC.enabled = false;

        }
    }
    public void Die()
    {
        PhotonNetwork.Instantiate("DeathEffect", cC.gameObject.transform.position, Quaternion.identity);
        cC.gameObject.SetActive(false);
        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void GetKill(PhotonMessageInfo info)
    {   
        PV.RPC("RPC_GetKill", info.Sender);        
    }

    [PunRPC]
    void RPC_GetKill()
    {
        FindObjectOfType<SoundManager>().Play("Blast");
        Debug.Log("kill");
        kills = kills + 1;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void RespawnS()
    {

        PV.RPC("Respawn", RpcTarget.All);

    }

    [PunRPC]
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        cC.sphereRB.gameObject.transform.position = transformPM;
        yield return new WaitForSeconds(0.5f);
        cC.gameObject.SetActive(true);
        cC.currentHealth = cC.maxHealth;
        cC.doOnce = false;
        cC.RPC_UpdateHealthBar(cC.currentHealth);
        //cC.canUse1 = true;
        //cC.canUse2 = true;
        //cC.Overlay1.fillAmount = 0;
        //cC.Overlay2.fillAmount = 0;
        //cC.isBoosted = false;
        //cC.jumping = false;
        //Debug.Log("Dead");
    }
    public static PlayerManager2 Find(Player player)
    {
        return FindObjectsOfType<PlayerManager2>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
