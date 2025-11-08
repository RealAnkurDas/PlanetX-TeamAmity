using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TimeSpeedController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Orbit script that controls gravity")]
    [SerializeField] private Orbit orbitScript;
    
    [Tooltip("Button to cycle time speed")]
    [SerializeField] private Button speedButton;
    
    [Header("Display Settings")]
    [Tooltip("Show speed display on screen")]
    [SerializeField] private bool showSpeedDisplay = true;
    
    [Header("Speed Settings")]
    [Tooltip("G values to cycle through")]
    [SerializeField] private float[] gValues = { 2f, 4f, 8f, 16f, 32f };
    
    private int currentSpeedIndex = 0;
    private const float REAL_TIME_G = 4e-13f; // True real-time G with your mass scale (Sun=333,000)
    private string currentSpeedLabel = ""; // Start with no label
    
    void Start()
    {
        // Auto-find Orbit script if not assigned
        if (orbitScript == null)
        {
            orbitScript = FindFirstObjectByType<Orbit>();
            if (orbitScript != null)
            {
                Debug.Log("TimeSpeedController: Found Orbit script");
            }
            else
            {
                Debug.LogError("TimeSpeedController: Could not find Orbit script!");
            }
        }
        
        // Setup button listener
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(CycleSpeed);
            Debug.Log("TimeSpeedController: Button listener added");
        }
        else
        {
            Debug.LogWarning("TimeSpeedController: Speed button not assigned! Please assign in Inspector.");
        }
        
        // Set initial speed
        SetSpeed(currentSpeedIndex);
        
        Debug.Log("TimeSpeedController: Initialized. Click the Speed button to cycle through speeds.");
    }
    
    /// <summary>
    /// Cycle to the next speed setting
    /// </summary>
    public void CycleSpeed()
    {
        // Move to next speed
        currentSpeedIndex = (currentSpeedIndex + 1) % gValues.Length;
        SetSpeed(currentSpeedIndex);
        
        Debug.Log($"TimeSpeedController: Cycled to speed index {currentSpeedIndex}");
    }
    
    /// <summary>
    /// Set speed by index
    /// </summary>
    void SetSpeed(int index)
    {
        if (orbitScript == null)
        {
            Debug.LogError("TimeSpeedController: No Orbit script assigned!");
            return;
        }
        
        float newG = gValues[index];
        
        // Update G value (now public)
        orbitScript.G = newG;
        
        Debug.Log($"TimeSpeedController: Set G to {newG:E2}");
        
        // Update display text
        UpdateSpeedText(newG);
    }
    
    /// <summary>
    /// Update the speed display text
    /// </summary>
    void UpdateSpeedText(float currentG)
    {
        // Speed labels: first is empty (no text), then show multipliers
        string[] speedLabels = { "", "2x", "4x", "8x", "16x" };
        
        // Get label for current index
        if (currentSpeedIndex < speedLabels.Length)
        {
            currentSpeedLabel = speedLabels[currentSpeedIndex];
        }
        else
        {
            currentSpeedLabel = "16x"; // Cap at highest
        }
    }
    
    /// <summary>
    /// Draw speed display on screen using OnGUI
    /// </summary>
    void OnGUI()
    {
        if (!showSpeedDisplay) return;
        
        // Only show if there's a label (not empty string)
        if (string.IsNullOrEmpty(currentSpeedLabel)) return;
        
        // Style for speed display
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperRight;
        
        // Display text - just show the multiplier
        string displayText = currentSpeedLabel;
        
        // Calculate size
        Vector2 textSize = style.CalcSize(new GUIContent(displayText));
        
        // Position at top-right corner
        float xPosition = Screen.width - textSize.x - 20f;
        float yPosition = 20f;
        
        // Draw the label
        GUI.Label(new Rect(xPosition, yPosition, textSize.x, textSize.y), displayText, style);
    }
    
    /// <summary>
    /// Set speed by index from external scripts
    /// </summary>
    public void SetSpeedByIndex(int index)
    {
        if (index >= 0 && index < gValues.Length)
        {
            currentSpeedIndex = index;
            SetSpeed(currentSpeedIndex);
        }
    }
    
    /// <summary>
    /// Get current speed multiplier
    /// </summary>
    public float GetCurrentSpeedMultiplier()
    {
        return gValues[currentSpeedIndex] / REAL_TIME_G;
    }
    
    /// <summary>
    /// Get current G value
    /// </summary>
    public float GetCurrentG()
    {
        return gValues[currentSpeedIndex];
    }
}

