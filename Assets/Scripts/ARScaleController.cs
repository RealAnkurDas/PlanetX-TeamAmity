using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI controller for adjusting AR view scale
/// Allows user to scale the XR Origin to better view the solar system
/// </summary>
public class ARScaleController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The AR Solar System Manager to control")]
    [SerializeField] private ARSolarSystemManager arManager;
    
    [Header("UI Elements (Optional)")]
    [Tooltip("Slider to adjust scale")]
    [SerializeField] private Slider scaleSlider;
    
    [Tooltip("Text to display current scale")]
    [SerializeField] private TextMeshProUGUI scaleText;
    
    [Header("Scale Settings")]
    [Tooltip("Minimum scale multiplier")]
    [SerializeField] private float minScale = 100f;
    
    [Tooltip("Maximum scale multiplier")]
    [SerializeField] private float maxScale = 10000f;
    
    [Tooltip("Default scale")]
    [SerializeField] private float defaultScale = 1000f;
    
    private float currentScale;
    
    void Start()
    {
        // Auto-find AR manager if not assigned
        if (arManager == null)
        {
            arManager = FindFirstObjectByType<ARSolarSystemManager>();
        }
        
        // Setup slider if present
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultScale;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
        
        // Set initial scale
        currentScale = defaultScale;
        UpdateScaleDisplay();
    }
    
    void OnScaleChanged(float value)
    {
        currentScale = value;
        
        if (arManager != null)
        {
            arManager.SetXROriginScale(currentScale);
        }
        
        UpdateScaleDisplay();
    }
    
    void UpdateScaleDisplay()
    {
        if (scaleText != null)
        {
            scaleText.text = $"Scale: {currentScale:F0}x";
        }
    }
    
    /// <summary>
    /// Increase scale by 10%
    /// </summary>
    public void IncreaseScale()
    {
        float newScale = Mathf.Clamp(currentScale * 1.1f, minScale, maxScale);
        
        if (scaleSlider != null)
        {
            scaleSlider.value = newScale;
        }
        else
        {
            OnScaleChanged(newScale);
        }
    }
    
    /// <summary>
    /// Decrease scale by 10%
    /// </summary>
    public void DecreaseScale()
    {
        float newScale = Mathf.Clamp(currentScale * 0.9f, minScale, maxScale);
        
        if (scaleSlider != null)
        {
            scaleSlider.value = newScale;
        }
        else
        {
            OnScaleChanged(newScale);
        }
    }
    
    /// <summary>
    /// Reset to default scale
    /// </summary>
    public void ResetScale()
    {
        if (scaleSlider != null)
        {
            scaleSlider.value = defaultScale;
        }
        else
        {
            OnScaleChanged(defaultScale);
        }
    }
}

