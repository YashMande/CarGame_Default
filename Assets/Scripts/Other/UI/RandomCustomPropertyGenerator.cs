using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RandomCustomPropertyGenerator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    private ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
  
    private void SetCustomNumber()
    {
        System.Random rnd = new System.Random();
        int result = rnd.Next(0, 99);

        _text.text = result.ToString();

        _myCustomProperty["RandomNumber"] = result;

        //PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperty;
        PhotonNetwork.SetPlayerCustomProperties(_myCustomProperty);

    }

    public void OnClick_Button()
    {
        SetCustomNumber();
    }
}
