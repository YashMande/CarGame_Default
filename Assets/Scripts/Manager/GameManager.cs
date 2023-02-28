using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviourPun
{
    static GameManager gM;
    private int MaxKills;
    public int maxKills;
    PhotonView pV;
    public bool gameEnded;
    public string playerWon;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (gM == null)
        {
            gM = this;
        }
        else
        {
            Destroy(gameObject); //makes sure only 1 sound manager is present inthe scene
        }
        pV = gameObject.GetPhotonView();
    }
    [PunRPC]
    void RPC_GameOver(string name)
    {
        gameEnded = true;
        playerWon = name;
        FindObjectOfType<SoundManager>().Stop("ShootSound");
        //Debug.Log("GameEnded");
    }

}
