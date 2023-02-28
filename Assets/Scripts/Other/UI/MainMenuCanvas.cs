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
        FindObjectOfType<SoundManager>().Play("Click");
        mainMenu.SetActive(false);
        enterName.SetActive(true);
    }
    public void OnClick_ExitButton()
    {
        FindObjectOfType<SoundManager>().Play("Click");
        Application.Quit();
    }
    public void OnClick_Confirm()
    {
        FindObjectOfType<SoundManager>().Play("Click");
        PhotonNetwork.NickName = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<TMP_InputField>().text;
        enterName.SetActive(false);
        cOrJRoom.GetComponent<CanvasGroup>().alpha = 1;
        cOrJRoom.GetComponent<CanvasGroup>().interactable = true;
        cOrJRoom.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
