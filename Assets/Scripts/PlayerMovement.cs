using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Data.Common;

public class PlayerMovement : MonoBehaviourPun
{
    private Camera playerCamera;
    private Rigidbody playerRigidBody;

    private float mouseSensitivity = 2f;

    [Header("Camera")]
    public float cameraPitch = 0f;
    public const float maxCameraPitch = 45f;
    public const float minCameraPitch = -45f;
    public float cameraYaw = 0f;
    public const float maxCameraYaw = 90f;
    public const float minCameraYaw = -90f;

    [Header("Player")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform playerTransform;


    [Header("Movement")] 
    public float mouseX;
    public float mouseY;
    public float moveX;
    public float moveY;    
    public float groundEndStrafeSpeed = 12f;
    public float strafeAccelerationTime = 0.3f;
    public float strafeDecelerationTime = 1f;
    public float groundEndSpeed = 12f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;
    public float currentSpeedX = 0f;
    public float currentSpeedY = 0f;
    public Vector3 moveDirection;

    [Header("SlopeHandling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;

    [Header("GroundCheck")]
    public bool isGrounded;
    public LayerMask groundLayer;
    public Vector3 groundCheckPoint;


    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Weapon")]
    [SerializeField] private ParticleSystem semiWeaponMuzzleFlash;

    private void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (photonView.IsMine) 
        {
            playerCamera = Camera.main;
        }        
        playerRigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!photonView.IsMine) { return; }

        PlayerInput();       
        //PrimativeShooting();
        HandleCursorLock();
    }
    private void LateUpdate()
    {
        CameraMovement();
    }
    private void FixedUpdate()
    {
        RotateBody();
        GroundMovement();
        //AirMovement();
    }

    private void PlayerInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
    }

