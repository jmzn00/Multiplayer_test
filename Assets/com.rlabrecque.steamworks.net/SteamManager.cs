using UnityEngine;
using Steamworks;

public class CustomSteamManager : MonoBehaviour
{
    private void Start()
    {
        try
        {
            if (!SteamAPI.Init())
            {
                Debug.LogError("SteamAPI_Init failed. Steam is not running or App ID is wrong.");
            }
            else
            {
                Debug.Log("SteamAPI initialized successfully!");
                Debug.Log("Logged in as: " + SteamFriends.GetPersonaName());
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load Steam DLL. " + e.Message);
        }
    }

    private void Update()
    {
        SteamAPI.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }
}
