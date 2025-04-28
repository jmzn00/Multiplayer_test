using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviourPun
{
    [SerializeField] private GameObject itemHolster;
    private List<GameObject> playerItems = new List<GameObject>();
    private List<String> playerItemPrefabNames = new List<String>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip healSound;

    private int _healAmount;
    private float _cooldown;
    private AudioClip _useSound;

    private float timer = 0f;
    Transform currentItem;

    private void Update()
    {
        if (!photonView.IsMine) { return; }

        ItemScroll();

        if (Input.GetMouseButtonDown(1)) 
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
        playerItemPrefabNames.Add(item.GetComponent<Item>().prefabName);

        GameObject itemInstance = PhotonNetwork.Instantiate(item.name, itemHolster.transform.position, itemHolster.transform.rotation, 0, instData);
        itemInstance.gameObject.SetActive(false);
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
    private void SwitchItem() 
    {
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
        _useSound = currentItem.gameObject.GetComponent<Item>().useSound;

        UI_Controller.instance.itemUseSlider.maxValue = _cooldown;           
    }

    private void UseItem() 
    {
        if (currentItem.gameObject.activeInHierarchy) 
        {
            PlayerControllerQuake playerController = GetComponent<PlayerControllerQuake>();
            playerController.HealPlayer(_healAmount);
            audioSource.PlayOneShot(healSound);
            audioSource.PlayOneShot(_useSound);
        }      
    }

    public void DropItems() 
    {
        if (!photonView.IsMine) { return; }

        for (int i = 0; i < itemHolster.transform.childCount; i++)
        {
            Transform item = itemHolster.transform.GetChild(i);
            object[] instData = new object[] { 0 };
            GameObject dropped = PhotonNetwork.Instantiate(playerItemPrefabNames[i], itemHolster.transform.position, itemHolster.transform.rotation, 0 , instData);
            Rigidbody rb = dropped.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }



        


}
