using UnityEngine;
using System;

/// <summary>
/// Time tracker for EphemerisBasedSimulation
/// Displays current simulation date and time
/// </summary>
public class SimulationTimeTrackerNew : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The EphemerisBasedSimulation to track")]
    [SerializeField] private EphemerisBasedSimulation simulation;
    
    [Tooltip("Optional: Orbit script to display G value")]
    [SerializeField] private Orbit orbitScript;
    
    [Header("Display Settings")]
    [Tooltip("Show date/time display")]
    [SerializeField] private bool showDisplay = true;
    
    private bool isARMode = false;
    
    [Tooltip("Show timestep multiplier instead of G")]
    [SerializeField] private bool showTimeStep = true;
    
    [Header("Current Time")]
    [SerializeField] private string currentDate = "";
    [SerializeField] private string currentTime = "";
    [SerializeField] private double currentTimeStepMultiplier = 0;
    
    void Start()
    {
        // Check if in AR mode
        isARMode = (FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>() != null);
        
        // Auto-find simulation if not assigned
        if (simulation == null)
        {
            simulation = FindFirstObjectByType<EphemerisBasedSimulation>();
            if (simulation != null)
            {
                Debug.Log("TimeTracker: Found EphemerisBasedSimulation");
            }
            else
            {
                Debug.LogError("TimeTracker: Could not find EphemerisBasedSimulation!");
            }
        }
    }
    
    void Update()
    {
        if (simulation == null) return;
        
        // Get DateTime from simulation
        DateTime simDateTime = simulation.GetCurrentDateTime();
        
        // Format date as "12 MAR 2026"
        currentDate = simDateTime.ToString("dd MMM yyyy").ToUpper();
        
        // Format time as 12-hour with AM/PM
        currentTime = simDateTime.ToString("hh:mm:ss tt");
    }
    
    void OnGUI()
    {
        if (!showDisplay || simulation == null) return;
        
        // Calculate scale factor based on screen height (reference: 800 for mobile portrait)
        // This makes UI look good on most phones and scales proportionally
        float scale = Screen.height / 800f;
        
        // Style for display
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = Mathf.RoundToInt(18 * scale);
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        
        // Create display text with date and time
        string displayText = $"{currentDate}\n{currentTime}";
        
        // Calculate size for centered text
        Vector2 textSize = style.CalcSize(new GUIContent(displayText));
        
        // Position at bottom center of screen (moved up from bottom)
        float xPosition = (Screen.width - textSize.x) / 2f;
        float yPosition = Screen.height - textSize.y - (80f * scale); // Scale the offset
        
        // Draw the label
        GUI.Label(new Rect(xPosition, yPosition, textSize.x, textSize.y), displayText, style);
    }
    
    // Public API
    public string GetDate() => currentDate;
    public string GetTime() => currentTime;
}

