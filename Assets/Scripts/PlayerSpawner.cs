using UnityEngine;
using Photon.Pun;

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
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);

        Invoke("SpawnPlayer", 2f);
    }

}
