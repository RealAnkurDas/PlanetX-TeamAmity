using UnityEngine;
using System;

[Serializable]
public class PlanetZoomSetting
{
    public string planetName;
    public float zoomDistance = 300f;
    public float fieldOfView = -1f; // -1 means use default FOV
}

public class PlanetSelector : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera controller to update")]
    [SerializeField] private TouchCameraController cameraController;
    
    [Header("Display Settings")]
    [Tooltip("Position from left edge of screen")]
    [SerializeField] private float menuLeftOffset = 20f;
    
    [Tooltip("Position from top edge of screen")]
    [SerializeField] private float menuTopOffset = 20f;
    
    [Header("Planet Zoom Settings")]
    [Tooltip("Zoom distance for each planet - customize in Inspector")]
    [SerializeField] private PlanetZoomSetting[] planetZoomSettings = new PlanetZoomSetting[]
    {
        new PlanetZoomSetting { planetName = "Sun", zoomDistance = 1.25f },
        new PlanetZoomSetting { planetName = "Mercury", zoomDistance = 0.015f },
        new PlanetZoomSetting { planetName = "Venus", zoomDistance = 0.02f },
        new PlanetZoomSetting { planetName = "Earth", zoomDistance = 0.02f },
        new PlanetZoomSetting { planetName = "Mars", zoomDistance = 0.02f },
        new PlanetZoomSetting { planetName = "Jupiter", zoomDistance = 0.25f },
        new PlanetZoomSetting { planetName = "Saturn", zoomDistance = 0.35f },
        new PlanetZoomSetting { planetName = "Uranus", zoomDistance = 0.1f },
        new PlanetZoomSetting { planetName = "Neptune", zoomDistance = 0.1f },
        new PlanetZoomSetting { planetName = "Westerwald", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Chimaera", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Rockox", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Moza", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Ousha", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Ghaf", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "Justitia", zoomDistance = 0.015f, fieldOfView = -1f },
        new PlanetZoomSetting { planetName = "MBR Explorer", zoomDistance = 0.005f, fieldOfView = 105f }
    };
    
    private string currentAnchor = "Sun";
    private bool isDropdownOpen = false;
    private bool isARMode = false;
    
    void Start()
    {
        // Check if in AR mode
        isARMode = (FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>() != null);
        
        if (isARMode)
        {
            Debug.Log("PlanetSelector: AR Mode detected - Dropdown UI will be hidden");
        }
        else
        {
            Debug.Log("PlanetSelector: Regular mode - Dropdown UI enabled");
        }
        
        // Auto-find camera controller if not assigned
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<TouchCameraController>();
            if (cameraController != null)
            {
                Debug.Log("PlanetSelector: Found TouchCameraController");
            }
            else
            {
                if (!isARMode)
                {
                    Debug.LogWarning("PlanetSelector: Could not find TouchCameraController!");
                }
            }
        }
        
        // Set initial minimum zoom distance for default anchor (Sun)
        if (cameraController != null)
        {
            PlanetZoomSetting initialSetting = GetSettingForPlanet(currentAnchor);
            if (initialSetting != null)
            {
                cameraController.SetMinimumZoomDistance(initialSetting.zoomDistance);
                Debug.Log($"PlanetSelector: Set initial min zoom for {currentAnchor} to {initialSetting.zoomDistance}");
            }
        }
    }
    
    void OnGUI()
    {
        // Hide dropdown UI in AR mode
        if (isARMode)
        {
            return;
        }
        
        // Calculate scale factor based on screen height (reference: 800 for mobile portrait)
        // This makes UI look good on most phones and scales proportionally
        float scale = Screen.height / 800f;
        
        // Dropdown button style
        GUIStyle dropdownButtonStyle = new GUIStyle(GUI.skin.button);
        dropdownButtonStyle.fontSize = Mathf.RoundToInt(16 * scale); // Increased from 12
        dropdownButtonStyle.fontStyle = FontStyle.Bold;
        dropdownButtonStyle.alignment = TextAnchor.MiddleLeft;
        dropdownButtonStyle.normal.textColor = Color.cyan;
        
        float buttonWidth = 140f * scale; // Increased from 100
        float buttonHeight = 35f * scale; // Increased from 25
        float scaledLeftOffset = menuLeftOffset * scale;
        float scaledTopOffset = menuTopOffset * scale;
        
        // Draw the dropdown toggle button showing current anchor
        string dropdownText = isDropdownOpen ? $"▼ {currentAnchor}" : $"▶ {currentAnchor}";
        
        if (GUI.Button(new Rect(scaledLeftOffset, scaledTopOffset, buttonWidth, buttonHeight), dropdownText, dropdownButtonStyle))
        {
            // Toggle dropdown open/close
            isDropdownOpen = !isDropdownOpen;
        }
        
        // If dropdown is open, show planet list
        if (isDropdownOpen)
        {
            // Style for planet option buttons
            GUIStyle optionStyle = new GUIStyle(GUI.skin.button);
            optionStyle.fontSize = Mathf.RoundToInt(14 * scale); // Increased from 11
            optionStyle.fontStyle = FontStyle.Normal;
            optionStyle.alignment = TextAnchor.MiddleLeft;
            optionStyle.normal.textColor = Color.white;
            optionStyle.hover.textColor = Color.yellow;
            
            float yPosition = scaledTopOffset + buttonHeight + (2f * scale);
            float optionHeight = 28f * scale; // Increased from 20
            float optionWidth = buttonWidth;
            
            // Draw planet option buttons from zoom settings
            foreach (PlanetZoomSetting setting in planetZoomSettings)
            {
                // Skip the current anchor (it's already shown in main button)
                if (setting.planetName == currentAnchor) continue;
                
                if (GUI.Button(new Rect(scaledLeftOffset, yPosition, optionWidth, optionHeight), setting.planetName, optionStyle))
                {
                    SetCameraAnchor(setting.planetName);
                    isDropdownOpen = false; // Close dropdown after selection
                }
                
                yPosition += optionHeight + (2f * scale);
            }
        }
    }
    
    void SetCameraAnchor(string planetName)
    {
        // Check if we're in AR mode (no TouchCameraController in AR)
        bool isARMode = (cameraController == null);
        
        if (isARMode)
        {
            // AR mode: Just update the current anchor
            // ARPlanetZoom script will detect the change and handle zooming
            currentAnchor = planetName;
            Debug.Log($"PlanetSelector (AR Mode): Selected '{planetName}' - ARPlanetZoom will handle zoom");
            return;
        }
        
        // Regular mode: Use TouchCameraController
        if (cameraController == null)
        {
            Debug.LogError("PlanetSelector: Camera controller not assigned!");
            return;
        }
        
        // Find the planet GameObject
        GameObject planet = FindPlanetByName(planetName);
        
        if (planet != null)
        {
            currentAnchor = planetName;
            
            // Get settings for this object
            PlanetZoomSetting setting = GetSettingForPlanet(planetName);
            
            // Set target and zoom
            cameraController.SetTarget(planet.transform);
            
            if (setting != null)
            {
                // Set minimum zoom distance (prevents zooming past the target)
                cameraController.SetMinimumZoomDistance(setting.zoomDistance);
                
                // Set zoom distance
                cameraController.SetZoomDistance(setting.zoomDistance);
                
                // Set FOV if specified (not -1)
                if (setting.fieldOfView > 0)
                {
                    cameraController.SetFieldOfView(setting.fieldOfView);
                    Debug.Log($"PlanetSelector: Camera anchor changed to {planetName}, zoom: {setting.zoomDistance}, FOV: {setting.fieldOfView}");
                }
                else
                {
                    // Reset to default FOV
                    cameraController.ResetFieldOfView();
                    Debug.Log($"PlanetSelector: Camera anchor changed to {planetName}, zoom: {setting.zoomDistance}");
                }
            }
            else
            {
                // Fallback: auto-calculate zoom
                float zoomDistance = GetZoomDistanceForPlanet(planetName, planet);
                cameraController.SetMinimumZoomDistance(zoomDistance);
                cameraController.SetZoomDistance(zoomDistance);
                cameraController.ResetFieldOfView();
                Debug.Log($"PlanetSelector: Camera anchor changed to {planetName}, zoom: {zoomDistance}");
            }
        }
        else
        {
            Debug.LogWarning($"PlanetSelector: Could not find planet '{planetName}'");
        }
    }
    
    PlanetZoomSetting GetSettingForPlanet(string planetName)
    {
        // Look up settings from array
        foreach (PlanetZoomSetting setting in planetZoomSettings)
        {
            if (setting.planetName.Equals(planetName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"PlanetSelector: Found setting for {planetName} - Zoom: {setting.zoomDistance}, FOV: {setting.fieldOfView}");
                return setting;
            }
        }
        return null;
    }
    
    float GetZoomDistanceForPlanet(string planetName, GameObject planet)
    {
        // Look up zoom distance from settings array
        foreach (PlanetZoomSetting setting in planetZoomSettings)
        {
            if (setting.planetName.Equals(planetName, StringComparison.OrdinalIgnoreCase))
            {
                return setting.zoomDistance;
            }
        }
        
        // Default if not found in settings
        Debug.LogWarning($"PlanetSelector: No zoom setting found for '{planetName}', using default 300");
        return 300f;
    }
    
    GameObject FindPlanetByName(string planetName)
    {
        // First try exact name match
        GameObject planet = GameObject.Find(planetName);
        if (planet != null) return planet;
        
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
            if (planet != null) return planet;
        }
        
        // Try finding by partial name in all GameObjects with "Celestial" tag
        GameObject[] celestials = GameObject.FindGameObjectsWithTag("Celestial");
        foreach (GameObject celestial in celestials)
        {
            if (celestial.name.ToLower().Contains(planetName.ToLower()))
            {
                return celestial;
            }
        }
        
        // Also check objects without the tag
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains(planetName.ToLower()))
            {
                return obj;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Public method to set anchor from other scripts
    /// </summary>
    public void SetAnchor(string planetName)
    {
        SetCameraAnchor(planetName);
    }
    
    /// <summary>
    /// Get current anchor name
    /// </summary>
    public string GetCurrentAnchor()
    {
        return currentAnchor;
    }
}

