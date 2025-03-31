using UnityEngine;
using Photon.Pun;
using TMPro;

public class KillFeedManager : MonoBehaviourPun
{

    public GameObject killFeedTextPrefab;
    public Transform killFeedParent;

    public void AddKillFeed(string killMessage) 
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            photonView.RPC("SyncKillFeedText", RpcTarget.All, killMessage);
        }
    }

    [PunRPC]
    public void SyncKillFeedText(string killMessage) 
    {
        GameObject killText = Instantiate(killFeedTextPrefab, killFeedParent);
        killText.GetComponent<TextMeshProUGUI>().text = "Testee";
    
    }
}
