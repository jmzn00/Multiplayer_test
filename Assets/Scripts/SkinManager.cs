using Photon.Pun;
using UnityEngine;

public class SkinManager : MonoBehaviourPun
{
    public Material[] allSkins;
    [SerializeField] private SkinnedMeshRenderer playerModel;

    private void Start()
    {
        playerModel.material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }
}
