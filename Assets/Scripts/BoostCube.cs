using UnityEngine;

public class BoostCube : MonoBehaviour
{
    public float forceAmount;
    private void OnTriggerStay(Collider other)
    {
        Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
        playerRb.AddForce(Vector3.up * forceAmount, ForceMode.Force);
    }
}
