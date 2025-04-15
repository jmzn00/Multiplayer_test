using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform OwnerTransform;

    private float m_cameraPitch = 0.0f;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        m_cameraPitch = Mathf.Clamp(m_cameraPitch - mouseY, -80.0f, 80.0f);

        OwnerTransform.Rotate(Vector3.up, mouseX);
        transform.localRotation = Quaternion.Euler(m_cameraPitch, 0.0f, 0.0f);
    }
}
