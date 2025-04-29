using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Rendering;
using Photon.Realtime;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject menuButtons;

    public GameObject createRoomScren;
    public TMP_InputField roomNameInput;

    public GameObject roomScreen;
    public TMP_Text roomNameText;
    public TMP_Text playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public RoomButton theRoomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();

    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    public static bool hasSetNick;

    public string levelToPlay;
    public GameObject startButton;

    public GameObject roomTestButton;

    public GameObject settingsPanel;

    public string[] allMaps;
    public bool changeMapBetweenRounds;
    
    void Start()
    {
        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network...";

        PhotonNetwork.ConnectUsingSettings();
        settingsPanel.SetActive(false);

#if UNITY_EDITOR
        roomTestButton.SetActive(true);
#endif

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    public void OpenSettings() 
    {
        settingsPanel.gameObject.SetActive(true);
    }

    public void CloseSettings() 
    {
        settingsPanel.gameObject.SetActive(false);
    }

    void CloseMenus() 
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScren.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joining Lobby";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if (!hasSetNick)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);
        }
    }

    public void OpenRoomCreate() 
    {
        CloseMenus();
        createRoomScren.SetActive(true);
    }
    public void CreateRoom() 
    {
        if (!string.IsNullOrEmpty(roomNameInput.text)) 
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);            

            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);


        }
    }
    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        if (PhotonNetwork.IsMasterClient) 
        {
            startButton.SetActive(true);
        }
        else 
        {
            startButton.SetActive(false);
        }
    }


    private void ListAllPlayers() 
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++) 
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create a room: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    public void CloseErrorScreen() 
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomBrowser() 
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    public void CloseRoomBrowser() 
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomButton rb in allRoomButtons) 
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++) 
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList) 
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);

                allRoomButtons.Add(newButton);

            }
        }
    }

    public void JoinRoom(RoomInfo inputInfo) 
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenus();
        loadingText.text = "Joining Room...";
        loadingScreen.gameObject.SetActive(true);
    }

    public void SetNickname() 
    {
        if (!string.IsNullOrEmpty(nameInput.text)) 
        {
            PhotonNetwork.NickName = nameInput.text;

            CloseMenus();
            menuButtons.SetActive(true);

            hasSetNick = true;
        }
    }
    [SerializeField] public List<string> mapNames = new List<string>();

    public void StartGame() 
    {       
        //PhotonNetwork.LoadLevel(levelToPlay);
        int mapToLoad = Random.Range(0, mapNames.Count);
        PhotonNetwork.LoadLevel(mapNames[selectedMap]);
    }

    public TMP_Text selectedMapText;
    int selectedMap = 0;

    public void SwitchMap() 
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
      
        if(selectedMap >= mapNames.Count - 1) 
        {
            selectedMap = 0;
        }
        else 
        {
            selectedMap++;
        }
        selectedMapText.text = "Selected map: " + mapNames[selectedMap];      
    }

    public void TogglePerpetual(bool toggle) 
    {
        if(!PhotonNetwork.IsMasterClient) { return; }
        if(toggle == true) 
        {
            changeMapBetweenRounds = true;
        }
        else 
        {
            changeMapBetweenRounds = false;
        }

        Debug.Log("Perpetual: " + changeMapBetweenRounds);
    }



    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            startButton.SetActive(true);
        }
        else 
        {
            startButton.SetActive(false);
        }
    }

    public void QuickJoin() 
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        PhotonNetwork.CreateRoom("Test");
        CloseMenus();
        loadingText.text = "Creating room";
        loadingScreen.SetActive(true);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
