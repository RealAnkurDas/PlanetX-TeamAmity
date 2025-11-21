using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

/// <summary>
/// AR viewer for detailed spacecraft inspection
/// Centers camera around spacecraft model and scales appropriately
/// </summary>
public class ARSpacecraftViewer : MonoBehaviour
{
    [Header("Spacecraft")]
    [Tooltip("The spacecraft model to view in AR")]
    [SerializeField] private GameObject spacecraftModel;
    
    [Tooltip("Scale of the spacecraft in AR (makes model bigger/smaller)")]
    [SerializeField] private float spacecraftScale = 0.5f; // 50cm size in AR
    
    [Header("AR Setup")]
    [Tooltip("XR Origin for AR")]
    [SerializeField] private Transform xrOrigin;
    
    [Tooltip("AR Camera")]
    [SerializeField] private Camera arCamera;
    
    [Tooltip("Distance to place spacecraft from camera")]
    [SerializeField] private float placementDistance = 1.5f; // 1.5 meters away
    
    [Tooltip("Height above floor/surface")]
    [SerializeField] private float heightAboveFloor = 0.5f; // 0.5 meters up
    
    [Header("Camera Settings")]
    [Tooltip("Near clipping plane")]
    [SerializeField] private float nearClipPlane = 0.01f;
    
    [Tooltip("Far clipping plane")]
    [SerializeField] private float farClipPlane = 100f;
    
    private bool isPlaced = false;
    private Vector3 originalSpacecraftScale;
    
    void Start()
    {
        // Auto-find spacecraft if not assigned
        if (spacecraftModel == null)
        {
            spacecraftModel = GameObject.Find("MBR FINAL - Copy");
            if (spacecraftModel == null)
            {
                Debug.LogError("ARSpacecraftViewer: Could not find 'MBR FINAL - Copy'!");
            }
            else
            {
                Debug.Log("ARSpacecraftViewer: Found spacecraft model");
            }
        }
        
        // Auto-find XR Origin
        if (xrOrigin == null)
        {
            var xrOriginComponent = FindFirstObjectByType<XROrigin>();
            if (xrOriginComponent != null)
            {
                xrOrigin = xrOriginComponent.transform;
                Debug.Log("ARSpacecraftViewer: Found XR Origin");
            }
        }
        
        // Auto-find AR camera
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
        
        // Configure camera
        if (arCamera != null)
        {
            arCamera.nearClipPlane = nearClipPlane;
            arCamera.farClipPlane = farClipPlane;
            Debug.Log($"ARSpacecraftViewer: Configured camera - Near: {nearClipPlane}, Far: {farClipPlane}");
        }
        
        // Store original spacecraft scale
        if (spacecraftModel != null)
        {
            originalSpacecraftScale = spacecraftModel.transform.localScale;
        }
        
        // Place spacecraft after short delay (let AR initialize)
        Invoke("PlaceSpacecraft", 0.5f);
    }
    
    void PlaceSpacecraft()
    {
        if (spacecraftModel == null || arCamera == null || isPlaced)
            return;
        
        // Get camera position and forward direction
        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;
        
        // Calculate horizontal forward (ignore vertical tilt)
        Vector3 horizontalForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        
        // Calculate placement position
        Vector3 targetPosition = cameraPosition + horizontalForward * placementDistance;
        targetPosition.y = cameraPosition.y + heightAboveFloor;
        
        // Position spacecraft
        spacecraftModel.transform.position = targetPosition;
        
        // Scale spacecraft to desired AR size
        spacecraftModel.transform.localScale = originalSpacecraftScale * spacecraftScale;
        
        // Make spacecraft face camera
        Vector3 lookDirection = cameraPosition - targetPosition;
        lookDirection.y = 0; // Keep horizontal
        if (lookDirection != Vector3.zero)
        {
            spacecraftModel.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
        
        isPlaced = true;
        Debug.Log($"ARSpacecraftViewer: Placed spacecraft at {targetPosition}, scale: {spacecraftScale}");
        Debug.Log($"  Distance from camera: {placementDistance}m, Height: {heightAboveFloor}m");
    }
    
    /// <summary>
    /// Reposition spacecraft in front of camera (call when user moves)
    /// </summary>
    public void ReplaceSpacecraft()
    {
        isPlaced = false;
        PlaceSpacecraft();
    }
    
    /// <summary>
    /// Adjust spacecraft scale in AR
    /// </summary>
    public void SetSpacecraftScale(float scale)
    {
        spacecraftScale = scale;
        if (spacecraftModel != null)
        {
            spacecraftModel.transform.localScale = originalSpacecraftScale * spacecraftScale;
            Debug.Log($"ARSpacecraftViewer: Updated spacecraft scale to {spacecraftScale}");
        }
    }
    
    /// <summary>
    /// Get current spacecraft scale
    /// </summary>
    public float GetSpacecraftScale()
    {
        return spacecraftScale;
    }
    
    /// <summary>
    /// Center camera view on spacecraft
    /// Called when user taps "center" button
    /// </summary>
    public void CenterOnSpacecraft()
    {
        if (spacecraftModel == null || arCamera == null)
            return;
        
        // Calculate direction from camera to spacecraft
        Vector3 direction = (spacecraftModel.transform.position - arCamera.transform.position).normalized;
        
        // User should physically move to align camera with spacecraft
        // We can't move the AR camera (it follows device)
        // So we move the spacecraft to be in front of camera
        
        ReplaceSpacecraft();
    }
}

