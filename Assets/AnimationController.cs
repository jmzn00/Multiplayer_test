using Photon.Pun;
using UnityEngine;

public class AnimationController : MonoBehaviourPun
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Transform targetPosition;
    private Vector3 weaponPosition;
    private newPlayerWeapons playerWeaponsScript;

    private void Awake()
    {
        playerWeaponsScript = GetComponent<newPlayerWeapons>();
    }

    private void Update()
    {
        if(playerRb.linearVelocity.magnitude > 1) 
        {
            playerAnim.SetBool("Running", true);
        }
        else 
        {
            playerAnim.SetBool("Running", false);
        }

        if (playerWeaponsScript.hasActiveWeapon) 
        {
            targetPosition.position = playerWeaponsScript.GetCurrentWeaponPosition().position + playerWeaponsScript.GetCurrentWeaponPosition().right / 4;
            targetPosition.rotation = playerRb.rotation * Quaternion.Euler(0,90,0);
            
        }
    }


}
