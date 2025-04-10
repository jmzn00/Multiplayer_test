using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Body Parts")]
    [SerializeField] Transform viewPoint;


    [SerializeField] float mouseSensitivity = 1f;
    private float verticalRotationStore;
    private Vector2 mouseInput;

    
    public float moveSpeed = 25f;
    public float walkSpeed = 15f;
    private float activeMoveSpeed;

    private Vector3 moveDirection;
    private Vector3 movement;
    

    [SerializeField] CharacterController characterController;

    private Camera playerCamera;

    public float jumpForce = 20;
    int jumpIterations = 0;
    public float gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;
    public LayerMask wallLayer;

    public GameObject bulletImpact;
    //public float timeBetweenShots = 0.1f;
    private float shotCounter;

    public Gun[] allGuns;
    private int selectedGun;

    public GameObject playerHitImpact;

    public int maxHealth = 100;
    public int currentHealth;

    //For WallRunJump
    private bool canJump = true;

    [Header("Nametag")]
    [SerializeField] int nametagYoffset = 2;
    [SerializeField] TMP_Text nametag;
    [SerializeField] Vector3 targetRotation = new Vector3(0, 180, 0);

    [Header("Weapons")]
    public bool hasSemiWeapon;
    public bool hasAutoWeapon;
    public GameObject autoWeaponPickup;
    public GameObject semiWeaponPickup;
    public float semiWeaponCooldownTime = 3f;
    [SerializeField] ParticleSystem semiMuzzleFlash;
    [SerializeField] ParticleSystem muzzleFlash;

    [Header("Fart")]
    public GameObject FartParticle;
    public float radius = 5f;
    public float maxDistance = 10f;
    public bool canFart;

    [Header("Heat")]
    public float maxHeat = 10f;
    public float coolRate = 4f;
    public float overHeatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    [Header("Timer")]
    public float shotTimer = 0f;
    public bool startTimer = false;
    public bool canShootSemi = true;



    private void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }

    void Start()
    {
        selectedGun = 0;

        canShootSemi = true;

        hasSemiWeapon = false;
        hasAutoWeapon = false;

        currentHealth = maxHealth;

        playerCamera = Camera.main;

        UI_Controller.instance.weaponTempSlider.maxValue = maxHeat;
        UI_Controller.instance.semiWeaponSlider.maxValue = semiWeaponCooldownTime;

        UI_Controller.instance.healthSlider.maxValue = maxHealth;
        UI_Controller.instance.healthSlider.value = maxHealth;

        //SwitchWeapon();
        //photonView.RPC("SetGun", RpcTarget.All, selectedGun);

        nametag.text = photonView.Owner.NickName;

        if(photonView.IsMine)
        {
            if (PhotonNetwork.LocalPlayer.NickName == "p1 ")
            {
                JaakkoSpecialTreatment();
            }
        }
        /*
        if (PhotonNetwork.IsMasterClient) 
        {
            gameObject.tag = "Master";
        }master Tag
        */

        /*
        Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        transform.position = SpawnManager.instance.GetSpawnPoint().position;
        transform.rotation = SpawnManager.instance.GetSpawnPoint().rotation;
        */
        //transform.position = Vector3.zero;

    }

    void Update()
    {
        NewHandleMovement();
        HandleRotation();
        //HandleNametag();

        if (photonView.IsMine) 
        {
            //Timer();
            //HandleMovement();
            //HandleRotation();
            //HandleWallRun();
            //HandleHeat();
            //PickupWeapon();
            //HandleDash();
            HandleCursorLock();


            UI_Controller.instance.semiWeaponSlider.value = shotTimer;
            UI_Controller.instance.weaponTempSlider.value = heatCounter;

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                selectedGun++;

                if (selectedGun >= allGuns.Length)
                {
                    selectedGun = 0;
                }
                //SwitchWeapon();
                //SetGunLocally(selectedGun);
            }

            for (int i = 0; i < allGuns.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    selectedGun = i;
                    //SwitchWeapon();
                    //SetGunLocally(selectedGun);
                }
            }
        }

    }
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform viewPointPos;
    private void NewHandleMovement() 
    {
        Debug.DrawRay(viewPointPos.position, Vector3.forward, Color.red, 0.1f);
    }
    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            playerCamera.transform.position = viewPoint.transform.position;
            playerCamera.transform.rotation = viewPoint.transform.rotation;
        }
    }

    private void HandleHeat() 
    {
        if (!overHeated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleShooting();
            }
            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    HandleShooting();
                }
            }
            heatCounter -= coolRate * Time.deltaTime;
        }
        else
        {
            heatCounter -= overHeatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                overHeated = false;
                UI_Controller.instance.overheatedMessage.gameObject.SetActive(false);
            }
        }
        if (heatCounter < 0)
        {
            heatCounter = 0;
        }
    }
    private void SwitchWeapon()
    {
        foreach (Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
    }

    [PunRPC]
    public void SetGunLocally(int gunToSwitchTo)
    {
        /*
        if(gunToSwitchTo < allGuns.Length) 
        {
            selectedGun = gunToSwitchTo;
            SwitchWeapon();
        }
        */
        /*
        if (gunToSwitchTo < allGuns.Length)
        {
            if(gunToSwitchTo == 1) 
            {
                if (hasAutoWeapon) 
                {
                    selectedGun = gunToSwitchTo;
                    SwitchWeapon();
                }
                else 
                {
                    selectedGun = gunToSwitchTo - 1;
                    SwitchWeapon();
                }
            }
            else if(gunToSwitchTo == 2) 
            {
                if (hasSemiWeapon) 
                {
                    selectedGun = gunToSwitchTo;
                    SwitchWeapon();
                }
                else 
                {
                    selectedGun = 0;
                    SwitchWeapon();
                }
            }
            else
            {
                selectedGun = gunToSwitchTo;
                SwitchWeapon();
            }
        }
        */
        if (gunToSwitchTo < allGuns.Length)
        {
            int finalGun = gunToSwitchTo;

            if (gunToSwitchTo == 1 && !hasAutoWeapon)
            {
                finalGun = 0;
            }
            else if (gunToSwitchTo == 2 && !hasSemiWeapon)
            {
                finalGun = 0;
            }

            selectedGun = finalGun;
            SwitchWeapon();

            // Send the final selected gun to other players
            photonView.RPC("UpdateGunDisplay", RpcTarget.Others, selectedGun);
        }

    }

    [PunRPC]
    void UpdateGunDisplay(int newGun) 
    {
        selectedGun = newGun;
        SwitchWeapon();
    }
    private void PickupWeapon()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (hit.collider.gameObject.tag == "SemiWeaponPickup")
                {
                    hasSemiWeapon = true;
                    selectedGun = 2;
                    //SetGun(selectedGun);
                    //photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                    Destroy(hit.collider.gameObject);
                
                }
                else if (hit.collider.gameObject.tag == "AutoWeaponPickup")
                {
                    hasAutoWeapon = true;
                    selectedGun = 1;
                    //SetGun(selectedGun);
                    //photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                    //Debug.Log(hit.collider.gameObject.tag);
                    Destroy(hit.collider.gameObject);

                }
            }
        }
    }
    private void HandleRotation()
    {
        if (UI_Controller.instance == null) return;
        if (UI_Controller.instance.isPaused) { return; }
        
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            verticalRotationStore += mouseInput.y;
            verticalRotationStore = Mathf.Clamp(verticalRotationStore, -60f, 60f);

            viewPoint.rotation = Quaternion.Euler(-verticalRotationStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        
    }
    private void HandleMovement()
    {
        if (UI_Controller.instance.isPaused) { return; }

        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (isGrounded)
            {
                activeMoveSpeed = walkSpeed;
            }
        }

        else
        {
                activeMoveSpeed = moveSpeed;                     
        }
        

        float yVelocity = movement.y;
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeMoveSpeed;
        movement.y = yVelocity;


        if (characterController.isGrounded)
        {
            movement.y = 0f;
            jumpIterations = 0;
        }

        if (Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, 1f))
        {
            isGrounded = true;
            canJump = true;
        }
        else
        {
            isGrounded = false;
        }
        //Debug.Log("GroundCheckPoint: " + groundCheckPoint.transform.position);
        //Debug.Log("isGrounded: " + isGrounded);

        //isGrounded = Physics.Raycast(groundCheckPoint.transform.position, Vector3.down, 1f, groundLayers);
        //Debug.Log("trans: " + transform.position);

        if (Input.GetButtonDown("Jump") && jumpIterations < 2)
        {
            //RaycastHit hit;

            movement.y = jumpForce;
            jumpIterations++;

            if (jumpIterations == 2)
            {
                FartMechanic();
                /*
                    if(Input.GetButtonDown("Jump"))
                    {
                        if (Physics.Raycast(transform.position, Vector3.left, out hit, 2))
                        {
                            Debug.Log("Ray hit" + hit.collider.name);

                            movement.y = jumpForce;
                            jumpIterations++;

                        }
                        else
                        {
                            Debug.Log("WallRayNoHit");
                        }
                    }
                */

            }
        }




        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

        if (gameObject.activeInHierarchy)
        {
            characterController.Move(movement * Time.deltaTime);
        }

    }
    public void HandleWallRun() 
    {

        float maxDistance = 2f;

        RaycastHit hitLeft = new RaycastHit();
        RaycastHit hitRight = new RaycastHit();
        RaycastHit hitForward = new RaycastHit();
        RaycastHit hitBack = new RaycastHit();

        bool isWallRunning = Physics.Raycast(transform.position, Vector3.left, out hitLeft, maxDistance, wallLayer)
                          || Physics.Raycast(transform.position, Vector3.right, out hitRight, maxDistance, wallLayer)
                          || Physics.Raycast(transform.position, Vector3.forward, out hitForward, maxDistance, wallLayer)
                          || Physics.Raycast(transform.position, Vector3.back, out hitBack, maxDistance, wallLayer);
        /*
        if (isWallRunning)
        {
            Debug.Log("Wall detected!");

            if (hitLeft.collider != null) Debug.Log("Left wall: " + hitLeft.collider.gameObject.name);
            if (hitRight.collider != null) Debug.Log("Right wall: " + hitRight.collider.gameObject.name);
            if (hitForward.collider != null) Debug.Log("Front wall: " + hitForward.collider.gameObject.name);
            if (hitBack.collider != null) Debug.Log("Back wall: " + hitBack.collider.gameObject.name);
        }
        */

        if (isWallRunning)
        {
            gravityMod = 3f;

            if (Input.GetButtonDown("Jump") && canJump)
            {
                
                movement.y = jumpForce;
                canJump = false;
                
            }
        }
        else 
        {
            gravityMod = 4f;
           // gravityMod += Mathf.Lerp(gravityMod, Time.deltaTime * 2f, 2f);       
            //gravityMod = Mathf.Clamp(gravityMod, 2, 4);
        }
            //Debug.Log(gravityMod);
            //Debug.Log(canJump);
        //Debug.Log("isWallRunning: " + isWallRunning);
        //Debug.Log("canJump: " + canJump);
        
    }
    private void FartMechanic() 
    {
        if (!isGrounded)
                PhotonNetwork.Instantiate(FartParticle.name, transform.position, Quaternion.identity);
                Cast();
    }
    private void Cast() 
    {
        RaycastHit hit;
        float offsetAmount = 0.5f; // Adjust the spread
        float maxDistance = 10f;

        Vector3[] offsets = new Vector3[]
        {
        new Vector3(0, 0, 0),                     // Center
        new Vector3(offsetAmount, 0, 0),          // Right
        new Vector3(2 * offsetAmount, 0, 0),      // Right

        new Vector3(-offsetAmount, 0, 0),         // Left
        new Vector3(-offsetAmount * 2, 0, 0),     // Left

        new Vector3(0, 0, offsetAmount),          // Forward
        new Vector3(0, 0, offsetAmount * 2),      // Forward

        new Vector3(0, 0, -offsetAmount),          // Backward
        new Vector3(0, 0, -offsetAmount * 2)       // Backward
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 startPosition = transform.position + offset;

            if (Physics.Raycast(startPosition, Vector3.down, out hit, maxDistance))
            {
                Debug.DrawLine(startPosition, hit.point, Color.red, 5f);

                if (hit.collider.CompareTag("Player"))
                {
                    if (hit.collider.gameObject.GetPhotonView() != null)
                    {
                        PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                        hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, 100);
                        UI_Controller.instance.ShowKilledMessage("Obliterated", hit.collider.gameObject.GetPhotonView().Owner.NickName);
                    }
                }
            }
            else
            {
                Debug.DrawLine(startPosition, startPosition + Vector3.down * maxDistance, Color.yellow, 5f); // Show missed casts
            }
        }
    }
    private void HandleShooting()
    {
        if (UI_Controller.instance.isPaused) { return; }

        if (selectedGun == 0) 
        {
            HandleHandWeapon();
        }
        else if(selectedGun == 1)
        {
            HandleAutoWeapon();
        }
        else if(selectedGun == 2) 
        {
            HandleSemiWeapon();
            canShootSemi = false;
        }
    }
    private void HandleHandWeapon() 
    {
        float offsetAmount = 0.5f; // Increased for better spread

        Vector3[] rayOffsets = new Vector3[]
        {
        Vector3.zero,                             // Center
        playerCamera.transform.up * offsetAmount, // Up
        -playerCamera.transform.up * offsetAmount, // Down
        -playerCamera.transform.right * offsetAmount, // Left
        playerCamera.transform.right * offsetAmount // Right
        };        

        foreach (Vector3 offset in rayOffsets)
        {
            Vector3 rayStart = playerCamera.transform.position + offset;
            Ray ray = new Ray(rayStart, playerCamera.transform.forward); 

            if (Physics.Raycast(ray, out RaycastHit hit, 20f)) 
            {
                //Debug.Log("Hit: " + hit.collider.name);
                //Debug.DrawLine(ray.origin, hit.point, Color.red, 5f);

                if (hit.collider.CompareTag("Player"))
                {
                    if (hit.collider.gameObject.GetPhotonView() != null)
                    {
                        PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);                        
                        hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);
                    }
                }
                else
                {
                    GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                    Destroy(bulletImpactObject, 10f);
                }
                PhotonNetwork.Instantiate(muzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));
            }
            else
            {
                PhotonNetwork.Instantiate(muzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));
                //Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.yellow, 5f);
            }
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots;
        Debug.Log(shotCounter);

        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter > maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;
            UI_Controller.instance.overheatedMessage.gameObject.SetActive(true);
        }

        //muzzleFlash.transform.rotation = playerCamera.transform.rotation;
        //muzzleFlash.Play();
    }
    private void HandleAutoWeapon() 
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = playerCamera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.red, 5f); // Draws from origin to hit point
        }
        else
        {
            //Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 5f); // Draws full-length ray if no hit
        }

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (hit.collider.gameObject != null && hit.collider.gameObject.GetPhotonView() != null)
                {
                    PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                    hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);
                }
            }
            else
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 10f);
            }
            PhotonNetwork.Instantiate(muzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));

        }
        else 
        {
                PhotonNetwork.Instantiate(muzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter > maxHeat)
        {
            heatCounter = maxHeat;

            overHeated = true;

            UI_Controller.instance.overheatedMessage.gameObject.SetActive(true);
        }
        //muzzleFlash.transform.rotation = playerCamera.transform.rotation;
        //muzzleFlash.Play();
    }
    public void Timer() 
    {
        if (startTimer) 
        {
            shotTimer += Time.deltaTime;
            Mathf.Clamp(shotTimer, 0, semiWeaponCooldownTime);
        }
        if(shotTimer >= semiWeaponCooldownTime) 
        {
            startTimer = false;
            canShootSemi = true;
        }
        //Debug.Log(shotTimer);
        //ebug.Log("CanShootSemi" + canShootSemi);
        //Debug.Log("startTimer" + startTimer);
    }
    private void HandleSemiWeapon() 
    {
        if (!canShootSemi) { return; }   

       // Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

        float offsetAmount = 0.25f;

        Vector3[] rayOffsets = new Vector3[]
        {
            Vector3.zero,
            playerCamera.transform.right * offsetAmount,
            -playerCamera.transform.right * offsetAmount,
            playerCamera.transform.up * offsetAmount,
            -playerCamera.transform.up * offsetAmount
        };

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        foreach (Vector3 offset in rayOffsets) 
        {

            ray = new Ray(playerCamera.transform.position + offset, playerCamera.transform.forward);

            Debug.DrawRay(ray.origin + offset, playerCamera.transform.forward * 10f, Color.blue, 10f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    if (hit.collider.gameObject != null && hit.collider.gameObject.GetPhotonView() != null)
                    {
                        PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                        hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);
                    }
                }
                else
                {
                    GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                    Destroy(bulletImpactObject, 10f);
                }
            }
        }

        //ray.origin = playerCamera.transform.position;

        //Debug.DrawRay(ray.origin, playerCamera.transform.forward * 10f, Color.red, 10f);       

        /*
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.red, 5f); // Draws from origin to hit point
        }
        else
        {
            //Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 5f); // Draws full-length ray if no hit
        }

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (hit.collider.gameObject != null && hit.collider.gameObject.GetPhotonView() != null)
                {
                    PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                    hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);
                }
            }
            else
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 10f);
            }

        }
        */
            PhotonNetwork.Instantiate(semiMuzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));
        shotTimer = 0;
        startTimer = true;
            
        

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overHeated = true;

            UI_Controller.instance.overheatedMessage.gameObject.SetActive(true);
        }
    }
    [PunRPC]
    public void DealDamage(string damager, int damageAmount)
    {
        TakeDamage(damager, damageAmount);
    }
    private void TakeDamage(string damager, int damageAmount) 
    {
        if (photonView.IsMine) 
        {

            currentHealth -= damageAmount;
            UI_Controller.instance.healthSlider.value = currentHealth;
            //Debug.Log(photonView.Owner.NickName + "Has been hit by: " + damager);

            if (currentHealth <= 0) 
            {
                currentHealth = 0;
                PlayerSpawner.instance.Die();
                UI_Controller.instance.ShowKilledMessage(" ", damager);

                if (hasAutoWeapon) 
                {
                    PhotonNetwork.Instantiate(autoWeaponPickup.name, transform.position, Quaternion.identity);
                }
                if (hasSemiWeapon) 
                {
                    PhotonNetwork.Instantiate(semiWeaponPickup.name, transform.position, Quaternion.identity);
                }

            }
        }
    }
    private void HandleCursorLock() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0) && !UI_Controller.instance.pauseScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    private void JaakkoSpecialTreatment()
    {
        nametag.color = Color.yellow;
    }
    private void HandleNametag()
    {
        if (nametag != null)
        {
            Vector3 textPosition = new Vector3(transform.position.x,
                transform.position.y + nametagYoffset, transform.position.z);
            nametag.transform.position = textPosition;

            float speed = 180f / 3f;
            nametag.transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }
}
