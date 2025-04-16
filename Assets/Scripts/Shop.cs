using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shop : MonoBehaviourPun
{
    [SerializeField] private Button item1_Button;
    [SerializeField] private Button item2_Button;
    [SerializeField] private Button item3_Button;
    [SerializeField] private Button item4_Button;

    [SerializeField] AudioSource buySound; 
    [SerializeField] AudioSource errorSound; 

    public static Shop instance;

    //[SerializeField] private PlayerControllerQuake playerController;
    void Start()
    {
        instance = this;
        Debug.Log("Shop Start: " + gameObject.name);

    }

    public void FirstItem()
    {

        var player = PlayerControllerQuake.localPlayer;
        int price = 400;

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                Debug.Log("Bought First Item");
                MoneyManager.instance.money -= price;
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();
            }
            else
            {
                Debug.Log("Not enough money");
                errorSound.Play();
            }
        }
    }

    public void SecondItem() 
    {

        var player = PlayerControllerQuake.localPlayer;
        int price = 600;

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                Debug.Log("Bought First Item");
                MoneyManager.instance.money -= price;
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();
            }
            else
            {
                Debug.Log("Not enough money");
                errorSound.Play();
            }
        }
    }
}
