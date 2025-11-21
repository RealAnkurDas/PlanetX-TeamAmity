using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls simulation speed by adjusting timestep multiplier
/// Works with EphemerisBasedSimulation
/// </summary>
public class TimeStepSpeedController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The simulation to control")]
    [SerializeField] private EphemerisBasedSimulation simulation;
    
    [Tooltip("Button to cycle time speed")]
    [SerializeField] private Button speedButton;
    
    [Header("Display Settings")]
    [Tooltip("Show speed display on screen")]
    [SerializeField] private bool showSpeedDisplay = true;
    
    [Header("Speed Presets")]
    [Tooltip("Timestep multipliers to cycle through (simulation seconds per real second)")]
    [SerializeField] private double[] timeStepPresets = {
        1.0,          // Real-time (1x)
        2.0,          // 2x speed
        4.0,          // 4x speed
        8.0,          // 8x speed
        16.0          // 16x speed (MAX)
    };
    
    private int currentSpeedIndex = 0;
    private string currentSpeedLabel = "";
    
    void Start()
    {
        // Auto-find simulation if not assigned
        if (simulation == null)
        {
            simulation = FindFirstObjectByType<EphemerisBasedSimulation>();
            if (simulation != null)
            {
                Debug.Log("TimeStepController: Found EphemerisBasedSimulation");
            }
            else
            {
                Debug.LogError("TimeStepController: Could not find EphemerisBasedSimulation!");
            }
        }
        
        // Setup button listener
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(CycleSpeed);
            Debug.Log("TimeStepController: Button listener added");
        }
        else
        {
            Debug.LogWarning("TimeStepController: Speed button not assigned!");
        }
        
        // Set initial speed
        SetSpeed(currentSpeedIndex);
    }
    
    public void CycleSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % timeStepPresets.Length;
        SetSpeed(currentSpeedIndex);
    }
    
    void SetSpeed(int index)
    {
        if (simulation == null) return;
        
        double newTimeStep = timeStepPresets[index];
        simulation.SetTimeStepMultiplier(newTimeStep);
        
        UpdateSpeedLabel(newTimeStep);
        Debug.Log($"TimeStepController: Set speed to index {index}, timestep: {newTimeStep:E2}");
    }
    
    void UpdateSpeedLabel(double timeStep)
    {
        // Speed labels based on preset index
        string[] speedLabels = { "1x", "2x", "4x", "8x", "16x" };
        
        // Get label for current index
        if (currentSpeedIndex < speedLabels.Length)
        {
            currentSpeedLabel = speedLabels[currentSpeedIndex];
        }
        else
        {
            currentSpeedLabel = "16x";
        }
    }
    
    void OnGUI()
    {
        if (!showSpeedDisplay) return;
        if (string.IsNullOrEmpty(currentSpeedLabel)) return;
        
        // Calculate scale factor based on screen height (reference: 800 for mobile portrait)
        // This makes UI look good on most phones and scales proportionally
        float scale = Screen.height / 800f;
        
        // Style for speed display
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = Mathf.RoundToInt(24 * scale);
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperRight;
        
        // Calculate size
        Vector2 textSize = style.CalcSize(new GUIContent(currentSpeedLabel));
        
        // Position at top-right corner
        float xPosition = Screen.width - textSize.x - (20f * scale);
        float yPosition = 20f * scale;
        
        // Draw the label
        GUI.Label(new Rect(xPosition, yPosition, textSize.x, textSize.y), currentSpeedLabel, style);
    }
    
    // Public API
    public double GetCurrentTimeStep() => timeStepPresets[currentSpeedIndex];
    public int GetCurrentSpeedIndex() => currentSpeedIndex;
}

