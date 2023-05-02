using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
public class TestConnect : MonoBehaviourPunCallbacks
{
    public GameObject StartGame;
    public GameObject Extra;
    // Start is called before the first frame update
    void Start()
    {
       if(Application.loadedLevel == 0)
        {
            StartGame.SetActive(false);
        }
     
        ////print("Connecting To Server");
        PhotonNetwork.SendRate = 40; //20
        PhotonNetwork.SerializationRate = 20;//10
        PhotonNetwork.AutomaticallySyncScene = true;
    
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        StartGame.SetActive(true);
        Extra.SetActive(false);
        if (!PhotonNetwork.InLobby)
        {
            
            PhotonNetwork.JoinLobby();
        }
    

        
    
}
  
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        //print("Disconnected from the Server" + cause.ToString());
    }
}
