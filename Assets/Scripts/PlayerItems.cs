using NUnit.Framework;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerItems : MonoBehaviourPun
{
    [SerializeField] private GameObject itemHolster;
    public List<GameObject> playerItems = new List<GameObject>();
    //private List<String> playerItemPrefabNames = new List<String>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip healSound;

    private int _healAmount;
    private float _cooldown;
    private AudioClip _useSound;
    private bool _singleUse;

    private float timer = 0f;
    Transform currentItem;
    public bool hasItem = false;
    
    private void Update()
    {
        if (!photonView.IsMine) { return; }

        ItemScroll();

        if (Input.GetMouseButtonDown(1)) 
        {
            if (currentItem.gameObject.GetComponent<Item>().singleUse) 
            {
                 if(currentItem.gameObject.GetComponent<Item>()._itemAmount > 0) 
                {
                    UseItem();
                }   
            }
            else 
            {
                if (timer >= _cooldown && !currentItem.gameObject.GetComponent<Item>().singleUse)
                {
                    UseItem();
                    timer = 0f;
                }
            }                        
        }        
        if(timer > _cooldown) 
        {
            timer = _cooldown;
        }

        if (Input.GetKeyDown(KeyCode.G)) 
        {
            DropItem();
        }
        timer += Time.deltaTime;

        UI_Controller.instance.itemUseSlider.value = timer;

    }

    public Transform GetCurrentItemTransform() 
    {
        return item;
    }

    public void AddItem(GameObject item, int itemAmount) 
    {
        Debug.Log("AddItem: " + item.gameObject.name);

        int itemHolsterID = itemHolster.GetComponent<PhotonView>().ViewID;
        object[] instData = new object[] { itemHolsterID , itemAmount};

        playerItems.Add(item);
        //playerItemPrefabNames.Add(item.GetComponent<Item>().prefabName);

        GameObject itemInstance = PhotonNetwork.Instantiate(item.name, itemHolster.transform.position, itemHolster.transform.rotation, 0, instData);
        itemInstance.gameObject.SetActive(false);
        hasItem = true;

        if (item.gameObject.GetComponent<Item>().singleUse) 
        {
            UI_Controller.instance.beerSlider.value = itemAmount;
            Debug.Log("AddedItemAmount: " + itemAmount);
        }
    }

    private int selectedItem = 0;
    private void ItemScroll() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            selectedItem = 0;
            SwitchItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedItem = 1;
            SwitchItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedItem = 2;
            SwitchItem();
        }
    }
    private Transform item;
    private void SwitchItem() 
    {
        int childCount = itemHolster.transform.childCount;

        for(int i = 0; i < childCount; i++) 
        {
            item = itemHolster.transform.GetChild(i);
            item.gameObject.SetActive(false);

            if(i == selectedItem) 
            {
                item.gameObject.SetActive(true);               
            }

        }
        currentItem = itemHolster.transform.GetChild(selectedItem);
        _healAmount = currentItem.gameObject.GetComponent<Item>().healAmount;
        _cooldown = currentItem.gameObject.GetComponent<Item>().cooldown;
        _useSound = currentItem.gameObject.GetComponent<Item>().useSound;
        _singleUse = currentItem.gameObject.GetComponent<Item>().singleUse;

        UI_Controller.instance.itemUseSlider.maxValue = _cooldown;           
    }

    private void UseItem() 
    {
        if (currentItem.gameObject.activeInHierarchy) 
        {
            if (currentItem.gameObject.GetComponent<Item>().singleUse) 
            {
                currentItem.gameObject.GetComponent<Item>().DecreaseItemAmount(1);
            }
            PlayerControllerQuake playerController = GetComponent<PlayerControllerQuake>();
            playerController.HealPlayer(_healAmount);
            audioSource.PlayOneShot(healSound);
            audioSource.PlayOneShot(_useSound);

            if (currentItem.gameObject.GetComponent<Item>().singleUse) 
            {
                UI_Controller.instance.beerSlider.value = currentItem.gameObject.GetComponent<Item>()._itemAmount;
            }
            
        }
        //Debug.Log("usedItemAmount:  " + currentItem.gameObject.GetComponent<Item>()._itemAmount);

        
    }

    public void DropItem() 
    {
        if (!photonView.IsMine) { return; }

        Transform item = itemHolster.transform.GetChild(selectedItem);
        Debug.Log("Dropped: " + itemHolster.transform.GetChild(selectedItem).name);

        object[] instData = new object[] { 0, item.GetComponent<Item>()._itemAmount };
        GameObject dropped = PhotonNetwork.Instantiate(playerItems[selectedItem].gameObject.GetComponent<Item>().prefabName, itemHolster.transform.position, itemHolster.transform.rotation, 0, instData);
        Rigidbody rb = dropped.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        PhotonView itemView = item.GetComponent<PhotonView>();


        if (item.gameObject.GetComponent<Item>().singleUse)
        {
            UI_Controller.instance.beerSlider.value = 0;
        }

        Debug.Log("Removed: " + playerItems[selectedItem].gameObject.name);
        playerItems.Remove(playerItems[selectedItem]);



        if(itemView.IsMine) 
        {
            PhotonNetwork.Destroy(item.gameObject);
        }
        else 
        {
            photonView.RPC("ItemDestroy", item.gameObject.GetPhotonView().Owner, itemView.ViewID);
        }
    }
    public void DropItems() 
    {
        if (!photonView.IsMine) { return; }

        for (int i = 0; i < itemHolster.transform.childCount; i++)
        {
            Transform item = itemHolster.transform.GetChild(i);

            object[] instData = new object[] { 0 , item.GetComponent<Item>()._itemAmount};
            GameObject dropped = PhotonNetwork.Instantiate(playerItems[i].gameObject.GetComponent<Item>().prefabName, itemHolster.transform.position, itemHolster.transform.rotation, 0, instData);
            Rigidbody rb = dropped.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
                         
        }
        hasItem = false;
    }



        


}
