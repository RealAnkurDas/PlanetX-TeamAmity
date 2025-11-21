using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple UI for AR scale controls
/// Creates buttons dynamically or uses existing ones
/// </summary>
public class ARScaleUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AR Solar System Manager to control")]
    [SerializeField] private ARSolarSystemManager arManager;
    
    [Tooltip("AR Planet Zoom controller for zooming to selected planets")]
    [SerializeField] private ARPlanetZoom planetZoom;
    
    [Tooltip("Planet Selector to show current selection")]
    [SerializeField] private PlanetSelector planetSelector;
    
    [Header("Manual Button References")]
    [Tooltip("Reset button to return to original view")]
    [SerializeField] private Button resetButton;
    
    [Header("Display")]
    [Tooltip("Text to show current scale (optional)")]
    [SerializeField] private TextMeshProUGUI scaleText;
    
    [Header("Scale Settings")]
    [SerializeField] private float minScale = 100f;
    [SerializeField] private float maxScale = 10000f;
    [SerializeField] private float scaleStep = 1.2f; // 20% change per click
    
    private float currentScale = 1000f;
    
    void Start()
    {
        // Auto-find AR manager if not assigned
        if (arManager == null)
        {
            arManager = FindFirstObjectByType<ARSolarSystemManager>();
            if (arManager != null)
            {
                currentScale = arManager.GetXROriginScale();
                Debug.Log($"ARScaleUI: Found AR Manager, current scale: {currentScale}");
            }
        }
        
        // Auto-find planet zoom if not assigned
        if (planetZoom == null)
        {
            planetZoom = FindFirstObjectByType<ARPlanetZoom>();
            if (planetZoom != null)
            {
                Debug.Log("ARScaleUI: Found AR Planet Zoom");
            }
        }
        
        // Auto-find planet selector if not assigned
        if (planetSelector == null)
        {
            planetSelector = FindFirstObjectByType<PlanetSelector>();
            if (planetSelector != null)
            {
                Debug.Log("ARScaleUI: Found Planet Selector");
            }
        }
        
        // Setup reset button listener
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetView);
            Debug.Log("ARScaleUI: Reset button connected");
        }
        else
        {
            Debug.LogWarning("ARScaleUI: Reset button not assigned! Please assign in Inspector.");
        }
        
        UpdateDisplay();
    }
    
    /// <summary>
    /// Reset view to original camera position
    /// </summary>
    public void ResetView()
    {
        if (planetZoom != null)
        {
            planetZoom.ResetView();
            Debug.Log("ARScaleUI: Resetting to original view");
        }
        else
        {
            Debug.LogWarning("ARScaleUI: Planet Zoom not assigned! Cannot reset view.");
        }
    }
    
    void ApplyScale()
    {
        if (arManager != null)
        {
            arManager.SetXROriginScale(currentScale);
        }
        
        UpdateDisplay();
    }
    
    void UpdateDisplay()
    {
        if (scaleText != null && planetSelector != null)
        {
            string currentPlanet = planetSelector.GetCurrentAnchor();
            scaleText.text = $"Viewing: {currentPlanet}";
        }
    }
    
    /// <summary>
    /// Get current scale value
    /// </summary>
    public float GetCurrentScale()
    {
        return currentScale;
    }
    
    /// <summary>
    /// Display AR info on screen using OnGUI (optional, can be disabled)
    /// </summary>
    void OnGUI()
    {
        // Disabled - no UI needed in AR mode
        // User just looks at the solar system in AR, no controls
        return;
    }
}

