using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PlayerListing _playerListing;

    [SerializeField]
    private Transform _content;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomsCanvases _roomsCanvases;

    [SerializeField]
    private TextMeshProUGUI _readyUpText;
    private bool _ready = false;
    [SerializeField]
    private Button ReadyUpButton;
    [SerializeField]
    ColorBlock color;
 
    public override void OnEnable()
    {
        base.OnEnable();
        SetReadyUp(false);
        GetCurrentRoomPlayers();
        base.photonView.RPC("RPC_MasterClient", RpcTarget.All);

    }
    public override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < _listings.Count; i++)       
            Destroy(_listings[i].gameObject);
        
        _listings.Clear();
    }
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
        
    }

    private void Update()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            ReadyUpButton.interactable = false;
            if(PhotonNetwork.CurrentRoom.PlayerCount <=1)
            {
                ReadyUpButton.gameObject.SetActive(false);
            }
            else
            {
                ReadyUpButton.gameObject.SetActive(true);
            }
            for (int i = 0; i < _listings.Count; i++)
            {
                if (_listings[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if (!_listings[i].Ready)
                    {
                        _readyUpText.text = "Waiting for others";
                        return;
                    }
                    else
                    {
                        _readyUpText.text = "All Players Connected";
                      
                    }
                    
                }
            }
            
        }
    }
    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (_ready)
        {
            _readyUpText.text = "Waiting for others";
        }
        else
        {
            _readyUpText.text = "Ready Up";

            ReadyUpButton.colors = color;
         
        }
    }


    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
       

      
        foreach (KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
        
    }
    private void AddPlayerListing(Player player)
    {
        base.photonView.RPC("RPC_MasterClient", RpcTarget.All);
        int index = _listings.FindIndex(x => x.Player == player);
        if(index!= -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            //base.OnPlayerEnteredRoom(player);
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }

        
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        _roomsCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
      
        AddPlayerListing(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       // base.OnPlayerLeftRoom(otherPlayer);
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < _listings.Count; i++)
            {
                if(_listings[i].Player!= PhotonNetwork.LocalPlayer)
                {
                    if (!_listings[i].Ready)
                        return;
                }
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            FindObjectOfType<SoundManager>().Play("Click");
            PhotonNetwork.LoadLevel(1);
        }
       
    }

    public void OnClick_ReadyUp()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            FindObjectOfType<SoundManager>().Play("Click");
            SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.All,PhotonNetwork.LocalPlayer,_ready);
        }
    }
    public void OnClick_PracticeArea()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    private void RPC_ChangeReadyState(Player player,bool ready)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].Ready = ready;
            _listings[index].readyICon.SetActive(ready);
        }
    }
    [PunRPC]
    void RPC_MasterClient()
    {

        for (int i = 0; i < _listings.Count; i++)
        {
            if (_listings[i].Player == PhotonNetwork.MasterClient)
            {

                _listings[i].masterClienticon.SetActive(true);
            }
         
        }
    }


}
