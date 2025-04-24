using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CustomPrefabPool : MonoBehaviour, IPunPrefabPool
{

    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // Register prefabs at the start
    void Start()
    {
        PhotonNetwork.PrefabPool = FindFirstObjectByType<CustomPrefabPool>();


        prefabDictionary.Add("SMG", Resources.Load<GameObject>("SMG"));
        prefabDictionary.Add("Pistol", Resources.Load<GameObject>("Pistol"));
        prefabDictionary.Add("SNIPER", Resources.Load<GameObject>("SNIPER"));
        prefabDictionary.Add("Rifle", Resources.Load<GameObject>("Rifle"));
        prefabDictionary.Add("PlayerQuake", Resources.Load<GameObject>("PlayerQuake"));
        // Add other weapons as needed
    }

    // Instantiate a prefab based on its name
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.ContainsKey(prefabId))
        {
            return GameObject.Instantiate(prefabDictionary[prefabId], position, rotation);
        }
        else
        {
            Debug.LogError("Prefab not found in pool: " + prefabId);
            return null;
        }
    }

    // Destroy a prefab
    public void Destroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
