using UnityEngine;
using TMPro;

public class LeaderboardPlayer : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerKills;
    public TMP_Text playerDeaths;
    public TMP_Text playerMoney;

    public void SetDetails(string name, int kills, int deaths, int money) 
    {
        playerName.text = name;
        playerKills.text = kills.ToString();
        playerDeaths.text = deaths.ToString();
        playerMoney.text = money.ToString() + "$";
    }
}
