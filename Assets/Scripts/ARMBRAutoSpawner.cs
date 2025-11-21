using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Auto-spawns MBR spacecraft when tapping on AR surface
/// No need to select from menu - just tap and it appears
/// </summary>
public class ARMBRAutoSpawner : MonoBehaviour
{
    [Header("Spacecraft Prefab")]
    [Tooltip("The MBR spacecraft prefab to spawn")]
    [SerializeField] private GameObject spacecraftPrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Scale of spawned spacecraft")]
    [SerializeField] private float spawnScale = 0.1f;
    
    [Tooltip("Only allow one spacecraft at a time")]
    [SerializeField] private bool onlyOneSpacecraft = true;
    
    [Header("AR References")]
    [Tooltip("AR Raycast Manager for detecting surfaces")]
    [SerializeField] private ARRaycastManager raycastManager;
    
    [Tooltip("AR Plane Manager (optional, for plane detection)")]
    [SerializeField] private ARPlaneManager planeManager;
    
    [Header("Current Spacecraft")]
    [SerializeField] private GameObject currentSpacecraft;
    
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    void OnEnable()
    {
        // Enable Enhanced Touch for new Input System
        EnhancedTouchSupport.Enable();
    }
    
    void OnDisable()
    {
        // Disable Enhanced Touch when component disabled
        EnhancedTouchSupport.Disable();
    }
    
    void Start()
    {
        // Auto-find AR components
        if (raycastManager == null)
        {
            raycastManager = FindFirstObjectByType<ARRaycastManager>();
            if (raycastManager != null)
            {
                Debug.Log("ARMBRAutoSpawner: Found AR Raycast Manager");
            }
            else
            {
                Debug.LogError("ARMBRAutoSpawner: No AR Raycast Manager found!");
            }
        }
        
        if (planeManager == null)
        {
            planeManager = FindFirstObjectByType<ARPlaneManager>();
        }
        
        Debug.Log("ARMBRAutoSpawner: Ready - Tap anywhere on surface to spawn MBR spacecraft");
    }
    
    void Update()
    {
        // Check for touch input using new Input System
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            
            // Only spawn on touch began (not while dragging)
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                TrySpawnSpacecraft(touch.screenPosition);
            }
        }
    }
    
    void TrySpawnSpacecraft(Vector2 screenPosition)
    {
        if (spacecraftPrefab == null || raycastManager == null)
        {
            Debug.LogWarning("ARMBRAutoSpawner: Missing prefab or raycast manager");
            return;
        }
        
        // Check if we already have a spacecraft and only one is allowed
        if (onlyOneSpacecraft && currentSpacecraft != null)
        {
            Debug.Log("ARMBRAutoSpawner: Spacecraft already spawned. Delete it first to spawn new one.");
            return;
        }
        
        // Raycast to find AR plane
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Get hit pose
            Pose hitPose = hits[0].pose;
            
            // Spawn spacecraft at hit position
            GameObject spacecraft = Instantiate(spacecraftPrefab, hitPose.position, Quaternion.identity);
            
            // Scale it
            spacecraft.transform.localScale = Vector3.one * spawnScale;
            
            // Make it face up (Y-axis up)
            spacecraft.transform.rotation = Quaternion.Euler(-90, 0, 0); // Adjust based on your model orientation
            
            // Store reference
            currentSpacecraft = spacecraft;
            
            Debug.Log($"ARMBRAutoSpawner: Spawned MBR spacecraft at {hitPose.position}, scale: {spawnScale}");
        }
        else
        {
            Debug.Log("ARMBRAutoSpawner: No AR plane detected at tap position");
        }
    }
    
    /// <summary>
    /// Delete current spacecraft (works with both manual spawner and auto-spawner)
    /// Call this from a Delete button
    /// </summary>
    public void DeleteSpacecraft()
    {
        // First try to delete the one we spawned
        if (currentSpacecraft != null)
        {
            Debug.Log("ARMBRAutoSpawner: Deleting spacecraft");
            Destroy(currentSpacecraft);
            currentSpacecraft = null;
            return;
        }
        
        // If we didn't spawn it, look for any MBR spacecraft in the scene
        // This handles objects spawned by the Object Spawner system
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            // Check if it's an MBR spacecraft (by name)
            if (obj.name.Contains("MBR") && (obj.name.Contains("FINAL") || obj.name.Contains("Spacecraft")))
            {
                // Make sure it's a spawned instance (has Clone in name) or is the active one
                Debug.Log($"ARMBRAutoSpawner: Found and deleting '{obj.name}'");
                Destroy(obj);
                return;
            }
        }
        
        Debug.Log("ARMBRAutoSpawner: No spacecraft found to delete");
    }
    
    /// <summary>
    /// Change spawn scale
    /// </summary>
    public void SetSpawnScale(float scale)
    {
        spawnScale = scale;
        
        // Also update current spacecraft if it exists
        if (currentSpacecraft != null)
        {
            currentSpacecraft.transform.localScale = Vector3.one * spawnScale;
            Debug.Log($"ARMBRAutoSpawner: Updated spacecraft scale to {spawnScale}");
        }
    }
    
    /// <summary>
    /// Get current spacecraft GameObject
    /// </summary>
    public GameObject GetCurrentSpacecraft()
    {
        return currentSpacecraft;
    }
    
    /// <summary>
    /// Check if spacecraft is currently spawned
    /// </summary>
    public bool HasSpacecraft()
    {
        return currentSpacecraft != null;
    }
}


