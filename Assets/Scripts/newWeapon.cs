using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class newWeapon : MonoBehaviour
{
    public int damage;
    public float rateOfFire;
    public float radius;
    public float range;
    public bool isAuto;
    public string prefabName;
    public AudioClip shootSound;
    public int soundIndex;


    [PunRPC]
    public void WeaponDestroy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null && view.IsMine)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }
}
