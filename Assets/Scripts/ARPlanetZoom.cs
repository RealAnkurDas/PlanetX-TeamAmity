using UnityEngine;

/// <summary>
/// Handles zooming to selected planets in AR mode
/// Works with PlanetSelector to focus on the current target
/// </summary>
public class ARPlanetZoom : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AR Solar System Manager")]
    [SerializeField] private ARSolarSystemManager arManager;
    
    [Tooltip("Planet Selector to get current target")]
    [SerializeField] private PlanetSelector planetSelector;
    
    [Header("Zoom Settings")]
    [Tooltip("Smooth zoom transitions")]
    [SerializeField] private float smoothSpeed = 5f;
    
    [Tooltip("Auto-zoom when planet is selected from dropdown")]
    [SerializeField] private bool autoZoomOnSelect = true;
    
    private Transform currentTarget;
    private Vector3 targetSolarSystemPosition;
    private Vector3 originalSolarSystemPosition;
    private bool isZooming = false;
    private bool hasOriginalPosition = false;
    private string lastSelectedPlanet = "";
    
    void Start()
    {
        // Auto-find components
        if (arManager == null)
        {
            arManager = FindFirstObjectByType<ARSolarSystemManager>();
            if (arManager == null)
            {
                Debug.LogError("ARPlanetZoom: Could not find ARSolarSystemManager!");
            }
            else
            {
                Debug.Log("ARPlanetZoom: Found ARSolarSystemManager");
            }
        }
        
        if (planetSelector == null)
        {
            planetSelector = FindFirstObjectByType<PlanetSelector>();
            if (planetSelector == null)
            {
                Debug.LogError("ARPlanetZoom: Could not find PlanetSelector!");
            }
            else
            {
                Debug.Log($"ARPlanetZoom: Found PlanetSelector, current anchor: '{planetSelector.GetCurrentAnchor()}'");
            }
        }
        
        Debug.Log($"ARPlanetZoom: Initialized - Auto-zoom: {autoZoomOnSelect}");
        
        // Store original position after a short delay (let AR Manager place it first)
        Invoke("StoreOriginalPosition", 0.5f);
        
        // Initialize lastSelectedPlanet to current anchor so first change triggers zoom
        if (planetSelector != null)
        {
            // Don't set lastSelectedPlanet so the first Update() will trigger zoom to initial planet
            Debug.Log("ARPlanetZoom: Will auto-zoom to initial planet on first Update()");
        }
    }
    
    void StoreOriginalPosition()
    {
        if (arManager != null)
        {
            Transform solarSystemRoot = arManager.GetSolarSystemRoot();
            if (solarSystemRoot != null)
            {
                originalSolarSystemPosition = solarSystemRoot.position;
                hasOriginalPosition = true;
                Debug.Log($"ARPlanetZoom: Stored original position: {originalSolarSystemPosition}");
            }
        }
    }
    
    void Update()
    {
        // Check if planet selection changed
        if (autoZoomOnSelect && planetSelector != null)
        {
            string currentPlanet = planetSelector.GetCurrentAnchor();
            
            // Debug every frame to see what's happening
            if (Time.frameCount % 60 == 0) // Log once per second
            {
                Debug.Log($"ARPlanetZoom Update: Current='{currentPlanet}', Last='{lastSelectedPlanet}', AutoZoom={autoZoomOnSelect}");
            }
            
            if (currentPlanet != lastSelectedPlanet && !string.IsNullOrEmpty(currentPlanet))
            {
                lastSelectedPlanet = currentPlanet;
                Debug.Log($"ARPlanetZoom: ★★★ Planet selection changed to '{currentPlanet}' - auto-zooming ★★★");
                ZoomToPlanet(currentPlanet);
            }
        }
        else
        {
            // Debug why it's not checking
            if (Time.frameCount % 120 == 0) // Log every 2 seconds
            {
                if (!autoZoomOnSelect)
                    Debug.LogWarning("ARPlanetZoom: Auto-zoom is DISABLED!");
                if (planetSelector == null)
                    Debug.LogWarning("ARPlanetZoom: Planet Selector is NULL!");
            }
        }
        
        // Smoothly move solar system to target position when zooming
        if (isZooming && arManager != null)
        {
            Transform solarSystemRoot = arManager.GetSolarSystemRoot();
            if (solarSystemRoot != null)
            {
                solarSystemRoot.position = Vector3.Lerp(
                    solarSystemRoot.position,
                    targetSolarSystemPosition,
                    Time.deltaTime * smoothSpeed
                );
                
                // Stop zooming when close enough
                if (Vector3.Distance(solarSystemRoot.position, targetSolarSystemPosition) < 0.1f)
                {
                    isZooming = false;
                    Debug.Log("ARPlanetZoom: Zoom complete");
                }
            }
        }
    }
    
    /// <summary>
    /// Reset camera view to original position
    /// </summary>
    public void ResetView()
    {
        if (!hasOriginalPosition)
        {
            Debug.LogWarning("ARPlanetZoom: No original position stored!");
            return;
        }
        
        targetSolarSystemPosition = originalSolarSystemPosition;
        isZooming = true;
        Debug.Log($"ARPlanetZoom: Resetting to original position: {originalSolarSystemPosition}");
    }
    
    /// <summary>
    /// Zoom to a specific planet
    /// </summary>
    public void ZoomToPlanet(string planetName)
    {
        if (arManager == null)
        {
            Debug.LogWarning("ARPlanetZoom: Missing AR Manager reference!");
            return;
        }
        
        if (planetSelector == null)
        {
            Debug.LogWarning("ARPlanetZoom: Missing Planet Selector reference!");
            return;
        }
        
        Debug.Log($"ARPlanetZoom: Attempting to zoom to '{planetName}'");
        
        // Find the planet GameObject using robust search
        GameObject planet = FindPlanetByName(planetName);
        if (planet == null)
        {
            Debug.LogError($"ARPlanetZoom: Could not find planet '{planetName}' in scene!");
            return;
        }
        
        Debug.Log($"ARPlanetZoom: Found planet '{planetName}' at position {planet.transform.position}");
        currentTarget = planet.transform;
        
        // Get camera and solar system
        Camera arCamera = arManager.GetARCamera();
        Transform solarSystemRoot = arManager.GetSolarSystemRoot();
        
        if (arCamera == null || solarSystemRoot == null || currentTarget == null)
        {
            Debug.LogWarning("ARPlanetZoom: Missing camera, solar system, or target");
            return;
        }
        
        // Calculate desired viewing distance based on planet's zoom setting
        float planetZoomDistance = GetPlanetZoomDistance(planetName);
        
        // Calculate direction from camera to planet
        Vector3 planetWorldPos = currentTarget.position;
        Vector3 cameraPos = arCamera.transform.position;
        Vector3 directionToPlanet = (planetWorldPos - cameraPos).normalized;
        
        // Calculate where solar system should be positioned so planet is at desired distance
        Vector3 desiredPlanetPosition = cameraPos + directionToPlanet * planetZoomDistance;
        Vector3 offset = desiredPlanetPosition - planetWorldPos;
        
        // Set target position for solar system
        targetSolarSystemPosition = solarSystemRoot.position + offset;
        isZooming = true;
        
        Debug.Log($"ARPlanetZoom: Zooming to {planetName} at distance {planetZoomDistance:F3}");
        Debug.Log($"  Planet world pos: {planetWorldPos}");
        Debug.Log($"  Camera pos: {cameraPos}");
        Debug.Log($"  Direction: {directionToPlanet}");
        Debug.Log($"  Target solar system pos: {targetSolarSystemPosition}");
        Debug.Log($"  Current solar system pos: {solarSystemRoot.position}");
        Debug.Log($"  Offset to move: {offset}");
    }
    
    /// <summary>
    /// Get the zoom distance for a specific planet from PlanetSelector settings
    /// </summary>
    float GetPlanetZoomDistance(string planetName)
    {
        // Default distances if not found in PlanetSelector
        // These match your PlanetSelector zoom settings
        switch (planetName)
        {
            case "Sun": return 1.25f;
            case "Mercury": return 0.015f;
            case "Venus": return 0.02f;
            case "Earth": return 0.02f;
            case "Mars": return 0.02f;
            case "Jupiter": return 0.25f;
            case "Saturn": return 0.35f;
            case "Uranus": return 0.1f;
            case "Neptune": return 0.1f;
            case "Westerwald": return 0.015f;
            case "Chimaera": return 0.015f;
            case "Rockox": return 0.015f;
            case "Moza": return 0.015f;
            case "Ousha": return 0.015f;
            case "Ghaf": return 0.015f;
            case "Justitia": return 0.015f;
            case "MBR Explorer": return 0.005f;
            case "MBR Explorer Real": return 0.2f;
            default: return 0.1f; // Default
        }
    }
    
    
    /// <summary>
    /// Find planet by name using robust search (same logic as PlanetSelector)
    /// </summary>
    GameObject FindPlanetByName(string planetName)
    {
        // First try exact name match
        GameObject planet = GameObject.Find(planetName);
        if (planet != null)
        {
            Debug.Log($"ARPlanetZoom: Found '{planetName}' by exact name");
            return planet;
        }
        
        // Try with common suffixes/prefixes
        string[] variations = {
            planetName,
            planetName + " Sphere",
            planetName + "_Planet",
            planetName + "Sphere",
            planetName.ToLower(),
            planetName.ToUpper()
        };
        
        foreach (string variation in variations)
        {
            planet = GameObject.Find(variation);
            if (planet != null)
            {
                Debug.Log($"ARPlanetZoom: Found '{planetName}' as '{variation}'");
                return planet;
            }
        }
        
        // Try finding by partial name in all GameObjects with "Celestial" tag
        GameObject[] celestials = GameObject.FindGameObjectsWithTag("Celestial");
        foreach (GameObject celestial in celestials)
        {
            if (celestial.name.ToLower().Contains(planetName.ToLower()))
            {
                Debug.Log($"ARPlanetZoom: Found '{planetName}' by Celestial tag: '{celestial.name}'");
                return celestial;
            }
        }
        
        // Last resort: check all objects
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains(planetName.ToLower()))
            {
                Debug.Log($"ARPlanetZoom: Found '{planetName}' in all objects: '{obj.name}'");
                return obj;
            }
        }
        
        return null;
    }
}

