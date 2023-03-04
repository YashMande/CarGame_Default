using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; 
public class CharacterSelectCanvas : MonoBehaviour
{
    public GameObject car1, car2, car3;
    public int carSelectedNumber;
    public GameObject selectionScreen;
    GameManager gm;
    public TextMeshProUGUI carName;
    // Start is called before the first frame update
    void Start()
    {
        selectionScreen.SetActive(false);

        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(carSelectedNumber == 0)
        {
            car1.SetActive(true);
            car2.SetActive(false);
            car3.SetActive(false);
            carName.text = "SpeedSter";
        }
        else if(carSelectedNumber ==1)
        {
            car2.SetActive(true);
            car1.SetActive(false);
            car3.SetActive(false);
            carName.text = "Mid";
        }
        else if(carSelectedNumber ==2)
        {
            car3.SetActive(true);
            car1.SetActive(false);
            car2.SetActive(false);
            carName.text = "Heal";
        }
    }

    public void OnClick_ChangeRider()
    {
        selectionScreen.SetActive(true);
    }

    public void NextButton()
    {
        if(carSelectedNumber<2)
        {
            carSelectedNumber++;
            FindObjectOfType<SoundManager>().Play("Click");
        }
       
    }
    public void PreviousButton()
    {
        if(carSelectedNumber>0)
        {
            carSelectedNumber--;
            FindObjectOfType<SoundManager>().Play("Click");
        }
      
    }
    public void OnClick_Select()
    {
        selectionScreen.SetActive(false);
        gm.playerSelected = carSelectedNumber;
    }
}
