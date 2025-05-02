using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class newPlayerWeapons : MonoBehaviourPun
{
    private int selectedWeapon = 0;

    public List<GameObject> playerWeaponsList = new List<GameObject>();
    private List<GameObject> playerWeaponModelList = new List<GameObject>();

    [SerializeField] private GameObject holster;

    [SerializeField] private GameObject playerHitParticle;
    [SerializeField] private GameObject hitParticle;

    [SerializeField] private GameObject pistolPrefab;

    

    private void Start()
    {
        if (photonView.IsMine) 
        {
            WeaponManager.LocalPlayerInstance.AddWeaponToList(pistolPrefab);
        }
        
        //SwitchWeapon();
    }

    private void Update()
    {
        if (photonView.IsMine) 
        {
            WeaponScroll();
            HandleShooting();
        }
        
        
    }
    private List<string> playerWeaponPrefabNames = new List<string>();
    public void AddWeapon(GameObject weapon) 
    {
        playerWeaponPrefabNames.Add(weapon.gameObject.name);
        playerWeaponsList.Add(weapon);

        //GameObject weaponInstance = Instantiate(weapon, holster.transform.position, holster.transform.rotation);
        //GameObject weaponInstance = PhotonNetwork.Instantiate(weapon.name, holster.transform.position, holster.transform.rotation);
        int holsterViewID = holster.GetComponent<PhotonView>().ViewID;
        object[] instData = new object[] { holsterViewID };

        GameObject weaponInstance = PhotonNetwork.Instantiate(weapon.name, holster.transform.position, holster.transform.rotation, 0, instData);
        //weaponInstance.transform.parent = holster.transform;

        playerWeaponModelList.Add(weaponInstance);
        weaponInstance.SetActive(false);
    }

    private void WeaponScroll() 
    {
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f && playerWeaponModelList.Count > 0) 
        {
            selectedWeapon++;

            if (selectedWeapon > playerWeaponsList.Count - 1)
            {
                selectedWeapon = 0;
            }

            SwitchWeapon();

        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f && playerWeaponModelList.Count > 0)
        {
            selectedWeapon--;

            if (selectedWeapon < 0)
            {
                selectedWeapon = 0;
            }

            SwitchWeapon();

        }
    }

    public void DropWeapons() 
    {
        if(!photonView.IsMine) { return; }
        if(holster.transform.childCount < 2) { return; }
        /*
            int i = 1;
            foreach (Transform weapon in holster.transform)
            {
                object[] instData = new object[] { 0 };
                GameObject dropped = PhotonNetwork.Instantiate(playerWeaponPrefabNames[i], weapon.transform.position, weapon.transform.rotation, 0, instData);
                i++;
            }
        */
            
        for(int i = 1; i < holster.transform.childCount; i++) 
        {
            Transform weapon = holster.transform.GetChild(i);
            object[] instData = new object[] { 0 };
            GameObject dropped = PhotonNetwork.Instantiate(playerWeaponPrefabNames[i], weapon.transform.position, weapon.transform.rotation, 0, instData);
            Rigidbody rb = dropped.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public void ClearAllLists() 
    {
        playerWeaponModelList.Clear();
        playerWeaponPrefabNames.Clear();
        playerWeaponsList.Clear();
    }

    /*
    [PunRPC]
    public void RequestWeaponDestroy(int viewId) 
    {
        PhotonView targetView = PhotonView.Find(viewId);
        PhotonNetwork.Destroy(targetView.gameObject);
    }
    */

    private int _damage;
    private float _range;
    private float _radius;
    private float _rateOfFire;
    private bool _isAuto;
    private AudioClip _shootSound;
    private int _soundIndex;
    private GameObject activeWeapon;
    public bool hasActiveWeapon = false;
    private void SwitchWeapon() 
    {
        foreach(GameObject playerWeapon in playerWeaponModelList) 
        {
            playerWeapon.gameObject.SetActive(false);
            //playerWeapon.gameObject.SetActive(false);
        }
        activeWeapon = playerWeaponModelList[selectedWeapon];
        activeWeapon.gameObject.SetActive(true);

        newWeapon weaponScript = activeWeapon.GetComponent<newWeapon>();

        if(weaponScript != null) 
        {
            _damage = weaponScript.damage;
            _range = weaponScript.range;
            _radius = weaponScript.radius;
            _rateOfFire = weaponScript.rateOfFire;
            _isAuto = weaponScript.isAuto;
            _shootSound = weaponScript.shootSound;
            _soundIndex = weaponScript.soundIndex;

        }
        if (photonView.IsMine) 
        {
            UI_Controller.instance.timerSlider.maxValue = _rateOfFire;
        }
        hasActiveWeapon = true;
    }

    public Transform GetCurrentWeaponPosition() 
    {
        return activeWeapon.transform;
    }

    [SerializeField] private AudioSource audioSource;

    private float timer = 0f;
    private void HandleShooting() 
    {    
        UI_Controller.instance.timerSlider.value = timer;

        if (_isAuto) 
        {
            if (Input.GetMouseButton(0))
            {
                if (timer >= _rateOfFire && _rateOfFire > 0f)
                {
                    SphereCast();
                    timer = 0;
                    //audioSource.PlayOneShot(_shootSound);
                    photonView.RPC("PlaySoundAtPosition", RpcTarget.All, _soundIndex, transform.position);
                }
            }
        }
        else 
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (timer >= _rateOfFire && _rateOfFire > 0f)
                {
                    SphereCast();
                    timer = 0;
                    //audioSource.PlayOneShot(_shootSound);
                    photonView.RPC("PlaySoundAtPosition", RpcTarget.All,_soundIndex, transform.position);
                }
            }
        }
        
        timer += Time.deltaTime;
    }

    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private GameObject damageTextGameobject;
    private void SphereCast() 
    {
        Vector3 textOffset = new Vector3(0.25f, 1, 0);
        RaycastHit hit;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.SphereCast(ray.origin, _radius, ray.direction, out hit, _range, ~weaponLayer)) 
        {
            if(hit.collider.tag == "Player" && hit.collider.gameObject.GetPhotonView() != null) 
            {
                if(hit.collider.gameObject.GetPhotonView() != null) 
                {
                    //Debug.Log("Hit: " + hit.collider.gameObject.GetPhotonView().ViewID);
                    //Debug.Log("Me: " + gameObject.GetPhotonView().ViewID);

                    if(hit.collider.gameObject.GetPhotonView().ViewID == gameObject.GetPhotonView().ViewID) 
                    {
                        return;                    
                    }

                        hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, _damage, PhotonNetwork.LocalPlayer.ActorNumber);
                        PhotonNetwork.Instantiate(playerHitParticle.name, hit.point, Quaternion.identity);

                        GameObject damageText = Instantiate(damageTextGameobject, hit.point + textOffset, transform.rotation);
                        TextMeshPro tmp = damageText.GetComponent<TextMeshPro>();
                        tmp.text = _damage.ToString();
                    
                    

                }                                
            }
            else 
            {
                PhotonNetwork.Instantiate(hitParticle.name, hit.point, Quaternion.identity);
            }
            
        }
    }
}
