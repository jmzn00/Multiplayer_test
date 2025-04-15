using Photon.Pun;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviourPun
{
    [SerializeField] private PlayerControllerQuake playerController;
    [SerializeField] private GameObject playerHitImpact;

    [SerializeField] private GameObject[] playerWeapons;

    private int damage = 0;
    private float fireRate = 0;
    private float radius = 0;
    private float range = 0;
    private ParticleSystem shootParticle;
    private ParticleSystem hitParticle;

    void Start()
    {
        if (!photonView.IsMine) { return; }

        SwitchWeapons();
    }

    
    void Update()
    {
        if (!photonView.IsMine) { return; 
        }
        HandleShooting();
        Weapons();
        //Debug.Log(selectedGun);
        //Debug.Log(playerCamera.transform.forward);
    }
    private float timer = 0f;
    private void HandleShooting()
    {

        timer += Time.deltaTime;
        UI_Controller.instance.timerSlider.maxValue = fireRate;
        UI_Controller.instance.timerSlider.value = timer;

        if (Input.GetMouseButton(0))
        {
            if(timer >= fireRate) 
            {
                SphereCast();                
                timer = 0;
            }
        }

    }
    private void SphereCast()
    {
        RaycastHit hit;
        Vector3 shootOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.SphereCast(ray.origin, radius, ray.direction, out hit, range))
        {
            //Debug.Log("Ray direction: " + ray.direction);
            //Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Player" && hit.collider.gameObject.GetPhotonView() != null)
            {
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, damage, PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.LookRotation(ray.direction));
            }
            else 
            {
                //AddWallImpact
            }
        }
        PhotonNetwork.Instantiate(shootParticle.name, transform.position, Quaternion.LookRotation(ray.direction));
        PhotonNetwork.Instantiate(hitParticle.name, hit.point, Quaternion.identity);
    }
    int selectedGun = 0;
    private void Weapons() 
    {

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f) 
        {
            selectedGun++;

            if(selectedGun > playerWeapons.Length - 1) 
            {
                selectedGun = 0;
            }
            SwitchWeapons();
        }
    }

    private void SwitchWeapons() 
    {
        foreach(GameObject playerWeapon in playerWeapons) 
        {
            playerWeapon.gameObject.SetActive(false);
        }

        GameObject selectedWeapon = playerWeapons[selectedGun];
        selectedWeapon.SetActive(true);

        //FIX
        //photonView.RPC("UpdateGunDisplay", RpcTarget.Others, selectedGun);

        Weapon weaponScript = selectedWeapon.GetComponent<Weapon>();

        if(weaponScript != null) 
        {
            damage = weaponScript.damage;
            fireRate = weaponScript.fireRate;
            radius = weaponScript.radius;
            range = weaponScript.range;
            shootParticle = weaponScript.shootParticle;
            hitParticle = weaponScript.hitParticle;
            //Debug.Log(damage);
        }
        else 
        {
            Debug.Log("WeaponScriptNull");
        }
        
    }

    [PunRPC]
    void UpdateGunDisplay(int newGun)
    {
        selectedGun = newGun;
        SwitchWeapons();
    }

}
