using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WeaponManager : MonoBehaviourPun
{
    public static WeaponManager LocalPlayerInstance;

    public List<GameObject> playerWeaponList = new List<GameObject>();
    newPlayerWeapons playerWeapons;


    private void Awake()
    {
        if (photonView.IsMine) 
        {
            LocalPlayerInstance = this;
            playerWeapons = GetComponent<newPlayerWeapons>();
        }
    }

    public void AddWeaponToList(GameObject weaponPrefab) 
    {
        playerWeaponList.Add(weaponPrefab);

        if (playerWeapons != null)
        {
            playerWeapons.AddWeapon(weaponPrefab);
        }
        else
        {
            Debug.LogWarning("No playerWeapons component found!");
        }
    }

    public List<GameObject> GetWeapons()
    {
        return playerWeaponList;
    }

}
