using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Collections;


public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;

    private List<LeaderboardPlayer> leaderboardPlayers = new List<LeaderboardPlayer>();

    private void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte 
    {
        NewPlayer,
        ListPlayers,
        ChangeStat,
        NextMatch
    }

    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    public int killsToWin = 3;
    public Transform mapCamPoint;
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;

    public bool perpetual;


    void Start()
    {
        if (!PhotonNetwork.IsConnected) 
        {
            SceneManager.LoadScene(0); //load main menu
        }
        else 
        {
            NewPlayerSend(PhotonNetwork.NickName);

            state = GameState.Playing;
        }
    }

    
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {

            ShowLeaderboard();

        }
        else if (state != GameState.Ending)
        {
            if (UI_Controller.instance.leaderboard.activeInHierarchy)
            {
                UI_Controller.instance.leaderboard.SetActive(false);
            }
        }
    }
    void ShowLeaderboard() 
    {
        UI_Controller.instance.leaderboard.SetActive(true);

        foreach(LeaderboardPlayer lp in leaderboardPlayers) 
        {
            Destroy(lp.gameObject);
        }
        leaderboardPlayers.Clear();

        UI_Controller.instance.leaderboardPlayer.gameObject.SetActive(false);

        foreach(PlayerInfo player in allPlayers) 
        {
            LeaderboardPlayer newPlayerDisplay = Instantiate(UI_Controller.instance.leaderboardPlayer, UI_Controller.instance.leaderboardPlayer.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths, player.money);

            newPlayerDisplay.gameObject.SetActive(true);

            leaderboardPlayers.Add(newPlayerDisplay);
        }
    }

    public void OnEvent(EventData photonEvent) 
    {
        if(photonEvent.Code < 200) 
        {

            
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            //Debug.Log("Recieved event" + theEvent);

            switch (theEvent) 
            {
                case EventCodes.NewPlayer:
                    NewPlayerRecieve(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayersRecieve(data);
                    break;
                case EventCodes.ChangeStat:
                    UpdateStatsRecieve(data);
                    break;
                case EventCodes.NextMatch:
                    NextMatchRecieve();
                    break;

            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string username) 
    {
        object[] package = new object[5];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;
        package[4] = 0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }
    public void NewPlayerRecieve(object[] dataRecieved) 
    {
        PlayerInfo player = new PlayerInfo((String)dataRecieved[0], (int)dataRecieved[1],
            (int)dataRecieved[2], (int)dataRecieved[3], (int)dataRecieved[4]);

        allPlayers.Add(player);

        ListPlayersSend();
    }
    public void ListPlayersSend() 
    {
        object[] package = new object[allPlayers.Count + 1];

        package[0] = state;

        for(int i = 0; i < allPlayers.Count; i++) 
        {
            object[] piece = new object[5];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;
            piece[4] = allPlayers[i].money;

            package[i + 1] = piece;
        }
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );

    }
    public void ListPlayersRecieve(object[] dataRecieved)
    {
        allPlayers.Clear();

        state = (GameState)dataRecieved[0];

        for(int i = 1; i < dataRecieved.Length; i++) 
        {
            object[] piece = (object[])dataRecieved[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3],
                (int)piece[4]
                );

            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor) 
            {
                index = i - 1;
            }
        }
        StateCheck();
    }
    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange) 
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
       (byte)EventCodes.ChangeStat,
       package,
       new RaiseEventOptions { Receivers = ReceiverGroup.All },
       new SendOptions { Reliability = true }
       );

    }
    public void UpdateStatsRecieve(object[] dataRecieved)
    {
        int actor = (int)dataRecieved[0];
        int statType = (int)dataRecieved[1];
        int amount = (int)dataRecieved[2];

        for(int i = 0; i < allPlayers.Count; i++) 
        {
            if (allPlayers[i].actor == actor) 
            {
                //Debug.Log("statType: " + statType);
                switch (statType) 
                {
                    case 0: //kills
                        allPlayers[i].kills += amount;
                        //Debug.Log("Player " + allPlayers[i].name + " : kills " + allPlayers[i].kills);
                        break;
                    case 1: //deaths
                        allPlayers[i].deaths += amount;
                        //Debug.Log("Player " + allPlayers[i].name + " : deaths " + allPlayers[i].deaths);
                        break;
                    case 4: // money
                        allPlayers[i].money += amount;
                        //Debug.Log("Player " + allPlayers[i].name + " : money : " + allPlayers[i].money);
                        break;
                }

                break;
            }
        }
        ScoreCheck();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);
    }

    void ScoreCheck() 
    {
        bool winnerFound = false;

        foreach(PlayerInfo player in allPlayers) 
        {
            if(player.kills >= killsToWin && killsToWin > 0) 
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound) 
        {
            if(PhotonNetwork.IsMasterClient && state != GameState.Ending) 
            {
                state = GameState.Ending;
                ListPlayersSend();
            }
        }
    }

    void StateCheck() 
    {
        if(state == GameState.Ending) 
        {
            EndGame();    
        }
    }

    void EndGame() 
    {
        state = GameState.Ending;

        if (PhotonNetwork.IsMasterClient) 
        {
            PhotonNetwork.DestroyAll();
        }

        UI_Controller.instance.endScreen.SetActive(true);
        ShowLeaderboard();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Camera.main.transform.position = mapCamPoint.position;
        Camera.main.transform.rotation = mapCamPoint.rotation;

        StartCoroutine(EndCo());
    }

    private IEnumerator EndCo() 
    {

        yield return new WaitForSeconds(waitAfterEnding);

        if (!perpetual) 
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else 
        {

            if (PhotonNetwork.IsMasterClient) 
            {
                if (!Launcher.instance.changeMapBetweenRounds) 
                {
                    NextMatchSend();
                }
                else 
                {
                    /*
                    if(Launcher.instance.availableMaps.Count > 0) 
                    {
                        int newLevel = UnityEngine.Random.Range(0, Launcher.instance.availableMaps.Count);
                        Debug.Log("NewLevel: " + newLevel);
                        PhotonNetwork.LoadLevel(Launcher.instance.availableMaps[newLevel]);
                    }
                    else 
                    {
                        Launcher.instance.availableMaps.Clear();
                        Launcher.instance.availableMaps = new List<string>(Launcher.instance.mapNames);
                        PhotonNetwork.LoadLevel(Launcher.instance.availableMaps[0]);
                    }
                    */


                    int newLevel = UnityEngine.Random.Range(0, Launcher.instance.mapNames.Count);

                    if (Launcher.instance.mapNames[newLevel] == SceneManager.GetActiveScene().name) 
                    {
                        NextMatchSend();
                    }
                    else 
                    {
                        PhotonNetwork.LoadLevel(Launcher.instance.mapNames[newLevel]);
                    }
                    
                    
                                        
                }
                
            }
        }
        
    }

    public void NextMatchSend() 
    {
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NextMatch,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void NextMatchRecieve() 
    {
        state = GameState.Playing;

        UI_Controller.instance.endScreen.SetActive(false);
        UI_Controller.instance.leaderboard.SetActive(false);

        foreach(PlayerInfo player in allPlayers) 
        {
            player.kills = 0;
            player.deaths = 0;
            player.money = 0;
        }

        PlayerSpawner.instance.SpawnPlayer();
        
    }
}

[System.Serializable]
public class PlayerInfo 
{
    public string name;
    public int actor;
    public int kills;
    public int deaths;
    public int money;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths, int _money) 
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
        money = _money;
    }
}