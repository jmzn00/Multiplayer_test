using Photon.Pun;
using UnityEngine;

public class NetworkWeapon : MonoBehaviour, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instData = info.photonView.InstantiationData;
        int holsterViewID = (int)instData[0];


        PhotonView holsterView = PhotonView.Find(holsterViewID);
        if (holsterView != null)
        {
            transform.parent = holsterView.transform;
        }
    }
}
