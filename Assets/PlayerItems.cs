using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItems : MonoBehaviourPun
{
    [SerializeField] private GameObject itemHolster;
    private List<GameObject> playerItems = new List<GameObject>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip healSound;

    private float timer = 0f;
    private void Update()
    {
        if (!photonView.IsMine) { return; }

        ItemScroll();

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (timer >= _cooldown)
            {
                UseItem();
                timer = 0f;
            }
            
        }        

        if(timer > _cooldown) 
        {
            timer = _cooldown;
        }
        timer += Time.deltaTime;

        UI_Controller.instance.itemUseSlider.value = timer;

    }

    public void AddItem(GameObject item) 
    {
        int itemHolsterID = itemHolster.GetComponent<PhotonView>().ViewID;
        object[] instData = new object[] { itemHolsterID };

        playerItems.Add(item);

        GameObject itemInstance = PhotonNetwork.Instantiate(item.name, itemHolster.transform.position, itemHolster.transform.rotation, 0, instData);
        itemInstance.gameObject.SetActive(false);
    }

    private int selectedItem = 0;
    private void ItemScroll() 
    {
        for(int i = 0; i <= 9; i++) 
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i)) 
            {
                selectedItem = i;

                if (selectedItem > playerItems.Count - 1)
                {
                    selectedItem = playerItems.Count - 1;
                }
                SwitchItem();
            }
        }
    }
    private int _healAmount;
    private float _cooldown;

    Transform currentItem;
    private void SwitchItem() 
    {
        /*
        foreach (GameObject item in playerItems) 
        {
            item.gameObject.SetActive(false);
        }
        playerItems[selectedItem].gameObject.SetActive(true);
        Debug.Log("SelectedItem: " + playerItems[selectedItem].gameObject.name);
        */

        int childCount = itemHolster.transform.childCount;

        for(int i = 0; i < childCount; i++) 
        {
            Transform item = itemHolster.transform.GetChild(i);
            item.gameObject.SetActive(false);

            if(i == selectedItem) 
            {
                item.gameObject.SetActive(true);               
            }

        }
        currentItem = itemHolster.transform.GetChild(selectedItem);

        _healAmount = currentItem.gameObject.GetComponent<Item>().healAmount;
        _cooldown = currentItem.gameObject.GetComponent<Item>().cooldown;

        UI_Controller.instance.itemUseSlider.maxValue = _cooldown;

        //Debug.Log("HealAmount: " + _healAmount);
        //Debug.Log("Cooldown: " + _cooldown);
                  
    }

    private void UseItem() 
    {
        if (currentItem.gameObject.activeInHierarchy) 
        {
            PlayerControllerQuake playerController = GetComponent<PlayerControllerQuake>();
            playerController.HealPlayer(_healAmount);
            audioSource.PlayOneShot(healSound);
        }      
    }



        


}
