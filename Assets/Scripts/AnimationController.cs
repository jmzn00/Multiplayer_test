using Photon.Pun;
using UnityEngine;

public class AnimationController : MonoBehaviourPun
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Transform rightTargetPosition;
    [SerializeField] private Transform leftTargetPosition;
    private Vector3 weaponPosition;
    private newPlayerWeapons playerWeaponsScript;
    private PlayerItems playerItemsScript;

    [SerializeField] private Transform leftTargetTransform;

    private void Awake()
    {
        playerWeaponsScript = GetComponent<newPlayerWeapons>();
        playerItemsScript = GetComponent<PlayerItems>();
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
            rightTargetPosition.position = playerWeaponsScript.GetCurrentWeaponPosition().position + playerWeaponsScript.GetCurrentWeaponPosition().right / 4;
            rightTargetPosition.rotation = playerRb.rotation * Quaternion.Euler(0,90,0);
        }
        if (playerItemsScript.hasItem && playerItemsScript.GetCurrentItemTransform() != null) 
        {
            leftTargetPosition.position = leftTargetTransform.position;
            leftTargetPosition.rotation = playerRb.rotation * Quaternion.Euler(180,-90,0);
        }
        
    }


}
