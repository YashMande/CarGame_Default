using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class KillFeedObject : MonoBehaviour
{
    public TextMeshProUGUI Killer;
    public TextMeshProUGUI Killed;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void UpdateNames(string killer, string killed)
    {
        Killer.text = killer;
        Killed.text = killed;
    }
}
