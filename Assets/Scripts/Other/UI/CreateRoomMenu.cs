using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _roomName;
    private RoomsCanvases _roomCanvases;
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomCanvases = canvases;
    }
    public void OnClick_CreateRoom()
    {
        FindObjectOfType<SoundManager>().Play("Click");
        if(!PhotonNetwork.IsConnected)
        {
            return;
        }
        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
       
    }
 
    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        Debug.Log("Created room successs", this);
        _roomCanvases.CurrentRoomCanvas.Show();
        gameObject.GetComponentInParent<CanvasGroup>().alpha = 0;
        gameObject.GetComponentInParent<CanvasGroup>().interactable = false;
        gameObject.GetComponentInParent<CanvasGroup>().blocksRaycasts = false;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Room Creation failed:"+ message, this);
    }

}
