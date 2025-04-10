using UnityEngine;
using QuakeLR;

public class MovementController : MonoBehaviour
{
    private QuakeCharacterController m_QuakeCharacterController = null;
    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.forward * moveY + transform.right * moveX).normalized;
        
        m_QuakeCharacterController.Move(moveDirection);
        m_QuakeCharacterController.ControllerThink(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            m_QuakeCharacterController.TryJump();
        }
    }

    private void Awake()
    {
        m_QuakeCharacterController = GetComponent<QuakeCharacterController>();

    }

}
