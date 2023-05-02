using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class MainCanvas : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI timerText;
    float defaultTimer = 10 * 60f;
    float currentTime;
    PhotonView myPhotonView;
    public Transform KillFeedArea;
    public TextMeshProUGUI playerConencted;
    public int playerCount;
    public bool gameStarted;
    bool allPlayerConnected;
    bool once;

    public TextMeshProUGUI respawnText;
    public bool rsText;
    public GameObject gameOverScreen;
    GameManager gM;
    bool gameEnd;
    public TextMeshProUGUI playerName;
    

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<SoundManager>().Stop("BG");
        gameStarted = false;
        allPlayerConnected = false;
        gameOverScreen.SetActive(false);
        ResetTimer();
        myPhotonView = gameObject.GetPhotonView();
        gM = FindObjectOfType<GameManager>();
        respawnText.gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(gameStarted && allPlayerConnected)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (PhotonNetwork.IsMasterClient)
                {
                    myPhotonView.RPC("UpdateTimerDisplay", RpcTarget.All, currentTime);
                }

            }
        }
        else
        {
            
        }
        if(playerCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            allPlayerConnected = true;
            
        }
        else
        {
            playerConencted.text = ("Players Connected: ")+ playerCount.ToString() + ("/") +PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }
        if(allPlayerConnected)
        {
            if(!once)
            {
                
                StartCoroutine(StartMatchTimer());
                once = true;
            }
          
        }

        if(gM.gameEnded && !gameEnd)
        {
            
            gameOverScreen.SetActive(true);
            playerName.text = gM.playerWon;
            gameEnd = true;
        }

        
    }
    public void ResPText()
    {
        StartCoroutine(RespawnTextShow());
    }
    public IEnumerator RespawnTextShow()
    {
        respawnText.gameObject.SetActive(true);
        respawnText.text = "Respawning In 3";
        yield return new WaitForSeconds(1f);
        respawnText.text = "Respawning In 2";
        yield return new WaitForSeconds(1f);
        respawnText.text = "Respawning In 1";
        yield return new WaitForSeconds(1f);
        respawnText.gameObject.SetActive(false);
    }

    IEnumerator StartMatchTimer()
    {
        playerConencted.text = ("All Players Connected");
        yield return new WaitForSeconds(1f);
        playerConencted.text = ("Match Starting in 5");
        FindObjectOfType<SoundManager>().Play("Beep");
        yield return new WaitForSeconds(1f);
        playerConencted.text = ("Match Starting in 4");
        FindObjectOfType<SoundManager>().Play("Beep");
        yield return new WaitForSeconds(1f);
        playerConencted.text = ("Match Starting in 3");
        FindObjectOfType<SoundManager>().Play("Beep");
        yield return new WaitForSeconds(1f);
        playerConencted.text = ("Match Starting in 2");
        FindObjectOfType<SoundManager>().Play("Beep");
        yield return new WaitForSeconds(1f);
        playerConencted.text = ("Match Starting in 1");
        FindObjectOfType<SoundManager>().Play("Beep");
        yield return new WaitForSeconds(1f);
        FindObjectOfType<SoundManager>().Play("Beep2");
        playerConencted.text = ("Get Em");
        yield return new WaitForSeconds(1f);
        playerConencted.gameObject.SetActive(false);
        gameStarted = true;
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<SoundManager>().Stop("Beep2");


    }
    [PunRPC]
    void PlayersConnected()
    {

    }
    [PunRPC]
    void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        string currentTimer = string.Format("{00:00}:{1:00}", minutes, seconds);
        timerText.text = currentTimer;
    }
    void ResetTimer()
    {
        currentTime = defaultTimer;
      
    }
    [PunRPC]
    public void PlayerKilled(string killer, string killed)
    {
        GameObject prefab = PhotonNetwork.Instantiate("KillFeedPrefab", KillFeedArea.position, KillFeedArea.rotation);
        prefab.transform.SetParent(KillFeedArea);
        prefab.GetComponent<PhotonView>().RPC("UpdateNames", RpcTarget.All,killer,killed);
    }

    public void OnClick_HomeButton()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
