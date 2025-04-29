using Photon.Pun;
using UnityEngine;

public class IkSync : MonoBehaviourPun, IPunObservable
{
    private Transform ikTargetRight; // Your IK target object (e.g., the weapon's handle position)
    private Transform ikTargetLeft;

     

    private Vector3 networkedPosition;
    private newPlayerWeapons playerWeaponScript;
    private PlayerItems playerItemScript;


    private void Start()
    {
        if (!photonView.IsMine) { return; }
        playerWeaponScript = GetComponent<newPlayerWeapons>();
        playerItemScript = GetComponent<PlayerItems>();
    
    }
    void Update()
    {
        if(photonView == null) { return; }

        if(ikTargetRight != null) 
        {
            if (playerWeaponScript.hasActiveWeapon)
            {
                ikTargetRight = playerWeaponScript.GetCurrentWeaponPosition();
            }
            else
            {
                ikTargetRight.position = Vector3.Lerp(ikTargetRight.position, networkedPosition, Time.deltaTime * 10f);
            }
        }
        if(ikTargetLeft != null) 
        {
            if (playerItemScript.hasItem) 
            {
                ikTargetLeft = playerItemScript.GetCurrentItemTransform();
            }
            else 
            {
                ikTargetLeft.position = Vector3.Lerp(ikTargetLeft.position, networkedPosition, Time.deltaTime * 10f);
            }
        }

        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the position to others
            stream.SendNext(ikTargetRight.position);
        }
        else
        {
            // Receive the position
            networkedPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
