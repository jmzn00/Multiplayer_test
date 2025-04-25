using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerQuake : MonoBehaviourPun
{
    [SerializeField] private string yAxisInput = "Vertical";
    [SerializeField] private string xAxisInput = "Horizontal";
    [SerializeField] private string inputMouseX = "Mouse X";
    [SerializeField] private string inputMouseY = "Mouse Y";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float groundAcceleration = 100f;
    [SerializeField] private float airAcceleration = 100f;
    [SerializeField] private float groundLimit = 12f;
    [SerializeField] private float airLimit = 1f;
    [SerializeField] private float gravity = 16f;
    [SerializeField] private float friction = 6f;
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float rampSlideLimit = 5f;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private bool additiveJump = true;
    [SerializeField] private bool autoJump = true;
    [SerializeField] private bool clampGroundSpeed = false;
    [SerializeField] private bool disableBunnyHopping = false;

    private Rigidbody rb;

    private Vector3 vel;
    private Vector3 inputDir;
    private Vector3 _inputRot;
    private Vector3 groundNormal;

    private bool onGround = false;
    private bool jumpPending = false;
    private bool ableToJump = true;

    public Vector3 InputRot { get => _inputRot; }

    private Camera playerCamera;

    private int health;
    private int maxHealth = 100;

    public int money = 0;
    [SerializeField] private GameObject moneyGameObject;

    public static PlayerControllerQuake localPlayer;


    void Start()
    {
        rb = GetComponent<Rigidbody>();       
        // Lock cursor
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        if (photonView.IsMine) 
        {
            localPlayer = this;
            playerCamera = Camera.main;
            health = maxHealth;
            
            UI_Controller.instance.healthSlider.maxValue = maxHealth;
            UI_Controller.instance.healthSlider.value = maxHealth;
            UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
        }

    }
    private void Update()
    {
        if (!photonView.IsMine) { return; }
        MouseLook();
        GetMovementInput();
        HandleCursorLock();
        //Debug.Log(money);
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine) { return; }

        vel = rb.linearVelocity;

        // Clamp speed if bunnyhopping is disabled
        if (disableBunnyHopping && onGround)
        {
            if (vel.magnitude > groundLimit)
                vel = vel.normalized * groundLimit;
        }

        // Jump
        if (jumpPending && onGround)
        {
            Jump();
        }

        // We use air physics if moving upwards at high speed
        if (rampSlideLimit >= 0f && vel.y > rampSlideLimit)
            onGround = false;

        if (onGround)
        {
            // Rotate movement vector to match ground tangent
            inputDir = Vector3.Cross(Vector3.Cross(groundNormal, inputDir), groundNormal);

            GroundAccelerate();
            ApplyFriction();
        }
        else
        {
            ApplyGravity();
            AirAccelerate();
        }

        rb.linearVelocity = vel;

        // Reset onGround before next collision checks
        onGround = false;
        groundNormal = Vector3.zero;
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine) { return; }
        if(MatchManager.instance.state == MatchManager.GameState.Playing) 
        {
            CameraFollow();
        }
        else 
        {
            playerCamera.transform.position = MatchManager.instance.mapCamPoint.position;
            playerCamera.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
        }
        
    }
    public float GetMagnitude() 
    {
        return vel.magnitude;
    }
    [PunRPC]
    public void DealDamage(string damagerNickname, int damageAmount, int actor) 
    {
        TakeDamage(damagerNickname, damageAmount, actor);
    }
    private void TakeDamage(string damagerNickname, int damageAmount, int actor) 
    {
        if (!photonView.IsMine) { return; }

        health -= damageAmount;
        UI_Controller.instance.healthSlider.value = health;

        if(health <= 0) 
        {
            int moneyValue = MoneyManager.instance.money;  // or whatever your player money variable is
            object[] instantiationData = new object[] { moneyValue };        



            if (PhotonNetwork.IsMasterClient)
            {
                if(moneyValue > 0) 
                {
                    PhotonNetwork.Instantiate(moneyGameObject.name, transform.position, Quaternion.identity, 0, instantiationData);
                }
                               
            }
            else
            {
                photonView.RPC("RequestMoneyDrop", RpcTarget.MasterClient, transform.position, moneyValue);
            }

            newPlayerWeapons playerWeapons = GetComponent<newPlayerWeapons>();
            PlayerItems playerItems = GetComponent<PlayerItems>();
            playerWeapons.DropWeapons();
            playerItems.DropItems();

            

            health = 0;
            
            MatchManager.instance.UpdateStatsSend(actor, 0, 1);

            MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, (MoneyManager.instance.money / 2) * -1);
            MoneyManager.instance.money -= MoneyManager.instance.money / 2;

            if (UI_Controller.instance.ShopPanel.activeInHierarchy) 
            {
                UI_Controller.instance.ShopPanel.SetActive(false);
            }

            PlayerSpawner.instance.Die();
        }
    }
    public void HealPlayer(int healAmount) 
    {
        health += healAmount;
        
        if(health > maxHealth) 
        {
            health = maxHealth;    
        }
        UI_Controller.instance.healthSlider.value = health;
    }
    [PunRPC]
    public void RequestMoneyDrop(Vector3 position, int moneyValue)
    {
        if(moneyValue > 0) 
        {
            PhotonNetwork.Instantiate(moneyGameObject.name, position, Quaternion.identity, 0, new object[] { moneyValue });
        }
    }
    public void AddMoneyLocally(int amount) 
    {
        MoneyManager.instance.money += amount;

        UI_Controller.instance.moneyValueText.text = MoneyManager.instance.money.ToString();
        //Debug.Log("MoneyManager: " + MoneyManager.instance.money);
        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 4, amount);

    }
    private void HandleCursorLock()
    {
        if (Input.GetMouseButtonDown(0) && !UI_Controller.instance.pauseScreen.activeInHierarchy && !UI_Controller.instance.ShopPanel.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (UI_Controller.instance.pauseScreen.activeInHierarchy || UI_Controller.instance.ShopPanel.activeInHierarchy) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

            //Debug.Log(Cursor.lockState);
    }
    private void CameraFollow() 
    {
        Vector3 cameraPosition = new Vector3(rb.transform.position.x, rb.transform.position.y + 1, rb.transform.position.z);
        playerCamera.transform.position = cameraPosition;

        playerCamera.transform.rotation = Quaternion.Euler(_inputRot.x, _inputRot.y, 0f);
    }

    void GetMovementInput()
    {
        float x = Input.GetAxisRaw(xAxisInput);
        float z = Input.GetAxisRaw(yAxisInput);

        inputDir = transform.rotation * new Vector3(x, 0f, z).normalized;

        if (Input.GetButtonDown(jumpButton))
            jumpPending = true;

        if (Input.GetButtonUp(jumpButton))
            jumpPending = false;
    }

    void MouseLook()
    {
        _inputRot.y += Input.GetAxisRaw(inputMouseX) * mouseSensitivity;
        _inputRot.x -= Input.GetAxisRaw(inputMouseY) * mouseSensitivity;

        if (_inputRot.x > 90f)
            _inputRot.x = 90f;
        if (_inputRot.x < -90f)
            _inputRot.x = -90f;

        transform.rotation = Quaternion.Euler(0f, _inputRot.y, 0f);
    }

    private void GroundAccelerate()
    {
        float addSpeed = groundLimit - Vector3.Dot(vel, inputDir);

        if (addSpeed <= 0)
            return;

        float accelSpeed = groundAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        vel += accelSpeed * inputDir;

        if (clampGroundSpeed)
        {
            if (vel.magnitude > groundLimit)
                vel = vel.normalized * groundLimit;
        }
    }

    private void AirAccelerate()
    {
        Vector3 hVel = vel;
        hVel.y = 0;

        float addSpeed = airLimit - Vector3.Dot(hVel, inputDir);

        if (addSpeed <= 0)
            return;

        float accelSpeed = airAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        vel += accelSpeed * inputDir;
    }

    private void ApplyFriction()
    {
        vel *= Mathf.Clamp01(1 - Time.deltaTime * friction);
    }

    private void Jump()
    {
        if (!ableToJump)
            return;

        if (vel.y < 0f || !additiveJump)
            vel.y = 0f;

        vel.y += jumpHeight;
        onGround = false;

        if (!autoJump)
            jumpPending = false;

        StartCoroutine(JumpTimer());
    }
    private void ApplyGravity()
    {
        vel.y -= gravity * Time.deltaTime;
    }

    private void OnCollisionStay(Collision other)
    {
        // Check if any of the contacts has acceptable floor angle
        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.normal.y > Mathf.Sin(slopeLimit * (Mathf.PI / 180f) + Mathf.PI / 2f))
            {
                groundNormal = contact.normal;
                onGround = true;
                return;
            }
        }
    }

    // This is for avoiding multiple consecutive jump commands before leaving ground
    private IEnumerator JumpTimer()
    {
        ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        ableToJump = true;
    }
}
