using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
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
        //print("Connected To Server");
     
        if(!PhotonNetwork.InLobby)
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
