using UnityEngine;
using Unity.XR.CoreUtils;

public class DelayedTrailRenderer : MonoBehaviour
{
    [Header("Trail Settings")]
    [Tooltip("Delay in seconds before the trail starts rendering")]
    [SerializeField] private float trailStartDelay = 2f;
    
    [Tooltip("Scale trail width with camera distance")]
    [SerializeField] private bool scaleWithDistance = true;
    
    [Tooltip("Base width of the trail at reference distance")]
    [SerializeField] private float baseWidth = 0.5f;
    
    [Tooltip("Reference distance for base width (trail will scale relative to this)")]
    [SerializeField] private float referenceDistance = 400f;
    
    private TrailRenderer trailRenderer;
    private Camera mainCamera;
    private float xrOriginScale = 1f; // AR scale compensation
    private bool isARMode = false;
    
    void Awake()
    {
        // Get the TrailRenderer component
        trailRenderer = GetComponent<TrailRenderer>();
        
        // Get main camera
        mainCamera = Camera.main;
        
        // Check if in AR mode and get XR Origin scale
        var xrOrigin = FindFirstObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            isARMode = true;
            xrOriginScale = xrOrigin.transform.localScale.x; // Assuming uniform scale
            Debug.Log($"{gameObject.name}: AR Mode detected, XR Origin scale: {xrOriginScale}");
        }
        
        if (trailRenderer != null)
        {
            // Disable the trail at start
            trailRenderer.emitting = false;
            
            // Enable it after delay
            Invoke(nameof(EnableTrail), trailStartDelay);
            
            Debug.Log($"{gameObject.name}: Trail will start in {trailStartDelay} seconds");
        }
        else
        {
            Debug.LogWarning($"No TrailRenderer found on {gameObject.name}");
        }
    }
    
    void Update()
    {
        if (scaleWithDistance && trailRenderer != null && mainCamera != null)
        {
            // Calculate distance from camera to this object
            float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
            
            // In AR mode, compensate for XR Origin scale
            // The actual world distance is scaled up, so we need to scale down the trail
            if (isARMode && xrOriginScale > 1f)
            {
                distance /= xrOriginScale; // Normalize distance
            }
            
            // Scale width proportionally to distance
            // Width = baseWidth * (distance / referenceDistance)
            float scaledWidth = baseWidth * (distance / referenceDistance);
            
            // Update trail width
            trailRenderer.startWidth = scaledWidth;
            trailRenderer.endWidth = scaledWidth * 0.5f; // End is half the start width for taper
        }
    }
    
    void EnableTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
            Debug.Log($"Trail enabled for {gameObject.name}");
        }
    }
    
    // Optional: Method to manually clear the trail
    public void ClearTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            Debug.Log($"Trail cleared for {gameObject.name}");
        }
    }
    
    // Optional: Method to restart the trail
    public void RestartTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = false;
            Invoke(nameof(EnableTrail), trailStartDelay);
        }
    }
}

