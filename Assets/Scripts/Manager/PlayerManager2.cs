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
        
    }
    public static PlayerManager2 Find(Player player)
    {
        return FindObjectsOfType<PlayerManager2>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
