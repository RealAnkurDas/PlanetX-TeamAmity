using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Manages the solar system in AR mode
/// Scales XR Origin to match solar system scale instead of rescaling the system
/// </summary>
public class ARSolarSystemManager : MonoBehaviour
{
    [Header("AR Setup")]
    [Tooltip("The XR Origin to scale (makes the 'player' bigger to see the huge solar system)")]
    [SerializeField] private Transform xrOrigin;
    
    [Tooltip("Scale multiplier for XR Origin (higher = you appear bigger in the world)")]
    [SerializeField] private float xrOriginScale = 1000f; // Makes you 1000x bigger to see solar system
    
    [Header("Solar System")]
    [Tooltip("Root of the solar system (usually the Sun's parent)")]
    [SerializeField] private Transform solarSystemRoot;
    
    [Tooltip("Initial distance to place solar system from camera")]
    [SerializeField] private float initialDistance = 5f; // In AR scale
    
    [Tooltip("Height above detected surface to place solar system")]
    [SerializeField] private float heightAboveFloor = 1.5f; // Meters above floor
    
    [Header("Camera Settings")]
    [Tooltip("AR Camera to configure")]
    [SerializeField] private Camera arCamera;
    
    [Tooltip("Far clipping plane for AR camera")]
    [SerializeField] private float farClipPlane = 10000000f;
    
    [Tooltip("Near clipping plane for AR camera")]
    [SerializeField] private float nearClipPlane = 0.01f;
    
    private bool isPlaced = false;
    
    void Start()
    {
        // Auto-find components if not assigned
        if (xrOrigin == null)
        {
            var xrOriginComponent = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOriginComponent != null)
            {
                xrOrigin = xrOriginComponent.transform;
                Debug.Log("ARSolarSystemManager: Found XR Origin");
            }
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
        
        // Configure AR camera for large-scale viewing
        if (arCamera != null)
        {
            arCamera.farClipPlane = farClipPlane;
            arCamera.nearClipPlane = nearClipPlane;
            Debug.Log($"ARSolarSystemManager: Configured AR camera - Near: {nearClipPlane}, Far: {farClipPlane}");
        }
        
        // Scale XR Origin to make player bigger
        if (xrOrigin != null)
        {
            xrOrigin.localScale = Vector3.one * xrOriginScale;
            Debug.Log($"ARSolarSystemManager: Scaled XR Origin by {xrOriginScale}x");
        }
        
        // Place solar system in front of camera
        PlaceSolarSystem();
    }
    
    void PlaceSolarSystem()
    {
        if (solarSystemRoot == null || arCamera == null || isPlaced)
            return;
        
        // Position solar system in front of AR camera at initial distance
        // The actual distance is scaled by XR Origin scale
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;
        
        // Calculate horizontal forward (ignore vertical component)
        Vector3 horizontalForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        
        // Place at distance (accounting for XR Origin scale)
        // Add height to make it float above the floor
        Vector3 targetPosition = cameraPosition + horizontalForward * (initialDistance * xrOriginScale);
        targetPosition.y += heightAboveFloor * xrOriginScale; // Lift it above floor
        
        solarSystemRoot.position = targetPosition;
        
        isPlaced = true;
        Debug.Log($"ARSolarSystemManager: Placed solar system at {targetPosition}, height: {heightAboveFloor * xrOriginScale}");
    }
    
    /// <summary>
    /// Manually reposition solar system in front of camera
    /// </summary>
    public void ReplaceSolarSystem()
    {
        isPlaced = false;
        PlaceSolarSystem();
    }
    
    /// <summary>
    /// Adjust XR Origin scale dynamically
    /// </summary>
    public void SetXROriginScale(float scale)
    {
        xrOriginScale = scale;
        if (xrOrigin != null)
        {
            xrOrigin.localScale = Vector3.one * xrOriginScale;
            Debug.Log($"ARSolarSystemManager: Updated XR Origin scale to {xrOriginScale}x");
        }
    }
    
    /// <summary>
    /// Get current XR Origin scale
    /// </summary>
    public float GetXROriginScale()
    {
        return xrOriginScale;
    }
    
    /// <summary>
    /// Get the solar system root transform
    /// </summary>
    public Transform GetSolarSystemRoot()
    {
        return solarSystemRoot;
    }
    
    /// <summary>
    /// Get AR camera
    /// </summary>
    public Camera GetARCamera()
    {
        return arCamera;
    }
}

