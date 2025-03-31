using Photon.Pun;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject[] weaponSpawnPoints;
    public GameObject autoWeaponPickup;
    public GameObject semiWeaponPickup;

    
    void Start()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        int i = 0;
        while(i < weaponSpawnPoints.Length) 
        {
            if (i % 5 == 0)
            {
                PhotonNetwork.Instantiate(semiWeaponPickup.name, weaponSpawnPoints[i].transform.position, Quaternion.identity);
                i++;
            }
            PhotonNetwork.Instantiate(autoWeaponPickup.name, weaponSpawnPoints[i].transform.position, Quaternion.identity);
            weaponSpawnPoints[i].gameObject.SetActive(false);
            i++;
            
        }    
    }
}
