using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.Analytics;
public class PlayerManager3 : MonoBehaviour
{
    public Vector3 transformPM;
    Quaternion rotationn;
    PhotonView PV;
    [SerializeField]
    public int kills;
    public int deaths;
    public CarController3 cC;
    [SerializeField]
    int characterID;
    public GameObject deathEffect;
    bool once;
    GameManager gm;
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        cC = GetComponentInChildren<CarController3>();

        characterID = PV.OwnerActorNr;
        gameObject.transform.position = SpawnManager.Instance.spawnPoints[characterID].transform.position;
        gameObject.transform.rotation = SpawnManager.Instance.spawnPoints[characterID].transform.rotation;
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
        cC.gameObject.transform.rotation = rotationn;
        cC.sphereRB.gameObject.transform.rotation = rotationn;
        yield return new WaitForSeconds(0.5f);
        cC.gameObject.SetActive(true);
        cC.currentHealth = cC.maxHealth;
        cC.doOnce = false;
        cC.RPC_UpdateHealthBar(cC.currentHealth);
        cC.canUse1 = true;
        cC.canUse2 = true;
        cC.Overlay1.fillAmount = 0;
        cC.Overlay2.fillAmount = 0;
        cC.isBoosted = false;
        cC.ResetAfterDead();
      
        // cC.portalPlaced = false;
        //Debug.Log("Dead");
    }
    public void GetKill(PhotonMessageInfo info)
    {
        //PV.RPC(");
        PV.RPC("RPC_GetKill", info.Sender);
        //Debug.Log("fff");

    }

    [PunRPC]
    void RPC_GetKill()
    {
        FindObjectOfType<SoundManager>().Play("Blast");
  
        kills = kills + 1;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }

    public static PlayerManager3 Find(Player player)
    {
        return FindObjectsOfType<PlayerManager3>().SingleOrDefault(x => x.PV.Owner == player);
    }

    private void Update()
    {
        if (kills >= gm.maxKills)
        {
            gm.photonView.RPC("RPC_GameOver", RpcTarget.All, cC.nickName.text);

        }
        if (gm.gameEnded)
        {
            cC.enabled = false;
            if(once == false)
            {
                if(PV.IsMine)
                {
                    once = true;
                    Analytics.CustomEvent("PulsarSelected", new Dictionary<string, object> { { "Pulsar", 2 } });
                    Analytics.CustomEvent("KillsWithPulsar", new Dictionary<string, object> { { "PulsarKills", kills * 2 } });
                    Analytics.CustomEvent("DeathsWithPulsar", new Dictionary<string, object> { { "PulsarDeaths", deaths * 2 } });
                    Analytics.CustomEvent("DeflectorAbility", new Dictionary<string, object> { { "Deflector", cC.ability1Used * 2 } });
                    Analytics.CustomEvent("ToxicCloudAbility", new Dictionary<string, object> { { "ToxicCloud", cC.ability2Used * 2 } });
                    Analytics.FlushEvents();







                }
            }

        }
    }


}
