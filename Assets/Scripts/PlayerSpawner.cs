using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject playerPrefab;
    private GameObject player;

    public GameObject deathEffect;

    void Start()
    {
        if (PhotonNetwork.IsConnected) 
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer() 
    {
        Transform spawnPoint = SpawnPoints.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    public void Die() 
    {

        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);

        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);

        if(player != null) 
        {
            StartCoroutine(DieCo());
        }
        
    }

    public IEnumerator DieCo() 
    {
        PhotonNetwork.Destroy(player);
        player = null;

        yield return new WaitForSeconds(2f);

        if(MatchManager.instance.state == MatchManager.GameState.Playing && player == null) 
        {
            SpawnPlayer();
        }


    }

}
