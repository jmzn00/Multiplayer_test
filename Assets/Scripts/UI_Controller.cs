using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI_Controller : MonoBehaviour
{

    public static UI_Controller instance;
    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;
    public Slider healthSlider;
    public Slider timerSlider;

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject backButton;
    public GameObject settingsMenu;
    public GameObject settingsButtons;
    public bool isPaused = false;
    public AudioMixer audioMixer;

    public string killfeedDead;
    public string killfeedKiller;

    public TMP_Text KilledByText;

    public TMP_Text moneyValueText;
    [Header("Shop")]
    public GameObject ShopPanel;

    public GameObject leaderboard;
    public LeaderboardPlayer leaderboardPlayer;

    //public TMP_Text killsText;
    //public TMP_Text deathsText;

    public GameObject endScreen;
    


    private void Awake()
    {
        instance = this;
        CloseUI();
        KilledByText.gameObject.SetActive(false);
        ShopPanel.gameObject.SetActive(false);
    }

    public void SfxSlider(float volume) 
    {
        audioMixer.SetFloat("SfxVolume", volume);
    }

    public void MainSlide(float volume) 
    {
        audioMixer.SetFloat("MainVolume", volume);
    }

    public void OpenUI() 
    {
        pauseScreen.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        //timerSlider.gameObject.SetActive(false);
        isPaused = true;
    }
    public void CloseUI() 
    {
        pauseScreen.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
        settingsButtons.gameObject.SetActive(false);
        //timerSlider.gameObject.SetActive(true);
        isPaused = false;
    }

    public void OpenSettings() 
    {
        settingsMenu.gameObject.SetActive(true);
        settingsButtons.gameObject.SetActive(true);

    }
    public void SettingsBack() 
    {
        CloseUI();
        OpenUI();
    }

    public void ReturnToMainMenu() 
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public void ShowKilledMessage(string killtype, string killed) 
    {
        if(killtype == "Obliterated") 
        {
            KilledByText.color = Color.cyan;
        }
        else 
        {
            KilledByText.color = Color.red;
            killtype = "were killed by";
        }
            KilledByText.gameObject.SetActive(true);
        KilledByText.text = "You " + killtype + " " + killed;
        Invoke("HideKilledMessage", 3);
    }
    private void HideKilledMessage() 
    {
        KilledByText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ShopPanel.activeInHierarchy) 
            {
                ShopPanel.gameObject.SetActive(false);
            }
            else 
            {
                OpenUI();
            }
                       
        }

        if(pauseScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None) 
        {
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }

    }


    
}
