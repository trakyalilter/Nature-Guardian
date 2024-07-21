using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera playerCamera;

    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        if (playerCamera != null)
        {
            // Make the text face the camera
            transform.LookAt(playerCamera.transform);
            transform.Rotate(0, 180, 0); // Adjust if the text is mirrored
        }
    }
}
