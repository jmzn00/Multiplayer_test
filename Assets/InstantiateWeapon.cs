using Photon.Pun;
using UnityEngine;

public class InstantiateWeapon : MonoBehaviourPun
{
    [SerializeField] private GameObject instantiateItem;
    private Camera playerCamera;
    public float cooldown;
    private float timer = 0f;

    private void Start()
    {
        if (!photonView.IsMine) { return; }

        playerCamera = Camera.main;        
    }

    private void Update()
    {
        if (!photonView.IsMine) { return; }
        if(!gameObject.activeInHierarchy) { return; }

        var player = transform.root.GetComponent<PlayerControllerQuake>();
        if (player == null || player.GetHealth() <= 0) { return; }


        if (Input.GetMouseButtonDown(0) && timer >= cooldown) 
        {
            Vector3 spawnPosition = playerCamera.transform.position + playerCamera.transform.forward * 2;
            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.transform.forward);

            object[] instData = new object[] { 0, 0 };
            PhotonNetwork.Instantiate(instantiateItem.name, spawnPosition, spawnRotation, 0, instData);
            timer = 0f;
        }

        timer += Time.deltaTime;
    }
}
