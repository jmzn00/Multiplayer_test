using Photon.Pun;
using UnityEngine;

public class IkSync : MonoBehaviourPun, IPunObservable
{
    private Transform ikTarget; // Your IK target object (e.g., the weapon's handle position)

    private Vector3 networkedPosition;
    private newPlayerWeapons playerWeaponScript;


    private void Start()
    {
        if (!photonView.IsMine) { return; }
        playerWeaponScript = GetComponent<newPlayerWeapons>();
    
    }
    void Update()
    {
        if (playerWeaponScript.hasActiveWeapon) 
        {
            ikTarget = playerWeaponScript.GetCurrentWeaponPosition();
        }
        if (photonView.IsMine)
        {
            // Local player: you move the ikTarget normally.
        }
        else
        {
            // Remote players: interpolate position
            ikTarget.position = Vector3.Lerp(ikTarget.position, networkedPosition, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the position to others
            stream.SendNext(ikTarget.position);
        }
        else
        {
            // Receive the position
            networkedPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
