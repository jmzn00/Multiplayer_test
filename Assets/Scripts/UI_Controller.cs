using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{

    public static UI_Controller instance;
    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;
    public Slider healthSlider;
    public Slider semiWeaponSlider;

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject backButton;
    public GameObject settingsMenu;
    public GameObject settingsButtons;
    public bool isPaused = false;

    public string killfeedDead;
    public string killfeedKiller;

    public TMP_Text KilledByText;


    private void Awake()
    {
        instance = this;
        CloseUI();
        KilledByText.gameObject.SetActive(false);
    }

    public void OpenUI() 
    {
        pauseScreen.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        isPaused = true;
    }
    public void CloseUI() 
    {
        pauseScreen.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
        settingsButtons.gameObject.SetActive(false);
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
            OpenUI();
        }
        if(pauseScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }


    
}
