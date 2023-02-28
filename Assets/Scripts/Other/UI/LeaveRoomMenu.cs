using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
    private RoomsCanvases _roomsCanvases;
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }
    public void OnClick_LeaveRoom()
    {
        FindObjectOfType<SoundManager>().Play("Click");
        PhotonNetwork.LeaveRoom(true);
        _roomsCanvases.CurrentRoomCanvas.Hide();
        _roomsCanvases.CreateOrJoinRoomCancas.GetComponent<CanvasGroup>().alpha = 1;
        _roomsCanvases.CreateOrJoinRoomCancas.GetComponent<CanvasGroup>().interactable = true;
        _roomsCanvases.CreateOrJoinRoomCancas.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

  
}
