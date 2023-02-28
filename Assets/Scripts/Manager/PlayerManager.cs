using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
public class PlayerManager : MonoBehaviour
{
    public Vector3 transformPM;
    PhotonView PV;
    [SerializeField]
    public int kills;
    public int deaths;
    public CarController cC;
    [SerializeField]
    int characterID;
    public GameObject deathEffect;

    GameManager gm;
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        cC = GetComponentInChildren<CarController>();
      
        characterID = PV.OwnerActorNr;
        gameObject.transform.position = SpawnManager.Instance.spawnPoints[characterID].transform.position;
        transformPM = this.gameObject.transform.position;
        //Debug.Log(PV.OwnerActorNr + "ggg");
    }
    private void Start()
    {
        cC.mC.playerCount++;
        gm = FindObjectOfType<GameManager>();
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
        //Debug.Log("Dead");
    }
    public void GetKill(PhotonMessageInfo info)
    {
        //PV.RPC(");
        PV.RPC("RPC_GetKill", info.Sender);
        Debug.Log("fff");

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

   public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

    private void Update()
    {
        if(kills >= gm.maxKills)
        {
            gm.photonView.RPC("RPC_GameOver", RpcTarget.All);
        }
    }

    
}
