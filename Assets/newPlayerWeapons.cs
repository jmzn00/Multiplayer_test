using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class newPlayerWeapons : MonoBehaviourPun
{
    private int selectedWeapon = 0;

    private List<GameObject> playerWeaponsList = new List<GameObject>();
    private List<GameObject> playerWeaponModelList = new List<GameObject>();

    [SerializeField] private GameObject holster;

    [SerializeField] private GameObject playerHitParticle;
    [SerializeField] private GameObject hitParticle;

    private void Start()
    {
        //playerWeaponsList = WeaponManager.LocalPlayerInstance.GetWeapons();
    }

    private void Update()
    {
        WeaponScroll();
        HandleShooting();
        
    }

    public void AddWeapon(GameObject weapon) 
    {
        Debug.Log("WeaponAdded: " + weapon.gameObject.name);

        playerWeaponsList.Add(weapon);
        GameObject weaponInstance = Instantiate(weapon, holster.transform.position, holster.transform.rotation);
        weaponInstance.transform.parent = holster.transform.parent;
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
    }
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
        //Debug.Log("Damage: " + _damage);
        //Debug.Log("Range: " + _range);
        //Debug.Log("Radius: " + _radius);
        //Debug.Log("rOf: " + _rateOfFire);
    }

    [SerializeField] private AudioSource audioSource;

    private float timer = 0f;
    private void HandleShooting() 
    {
        UI_Controller.instance.timerSlider.maxValue = _rateOfFire;
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
                    Debug.Log("Shoot");
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
                    Debug.Log("Shoot");
                }
            }
        }
        
        timer += Time.deltaTime;
    }

    private void SphereCast() 
    {
        RaycastHit hit;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.SphereCast(ray.origin, _radius, ray.direction, out hit, _range)) 
        {
            if(hit.collider.tag == "Player" && hit.collider.gameObject.GetPhotonView() != null) 
            {
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, _damage, PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.Instantiate(playerHitParticle.name, hit.point, Quaternion.identity);

            }
            else 
            {
                PhotonNetwork.Instantiate(hitParticle.name, hit.point, Quaternion.identity);
            }
        }
    }
}
