using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoarditemPrefab;
    [SerializeField] CanvasGroup canvasGroup;
    public int playerCount;

    Dictionary<Player, ScoreboardItem> scoreBoardItem = new Dictionary<Player, ScoreboardItem>();
    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreBoardItem(player);
        }
    }
    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddScoreBoardItem(newPlayer);
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemoveScoreBoardItem(otherPlayer);
    }

    void AddScoreBoardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreBoarditemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        scoreBoardItem[player] = item;
        //Debug.Log("new player" + player.ActorNumber);
    }

    void RemoveScoreBoardItem(Player player)
    {
        Destroy(scoreBoardItem[player].gameObject);
        scoreBoardItem.Remove(player);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGroup.alpha = 1;
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            canvasGroup.alpha = 0;
        }
    
    }
}
