using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OutOfBounds : MonoBehaviourPun
{
    GameObject player;
    private void OnCollisionEnter(Collision collision)
    {
        player = collision.gameObject;
        PlayerControllerQuake playerScript = player.GetComponent<PlayerControllerQuake>();
        PhotonView targetPhotonView = collision.gameObject.GetComponent<PhotonView>();
        Player targetPlayer = targetPhotonView.Owner;

        //playerScript.DealDamage(targetPlayer.NickName, 200, targetPhotonView.OwnerActorNr);                        
    }
}