    private void CameraMovement()
    {
        playerCamera.transform.position = headTransform.GetComponent<Renderer>().bounds.center;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minCameraPitch, maxCameraPitch);
        cameraYaw += mouseX;

        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        headTransform.transform.rotation = playerCamera.transform.localRotation;
    }
    
    private void RotateBody()
    {
        bodyTransform.rotation = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
        //Debug.DrawLine(headTransform.position, headTransform.position + headTransform.forward * 2f, Color.red, 0.01f);
        //Debug.DrawLine(bodyTransform.position, bodyTransform.position + bodyTransform.forward * 2f, Color.blue, 0.01f);
        //Debug.Log(headTransform.rotation.y);

        /*
        Quaternion targetRotation = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
        bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, targetRotation, Time.deltaTime * 2f);

        if (isMoving) 
        {
            //bodyTransform.rotation = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
            bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, 0.1f);
        }
        */
        /*
        Vector3 currentCameraRotation = playerCamera.transform.rotation.eulerAngles;

        if (isMoving)
        {

            if (initialYaw == 0f)
            {
                initialYaw = headTransform.rotation.eulerAngles.y;
            }            

            float targetYaw = headTransform.rotation.eulerAngles.y - initialYaw;  // Reset yaw offset based on initialYaw
            Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);

            // Rotate the body smoothly towards the target yaw
            bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetRotation, 2.5f);
        }
        if (currentCameraRotation != lastCameraRotation)
        {
            // Rotate the body (the player) to match the camera's yaw (horizontal rotation)+

            // Apply rotation to the body (player) based on yaw
            
            float deltaYaw = cameraYaw - lastCameraRotation.y; // Get the difference in yaw from last frame
            if (cameraYaw > 45 || cameraYaw < -45)
            {
                bodyTransform.transform.Rotate(0f, deltaYaw, 0f); // Rotate the body on the Y-axis based on the yaw difference
            }
            


            // Update the last rotation to the current one
            lastCameraRotation = currentCameraRotation;
        }
        */
    }

    public Transform Cube;
    private void GroundMovement() 
    {
        CheckGroundLayer();

        float targetSpeedX = 0f;
        float targetSpeedY = 0f;

        float sprintSpeed = 1f;

        if (!isGrounded) { return; }

        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            sprintSpeed = 1.25f;
        }
        else 
        {
            sprintSpeed = 1f;
        }

        if (moveX != 0)
        {
            targetSpeedX = moveX * groundEndStrafeSpeed;
            currentSpeedX = Mathf.MoveTowards(currentSpeedX, targetSpeedX, (groundEndStrafeSpeed / strafeAccelerationTime) * Time.deltaTime);
        }
        else
        {
            currentSpeedX = Mathf.MoveTowards(currentSpeedX, 0f, (groundEndStrafeSpeed / strafeDecelerationTime) * Time.deltaTime);
        }

        if (moveY != 0)
        {
            targetSpeedY = moveY * groundEndSpeed * sprintSpeed;
            currentSpeedY = Mathf.MoveTowards(currentSpeedY, targetSpeedY, (groundEndSpeed / accelerationTime) * Time.deltaTime);
        }
        else
        {
            currentSpeedY = Mathf.MoveTowards(currentSpeedY, 0f, (groundEndSpeed / decelerationTime) * Time.deltaTime);
        }


        Vector3 cubePos = new Vector3(playerTransform.position.x, playerTransform.position.y - 1.75f, playerTransform.position.z);
        Cube.position = cubePos;


         /* _________SLOPE_________*/

        RaycastHit slopeRayHit;

        
        if(Physics.Raycast(Cube.position, Vector3.down, out slopeRayHit, 0.25f)) 
        {
            /*
            float angle = Vector3.Angle(Vector3.up, slopeRayHit.normal);

            Vector3 currentEuler = Cube.rotation.eulerAngles;
            Vector3 targetEuler = new Vector3(angle, headTransform.rotation.eulerAngles.y, 0f);

            Cube.rotation = Quaternion.Euler(targetEuler);
            */

            Vector3 slopeNormal = slopeRayHit.normal;

            // Get the desired Y rotation (like from input or headTransform)
            float yaw = headTransform.eulerAngles.y;

            // Create a basis from the slope:
            Vector3 slopeForward = Vector3.ProjectOnPlane(headTransform.forward, slopeNormal).normalized;
            Vector3 slopeRight = Vector3.Cross(slopeNormal, slopeForward).normalized;

            // Rebuild the rotation from the slope-aware axes
            Quaternion slopeRotation = Quaternion.LookRotation(slopeForward, slopeNormal);

            // Apply it to the cube
            Cube.rotation = slopeRotation;

            float angle = Vector3.Angle(Vector3.up, slopeRayHit.normal);
            float force;
            float rampForce = 200f;
            float groundForce = 100f;

            if(angle != 0) 
            {
                playerRigidBody.AddForce(-Cube.up * 75f, ForceMode.Force);
                force = rampForce;
            }
            else 
            {
                force = groundForce;
            }


            if (moveX != 0)
            {
                playerRigidBody.AddForce(Cube.right * moveX * force, ForceMode.Force);
            }
            if(moveY != 0) 
            {
                playerRigidBody.AddForce(Cube.forward * moveY * force, ForceMode.Force);
            }
            /*
            if(angle != 0) 
            {
                playerRigidBody.AddForce(-Cube.transform.up * 250f, ForceMode.Force);

                if(moveY != 0) 
                {
                    playerRigidBody.AddForce(Cube.transform.right * moveY * 500f);
                }
                if(moveX != 0) 
                {
                    playerRigidBody.AddForce(Cube.transform.forward * moveX * 500f);
                }
            } 
            */
           
        }

        //moveDirection = (bodyTransform.right * currentSpeedX) + (bodyTransform.forward * currentSpeedY);

        //Vector3 velocity = (bodyTransform.right * currentSpeedX) + (bodyTransform.forward * currentSpeedY);
        //playerRigidBody.linearVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
        
        
    }



        
    
    private bool OnSlope()
    {
        Vector3 slopeCheckOffset = moveDirection.normalized * 1f;
        Vector3 slopeCheckPoint = transform.position + slopeCheckOffset;
        Debug.DrawRay(slopeCheckPoint, Vector3.down);
        //Vector3 slopeCheckPoint = new Vector3(playerTransform.position.x + 1f , playerTransform.position.y - 0.5f, playerTransform.position.z);
        if (Physics.Raycast(slopeCheckPoint, Vector3.down, out slopeHit, 3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log(angle);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void PrimativeShooting() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = new Ray(headTransform.position, headTransform.forward);
            if(Physics.Raycast(ray, out RaycastHit hit, 20f)) 
            {
                if(hit.collider.tag == "Player") 
                {
                    hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, 100);
                }               
            }
            PhotonNetwork.Instantiate(semiWeaponMuzzleFlash.name, ray.origin, Quaternion.LookRotation(ray.direction));           
        }
    }

    private void AirMovement()
    {        
        if(isGrounded) {return;}
    }

    private void CheckGroundLayer()
    {
        groundCheckPoint = new Vector3(bodyTransform.position.x, bodyTransform.position.y - 1f, bodyTransform.position.z);

        if (Physics.Raycast(groundCheckPoint, Vector3.down, 0.25f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    [PunRPC]
    public void DealDamage(int damageAmount) 
    {
        TakeDamage(damageAmount);
    }

    private void TakeDamage(int damageAmount) 
    {
        if (!photonView.IsMine) {return;}

        currentHealth -= damageAmount;

        if(currentHealth <= 0) 
        {
            currentHealth = 0;
            PlayerSpawner.instance.Die();
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
}
