using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Animations.Rigging;
using Photon.Pun;

public class Follow_Target : MonoBehaviourPun
{
    private Transform player;
    public Rig rig;

    public float dist;
    public float inMin = 2f;
    public float inMax = 5f;
    public float outMin = 0f;
    public float outMax = 1f;

    public AnimationCurve remapLerp;

    private void Start()
    {
        if (photonView.IsMine) // Only apply this logic for the local player
        {
            FindLocalPlayer();
        }

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }

    private void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                player = p.transform;
                break;
            }
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindLocalPlayer();
            return;
        }

        dist = Vector3.Distance(player.position, transform.position);
        float value = math.remap(inMin, inMax, outMin, outMax, dist);
        float weightValue = remapLerp.Evaluate(value);

        // If the local player is controlling it, sync it across all clients
        if (photonView.IsMine)
        {
            photonView.RPC("SyncRigWeight", RpcTarget.AllBuffered, weightValue);
        }
    }

    [PunRPC]
    private void SyncRigWeight(float weight)
    {
        rig.weight = weight;
    }
}
