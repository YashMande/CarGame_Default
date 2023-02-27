using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class MainMenuCanvas : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject enterName;
    public GameObject cOrJRoom;
   

    private void Awake()
    {
        enterName.SetActive(false);
        cOrJRoom.GetComponent<CanvasGroup>().alpha = 0;
        cOrJRoom.GetComponent<CanvasGroup>().interactable = false;
        cOrJRoom.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnClick_PlayButton()
    {
        mainMenu.SetActive(false);
        enterName.SetActive(true);
    }
    public void OnClick_ExitButton()
    {
        Application.Quit();
    }
    public void OnClick_Confirm()
    {
        PhotonNetwork.NickName = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<TMP_InputField>().text;
        enterName.SetActive(false);
        cOrJRoom.GetComponent<CanvasGroup>().alpha = 1;
        cOrJRoom.GetComponent<CanvasGroup>().interactable = true;
        cOrJRoom.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
