using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ConstantSizeText : MonoBehaviour
{
    public Camera mainCamera;
    public float baseSize = 1f; // Adjust this in the Inspector for desired visual size
    
    private float cachedFOV;
    private Vector3 targetScale;
    private const float smoothSpeed = 15f; // Smoothing speed for scale changes
    private bool isARMode = false;

    void Start()
    {
        // Check if in AR mode
        isARMode = (FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>() != null);
        
        // Find the main camera if not assigned manually
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // Match the camera's rotation initially
        transform.rotation = mainCamera.transform.rotation;
        
        // Cache initial FOV
        if (mainCamera != null)
        {
            cachedFOV = mainCamera.fieldOfView;
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;
        
        // Calculate the distance from the camera
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        // Update cached FOV smoothly to prevent jitter
        cachedFOV = Mathf.Lerp(cachedFOV, mainCamera.fieldOfView, Time.deltaTime * smoothSpeed);
        
        // Calculate target scale based on distance and FOV
        float scaleMultiplier = distance * baseSize * (cachedFOV / 60f);
        targetScale = Vector3.one * scaleMultiplier;
        
        // Smoothly interpolate to target scale to prevent jitter
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);

        // Match the camera's rotation so text follows camera orientation
        transform.rotation = mainCamera.transform.rotation;
    }
}
