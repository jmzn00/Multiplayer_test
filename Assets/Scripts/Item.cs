using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string prefabName;
    public int healAmount;
    public float cooldown;
    public AudioClip useSound;

    [PunRPC]
    public void ItemDestroy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null && view.IsMine)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }
}
