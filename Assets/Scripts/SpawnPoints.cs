using Unity.VisualScripting;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public Transform[] spawnPoints;

    public static SpawnPoints instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach(Transform point in spawnPoints) 
        {
            point.gameObject.SetActive(false);
        }

    }

    public Transform GetSpawnPoint() 
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

}
