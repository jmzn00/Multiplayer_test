using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shop : MonoBehaviourPun
{
    [SerializeField] private Button item1_Button;
    [SerializeField] private Button item2_Button;
    [SerializeField] private Button item3_Button;
    [SerializeField] private Button item4_Button;
    [SerializeField] private Button item5_Button;

    [SerializeField] AudioSource buySound; 
    [SerializeField] AudioSource errorSound;

    [SerializeField] GameObject pistolWeapon;
    [SerializeField] GameObject smgWeapon;
    [SerializeField] GameObject rifleWeapon;
    [SerializeField] GameObject sniperWeapon;
    [SerializeField] GameObject crackPipe;
    [SerializeField] GameObject bong;

    public static Shop instance;

    //[SerializeField] private PlayerControllerQuake playerController;
    void Start()
    {
        instance = this;      
    }
    public void FirstItem()
    {

        var player = PlayerControllerQuake.localPlayer;
        int price = 400;

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                //Debug.Log("Bought First Item");
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();

                if(WeaponManager.LocalPlayerInstance != null) 
                {
                    WeaponManager.LocalPlayerInstance.AddWeaponToList(pistolWeapon);
                }
            }
            else
            {
                //Debug.Log("Not enough money");
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
                //Debug.Log("Bought Second Item");
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();

                if (WeaponManager.LocalPlayerInstance != null)
                {                    
                    WeaponManager.LocalPlayerInstance.AddWeaponToList(smgWeapon);
                }
            }
            else
            {
                //Debug.Log("Not enough money");
                errorSound.Play();
            }
        }
    }
    public void ThirdItem() 
    {
        var player = PlayerControllerQuake.localPlayer;
        int price = 800;

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                //Debug.Log("Bought Second Item");
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();

                if (WeaponManager.LocalPlayerInstance != null)
                {
                    WeaponManager.LocalPlayerInstance.AddWeaponToList(rifleWeapon);
                }
            }
            else
            {
                //Debug.Log("Not enough money");
                errorSound.Play();
            }
        }
    }
    public void FourthItem() 
    {
        var player = PlayerControllerQuake.localPlayer;
        int price = 1000;

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                //Debug.Log("Bought Second Item");
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
                buySound.Play();

                if (WeaponManager.LocalPlayerInstance != null)
                {
                    WeaponManager.LocalPlayerInstance.AddWeaponToList(sniperWeapon);
                }
            }
            else
            {
                //Debug.Log("Not enough money");
                errorSound.Play();
            }
        }
    }
    public void FifthItem() 
    {
        var player = PlayerControllerQuake.localPlayer;
        int price = 2000;

        Debug.Log("FifthItem");

        if(player != null) 
        {
            if(MoneyManager.instance.money >= price) 
            {
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();

                if(WeaponManager.LocalPlayerInstance != null) 
                {
                    WeaponManager.LocalPlayerInstance.AddItemToList(crackPipe);
                    buySound.Play();
                }                                
            }
            else 
            {
                errorSound.Play();
                Debug.Log("NotEnoughMoney");
            }
        }
    }
    public void SixthItem()
    {
        var player = PlayerControllerQuake.localPlayer;
        int price = 4000;

        Debug.Log("SixthItem");

        if (player != null)
        {
            if (MoneyManager.instance.money >= price)
            {
                MoneyManager.instance.money -= price;
                MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, -price);
                UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();

                if (WeaponManager.LocalPlayerInstance != null)
                {
                    WeaponManager.LocalPlayerInstance.AddItemToList(bong);
                    buySound.Play();
                }
            }
            else
            {
                errorSound.Play();
                Debug.Log("NotEnoughMoney");
            }
        }
    }
}
