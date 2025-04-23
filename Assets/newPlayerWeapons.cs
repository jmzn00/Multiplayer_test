using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Assertions.Must;
using UnityEngine.Animations.Rigging;
using TMPro;

public class newPlayerWeapons : MonoBehaviourPun
{
    private int selectedWeapon = 0;

    private List<GameObject> playerWeaponsList = new List<GameObject>();
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
            PhotonNetwork.Instantiate(playerWeaponPrefabNames[i], weapon.transform.position, weapon.transform.rotation, 0, instData);
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
    private void SwitchWeapon() 
    {
        foreach(GameObject playerWeapon in playerWeaponModelList) 
        {
            playerWeapon.gameObject.SetActive(false);
            //playerWeapon.gameObject.SetActive(false);
        }
        GameObject activeWeapon = playerWeaponModelList[selectedWeapon];
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

        }
        if (photonView.IsMine) 
        {
            UI_Controller.instance.timerSlider.maxValue = _rateOfFire;
        }
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
                    audioSource.PlayOneShot(_shootSound);
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
                    audioSource.PlayOneShot(_shootSound);
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
                    hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, _damage, PhotonNetwork.LocalPlayer.ActorNumber);
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
