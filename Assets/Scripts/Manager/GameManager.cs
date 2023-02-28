using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager gM;
    private int MaxKills;
    public int maxKills;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (gM == null)
        {
            gM = this;
        }
        else
        {
            Destroy(gameObject); //makes sure only 1 sound manager is present inthe scene
        }
    }

  
}
