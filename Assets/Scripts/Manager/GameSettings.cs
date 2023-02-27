using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{

    [SerializeField]
    private string _gameVersion = "0.0.0";

    public string GameVersion { get { return _gameVersion; } }

    //[SerializeField]
    //private string _nickName = "Blaze";
    GameObject og;

    //public string NickName
    //{
        
    //    //get
    //    //{
    //    //    //og = GameObject.Find("PlayerName");
    //    //    //return og.GetComponent<TextMeshProUGUI>().text;
    //    //    ////int value = Random.Range(0, 9999);
    //    //    ////return _nickName + value.ToString();
    //    //}
    //}
}
