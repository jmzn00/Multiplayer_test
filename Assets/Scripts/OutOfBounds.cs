using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OutOfBounds : MonoBehaviourPun
{
    GameObject player;
    private void OnCollisionEnter(Collision collision)
    {
        PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();

        if(targetView != null) 
        {
            targetView.RPC("DealDamage", RpcTarget.All, 200, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}
