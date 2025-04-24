using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WeaponManager : MonoBehaviourPun
{
    public static WeaponManager LocalPlayerInstance;

    public List<GameObject> playerWeaponList = new List<GameObject>();
    public List<GameObject> playerItemList = new List<GameObject>();
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
            //Debug.LogWarning("No playerWeapons component found!");
        }
    }

    public void AddItemToList(GameObject itemPrefab) 
    {
        PlayerItems playerItems = GetComponent<PlayerItems>();
        //Debug.Log("AddedItem: " + itemPrefab.name);

        if(playerItems != null) 
        {
            playerItems.AddItem(itemPrefab);
            playerItemList.Add(itemPrefab);
        }
        else 
        {
            Debug.Log("PlayerItemsNull");
        }
        
    }

    public List<GameObject> GetWeapons()
    {
        return playerWeaponList;
    }

}
