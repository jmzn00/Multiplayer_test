using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeItem : MonoBehaviourPun
{
    public LayerMask playerLayer;
    public float detectionRadius;
    public float instantiateForceAmount;
    public float applyForceAmount;
    public int damage;
    Rigidbody grenadeRb;
    [SerializeField] private ParticleSystem explodeParticleSystem;

    private void Start()
    {
        if(!photonView.IsMine) { return; }

        StartCoroutine(StartGrenade());
    }
    private IEnumerator StartGrenade() 
    {

        grenadeRb = GetComponent<Rigidbody>();        

        yield return new WaitForFixedUpdate();

        grenadeRb.useGravity = true;
        grenadeRb.isKinematic = false;
        grenadeRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        grenadeRb.AddForce(transform.forward * instantiateForceAmount, ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) { return; }
        CheckGroundLayer();
    }

    void CheckGroundLayer() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f)) 
        {
            grenadeRb.useGravity = false;
            grenadeRb.isKinematic = true;

            explodeParticleSystem.Play();
            DetectPlayers();
        }
    }
    private HashSet<int> damagedPlayerIds = new HashSet<int>();
    void DetectPlayers()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (Collider hit in hits)
        {

            if (hit.gameObject.CompareTag("Player")) 
            {
                PhotonView targetView = hit.gameObject.GetComponent<PhotonView>();
                if(targetView != null && !damagedPlayerIds.Contains(targetView.ViewID)) 
                {
                    Rigidbody playerRb = hit.gameObject.GetComponent<Rigidbody>();
                    Vector3 directionToPlayer = (playerRb.position - transform.position).normalized;

                    playerRb.AddForce(Vector3.up * applyForceAmount, ForceMode.Impulse);
                    playerRb.AddForce(directionToPlayer * applyForceAmount, ForceMode.Impulse);

                    targetView.RPC("DealDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);

                    damagedPlayerIds.Add(targetView.ViewID);

                }
            }
        }
        StartCoroutine(DestroyItem());
    }

    private IEnumerator DestroyItem() 
    {
        yield return new WaitForSeconds(0.5f);

        PhotonNetwork.Destroy(gameObject);
    }
}
