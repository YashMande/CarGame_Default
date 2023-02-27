using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomListing _roomListing;

    [SerializeField]
    private Transform _content;

    private List<RoomListing> _listings = new List<RoomListing>();
    private RoomsCanvases _roomsCanvases;

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _roomsCanvases.CurrentRoomCanvas.Show();
        //_content.DestroyChildren();
        _listings.Clear();
        gameObject.GetComponentInParent<CanvasGroup>().alpha = 0;
        gameObject.GetComponentInParent<CanvasGroup>().interactable = false;
        gameObject.GetComponentInParent<CanvasGroup>().blocksRaycasts = false;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (RoomInfo info in roomList)
        {
            //removed from rooms list
            if(info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            //added to rooms list
            else
            {
                int index = _listings.FindIndex(x =>x.RoomInfo.Name == info.Name);
                if(index == -1)
                {
                    RoomListing listing = Instantiate(_roomListing, _content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
              
            }
        
        }
    }
}
