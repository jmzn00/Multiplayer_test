using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class VoidGrenade : MonoBehaviourPun
{
    public float detectionRadius;
    public float instantiateForceAmount;
    public float applyForceAmount;
    public LayerMask playerLayer;
    Rigidbody voidGrenadeRb;


    [SerializeField] private ParticleSystem explodeParticleSystem;

    private void Start()
    {
        if (!photonView.IsMine) { return; }
        StartCoroutine(StartVoidGrenade());

    }

    private IEnumerator StartVoidGrenade() 
    {
        voidGrenadeRb = GetComponent<Rigidbody>();

        yield return new WaitForFixedUpdate();

        voidGrenadeRb.useGravity = true;
        voidGrenadeRb.isKinematic = false;
        voidGrenadeRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        voidGrenadeRb.AddForce(transform.forward * instantiateForceAmount, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) { return; }
        CheckGroundLayer();
    }

    private void CheckGroundLayer() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f)) 
        {
            voidGrenadeRb.useGravity = false;
            voidGrenadeRb.isKinematic = true;

            explodeParticleSystem.Play();
            DetectPlayers();
        }
    }

    private void DetectPlayers() 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach(Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Player")) 
            {
                Rigidbody playerRb = hit.gameObject.GetComponent<Rigidbody>();
                Vector3 directionToGrenade = (transform.position - playerRb.position).normalized;

                playerRb.AddForce(directionToGrenade * applyForceAmount, ForceMode.Force);
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
