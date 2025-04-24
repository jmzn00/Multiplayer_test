using UnityEngine;
using Photon.Pun;
using NUnit.Framework;

public class PlayerInteraction : MonoBehaviourPun
{
    [SerializeField] private PlayerControllerQuake playerController;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (!photonView.IsMine) { return; }

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            PlayerInteract();
        }
        //AutoPickup();
    }

    public LayerMask groundLayer;
    private void PlayerInteract() 
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 5f)) 
        {
            if(hit.collider.tag == "Money")
            {
                if (photonView.IsMine) 
                {
                    Money moneyPickup = hit.collider.GetComponent<Money>();

                    if (moneyPickup != null)
                    {
                        playerController.AddMoneyLocally(moneyPickup.moneyValue);

                        PhotonView moneyView = moneyPickup.GetComponent<PhotonView>();

                        if (PhotonNetwork.IsMasterClient) 
                        {

                            PhotonNetwork.Destroy(moneyPickup.gameObject);
                        }
                        else 
                        {
                            photonView.RPC("RequestMoneyDestroy", RpcTarget.MasterClient, moneyView.ViewID);
                        }
                        
                    }
                }
            }
            if(hit.collider.tag == "NPC") 
            {
                UI_Controller.instance.ShopPanel.SetActive(true);
            }           
            if (hit.collider.tag == "Weapon")
            {
                string prefabName = hit.collider.GetComponent<newWeapon>().prefabName;
                PhotonView view = hit.collider.GetComponent<PhotonView>();
                newPlayerWeapons nPw = GetComponent<newPlayerWeapons>();

                bool weaponExists = false;

                foreach(GameObject weapon in nPw.playerWeaponsList) 
                {
                    newWeapon pW = weapon.GetComponent<newWeapon>();

                    Debug.Log("PrefabName: " + pW.prefabName);

                    if(pW.prefabName == prefabName) 
                    {
                        weaponExists = true;
                    }
                }

                if (!weaponExists) 
                {
                    GameObject weaponPrefab = Resources.Load<GameObject>(prefabName);
                    WeaponManager.LocalPlayerInstance.AddWeaponToList(weaponPrefab);

                    if (view.IsMine)
                    {
                        PhotonNetwork.Destroy(hit.collider.gameObject);
                    }
                    else
                    {
                        view.RPC("WeaponDestroy", view.Owner, view.ViewID);
                    }
                }                
            }  
            if(hit.collider.tag == "Item") 
            {
                string prefabName = hit.collider.GetComponent<Item>().prefabName;

                GameObject itemPrefab = Resources.Load<GameObject>(prefabName);
                WeaponManager.LocalPlayerInstance.AddItemToList(itemPrefab);

                PhotonView view = hit.collider.GetComponent<PhotonView>();

                if (view.IsMine)
                {
                    PhotonNetwork.Destroy(hit.collider.gameObject);
                }
                else
                {
                    view.RPC("WeaponDestroy", view.Owner, view.ViewID);
                }
            }
        }
    }
    private void AutoPickup()
    {
        Vector3 autoPickupSpot = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);

        if (Physics.SphereCast(autoPickupSpot, 0.25f, Vector3.down, out RaycastHit autoPickupHit, 0.75f, ~groundLayer))
        {
            if (autoPickupHit.collider.tag == "Money")
            {
                if (photonView.IsMine)
                {
                    Money moneyPickup = autoPickupHit.collider.GetComponent<Money>();

                    if (moneyPickup != null)
                    {
                        playerController.AddMoneyLocally(moneyPickup.moneyValue);

                        PhotonView moneyView = moneyPickup.GetComponent<PhotonView>();

                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(moneyPickup.gameObject);
                        }
                        else
                        {
                            photonView.RPC("RequestMoneyDestroy", RpcTarget.MasterClient, moneyView.ViewID);
                        }

                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 autoPickupSpot = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(autoPickupSpot, 0.5f);
    }

    [PunRPC]
    public void RequestMoneyDestroy(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if(targetView != null) 
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
        
    }
}
