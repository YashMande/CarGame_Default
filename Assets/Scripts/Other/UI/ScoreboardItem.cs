using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text userNameText;
    public TMP_Text killsText;
    public TMP_Text deathText;
    public TMP_Text pingText;

    Player player;
    public void Initialize(Player player)
    {
        userNameText.text = player.NickName;
        this.player = player;
        UpdateStat();
    }

    private void LateUpdate()
    {
        pingText.text = PhotonNetwork.GetPing().ToString();
    }
    void UpdateStat()
    {
        if(player.CustomProperties.TryGetValue("kills", out object kills))
        {
            killsText.text = kills.ToString();
          
        }

        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            deathText.text = deaths.ToString();
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
       
        if(targetPlayer == player)
        {
            if(changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
            {
                UpdateStat();
            }
        }
    }

}
