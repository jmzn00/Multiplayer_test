using UnityEngine;
using Photon.Pun;

public class Money : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public int moneyValue;
    [SerializeField] private GameObject moneyGameObject;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 0)
        {
            moneyValue = (int)instantiationData[0];
        }
    }
}
