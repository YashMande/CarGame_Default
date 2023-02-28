using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
    }

    public void OnClick_Button()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
        FindObjectOfType<SoundManager>().Play("Click");
        //PhotonNetwork.NickName = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<TMP_InputField>().text;
    }
 
}
