using UnityEngine;
using Photon.Pun;

public class PlayerInteraction : MonoBehaviourPun
{
    [SerializeField] private PlayerControllerQuake playerController;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            PlayerInteract();
        }
    }


    private void PlayerInteract() 
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 2.5f)) 
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
        }
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
