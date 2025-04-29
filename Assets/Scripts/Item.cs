using Photon.Pun;
using Unity.Properties;
using UnityEngine;

public class Item : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public string prefabName;
    public int healAmount;
    public float cooldown;
    public AudioClip useSound;
    public int _itemAmount = 10;
    public bool singleUse;

    [PunRPC]
    public void ItemDestroy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null && view.IsMine)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) 
    {
        object[] instData = info.photonView.InstantiationData;
        int holsterViewID = (int)instData[0];
        int itemAmount = (int)instData[1];


        PhotonView holsterView = PhotonView.Find(holsterViewID);
        if (holsterView != null)
        {
            transform.parent = holsterView.transform;
        }
            _itemAmount = itemAmount;
        

    }

    public void DecreaseItemAmount(int amount) 
    {
        _itemAmount -= amount;
    }
}
