using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ConstantSizeText : MonoBehaviour
{
    public Camera mainCamera;
    public float baseSize = 1f; // Adjust this in the Inspector for desired visual size

    void Start()
    {
        // Find the main camera if not assigned manually
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // Match the camera's rotation initially
        transform.rotation = mainCamera.transform.rotation;
    }

    void Update()
    {
        // Calculate the distance from the camera
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        // Adjust the local scale based on the distance
        // The idea is to counteract the perspective scaling
        transform.localScale = Vector3.one * distance * baseSize * (Camera.main.fieldOfView / 60f); // Adjusts for different FOVs

        // Match the camera's rotation so text follows camera orientation
        transform.rotation = mainCamera.transform.rotation;
    }
}
