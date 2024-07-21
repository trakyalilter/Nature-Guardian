using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public Camera playerCamera;
    private CharacterController characterController;

    private float verticalSpeed = 0.0f;
    private float gravity = -9.81f;
    private float jumpHeight = 1.0f;
    private float xRotation = 0.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Hide and lock the cursor
    }

    void Update()
    {
        MovePlayer();
        RotateCamera();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal") * speed;
        float moveZ = Input.GetAxis("Vertical") * speed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (characterController.isGrounded)
        {
            verticalSpeed = -1;
            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        move.y = verticalSpeed;

        characterController.Move(move * Time.deltaTime);

        // Adjust Y position based on terrain height
        Vector3 position = transform.position;
        position.y = Terrain.activeTerrain.SampleHeight(position) + 1.0f;
        transform.position = position;
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
