using UnityEngine;
using System;

public class SimulationTimeTracker : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Earth GameObject (must have 'earth' in its name or set manually)")]
    [SerializeField] private Transform earthTransform;
    
    [Tooltip("The Sun GameObject (searches for 'Sun Sphere' by default)")]
    [SerializeField] private Transform sunTransform;
    
    [Header("Starting Date/Time")]
    [Tooltip("Use current system date and time as simulation start")]
    [SerializeField] private bool useCurrentDateTime = true;
    
    [Tooltip("Manual start date in dd-mm-yyyy format (only used if 'Use Current DateTime' is false)")]
    [SerializeField] private string manualStartDate = "05-11-2025";
    
    [Tooltip("Manual start time in HH:MM:SS format (only used if 'Use Current DateTime' is false)")]
    [SerializeField] private string manualStartTime = "20:00:00";
    
    private string startDate; // Actual date used (either current or manual)
    private string startTime; // Actual time used (either current or manual)
    
    [Header("Current Simulation Time")]
    [SerializeField] private float daysElapsed = 0f;
    [SerializeField] private float yearsElapsed = 0f;
    [SerializeField] private float orbitalProgress = 0f; // 0 to 1 (percentage of orbit completed)
    [SerializeField] private string currentDate = "";
    [SerializeField] private string currentDateTime = "";
    
    [Header("Settings")]
    [Tooltip("Choose the axis perpendicular to the orbital plane (Y-up for horizontal orbits, adjust if needed)")]
    [SerializeField] private Vector3 orbitalPlaneNormal = Vector3.up;
    
    [Tooltip("Wait time before initializing tracking (allows positions to load)")]
    [SerializeField] private float initializationDelay = 2.5f;
    
    [Header("Display")]
    [Tooltip("The Orbit script to get G value from")]
    [SerializeField] private Orbit orbitScript;
    
    private float initialGValue;
    
    private Vector3 startingEarthDirection;
    private Vector3 lastEarthDirection;
    private float totalAngleRotated = 0f; // In degrees
    private bool isTracking = false;
    
    // Earth's orbital period (real days)
    private const float EARTH_ORBITAL_PERIOD = 365.25f;
    
    // Starting DateTime
    private DateTime startDateTime;
    
    void Start()
    {
        // Auto-find Earth and Sun if not set
        if (earthTransform == null)
        {
            GameObject earth = GameObject.Find("Earth");
            if (earth == null)
            {
                // Try finding any object with "earth" in the name
                GameObject[] celestials = GameObject.FindGameObjectsWithTag("Celestial");
                foreach (GameObject obj in celestials)
                {
                    if (obj.name.ToLower().Contains("earth"))
                    {
                        earth = obj;
                        break;
                    }
                }
            }
            
            if (earth != null)
            {
                earthTransform = earth.transform;
                Debug.Log($"Time Tracker: Found Earth - {earth.name}");
            }
            else
            {
                Debug.LogError("Time Tracker: Could not find Earth! Please assign it manually in Inspector.");
            }
        }
        
        if (sunTransform == null)
        {
            GameObject sun = GameObject.Find("Sun Sphere");
            if (sun == null)
            {
                sun = GameObject.Find("Sun");
            }
            
            if (sun != null)
            {
                sunTransform = sun.transform;
                Debug.Log($"Time Tracker: Found Sun - {sun.name}");
            }
            else
            {
                Debug.LogError("Time Tracker: Could not find Sun! Please assign it manually in Inspector.");
            }
        }
        
        // Determine which date/time to use
        if (useCurrentDateTime)
        {
            DateTime now = DateTime.Now;
            startDate = now.ToString("dd-MM-yyyy");
            startTime = now.ToString("HH:mm:ss");
            startDateTime = now;
            Debug.Log($"Time Tracker: Using current date/time: {startDate} {startTime}");
        }
        else
        {
            startDate = manualStartDate;
            startTime = manualStartTime;
            Debug.Log($"Time Tracker: Using manual date/time: {startDate} {startTime}");
            
            // Parse manual date and time
            try
            {
                string[] dateParts = startDate.Split('-');
                string[] timeParts = startTime.Split(':');
                
                int day = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[2]);
                
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                int second = int.Parse(timeParts[2]);
                
                startDateTime = new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception e)
            {
                Debug.LogError($"Time Tracker: Failed to parse manual date/time: {e.Message}");
                startDateTime = DateTime.Now;
                startDate = startDateTime.ToString("dd-MM-yyyy");
                startTime = startDateTime.ToString("HH:mm:ss");
            }
        }
        
        currentDate = startDate;
        currentDateTime = startDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        Debug.Log($"Time Tracker: Starting from {currentDateTime}");
        
        // Get initial G value from Orbit script
        if (orbitScript == null)
        {
            orbitScript = FindFirstObjectByType<Orbit>();
        }
        
        if (orbitScript != null)
        {
            initialGValue = orbitScript.G;
            Debug.Log($"Time Tracker: Initial G value = {initialGValue:E2}");
        }
        else
        {
            Debug.LogWarning("Time Tracker: Could not find Orbit script. G value will not be displayed.");
            initialGValue = 0f;
        }
        
        // Initialize tracking after delay (to allow positions to be set)
        if (earthTransform != null && sunTransform != null)
        {
            Invoke(nameof(InitializeTracking), initializationDelay);
        }
    }
    
    void InitializeTracking()
    {
        if (earthTransform == null || sunTransform == null)
        {
            Debug.LogError("Time Tracker: Cannot initialize - Earth or Sun not found!");
            return;
        }
        
        // Store Earth's starting direction relative to Sun (normalized)
        startingEarthDirection = (earthTransform.position - sunTransform.position).normalized;
        lastEarthDirection = startingEarthDirection;
        
        isTracking = true;
        
        Debug.Log($"Time Tracker: Initialized! Tracking Earth's orbit around Sun.");
        Debug.Log($"  Start position: {startingEarthDirection}");
        Debug.Log($"  Orbital plane normal: {orbitalPlaneNormal}");
    }
    
    void Update()
    {
        if (!isTracking || earthTransform == null || sunTransform == null)
            return;
        
        // Calculate Earth's current direction relative to Sun (normalized)
        Vector3 currentEarthDirection = (earthTransform.position - sunTransform.position).normalized;
        
        // Calculate angle rotated this frame
        float angleThisFrame = Vector3.SignedAngle(
            lastEarthDirection,
            currentEarthDirection,
            orbitalPlaneNormal
        );
        
        // Negate angle to correct for coordinate system conversion (API to Unity axes)
        angleThisFrame = -angleThisFrame;
        
        // Accumulate total rotation
        totalAngleRotated += angleThisFrame;
        
        // Calculate days elapsed based on orbital progress
        // 360 degrees = 365.25 days
        daysElapsed = (totalAngleRotated / 360f) * EARTH_ORBITAL_PERIOD;
        yearsElapsed = daysElapsed / 365.25f;
        
        // Calculate orbital progress (0 to 1)
        orbitalProgress = (totalAngleRotated % 360f) / 360f;
        if (orbitalProgress < 0) orbitalProgress += 1f; // Handle negative values
        
        // Update current date and time
        UpdateCurrentDateTime();
        
        // Store direction for next frame
        lastEarthDirection = currentEarthDirection;
    }
    
    void UpdateCurrentDateTime()
    {
        try
        {
            // Add elapsed days to starting DateTime
            DateTime currentDT = startDateTime.AddDays(daysElapsed);
            
            // Format for display
            currentDate = currentDT.ToString("dd-MM-yyyy");
            currentDateTime = currentDT.ToString("dd-MM-yyyy HH:mm:ss");
        }
        catch (Exception e)
        {
            Debug.LogError($"Time Tracker: Error updating date: {e.Message}");
        }
    }
    
    // ========== PUBLIC API ==========
    
    /// <summary>
    /// Get the number of days elapsed since simulation start
    /// </summary>
    public float GetDaysElapsed() => daysElapsed;
    
    /// <summary>
    /// Get the number of years elapsed since simulation start
    /// </summary>
    public float GetYearsElapsed() => yearsElapsed;
    
    /// <summary>
    /// Get the current date as a string (dd-MM-yyyy)
    /// </summary>
    public string GetCurrentDate() => currentDate;
    
    /// <summary>
    /// Get the current date and time as a string (dd-MM-yyyy HH:mm:ss)
    /// </summary>
    public string GetCurrentDateTime() => currentDateTime;
    
    /// <summary>
    /// Get Earth's orbital progress (0.0 to 1.0, where 1.0 = complete orbit)
    /// </summary>
    public float GetOrbitalProgress() => orbitalProgress;
    
    /// <summary>
    /// Get total angle Earth has rotated around Sun (in degrees)
    /// </summary>
    public float GetTotalAngleRotated() => totalAngleRotated;
    
    /// <summary>
    /// Get number of complete orbits Earth has made
    /// </summary>
    public int GetCompleteOrbits() => Mathf.FloorToInt(totalAngleRotated / 360f);
    
    /// <summary>
    /// Get the current DateTime object
    /// </summary>
    public DateTime GetDateTime()
    {
        return startDateTime.AddDays(daysElapsed);
    }
    
    /// <summary>
    /// Reset the time tracker (useful for restarting simulation)
    /// </summary>
    public void ResetTracking()
    {
        totalAngleRotated = 0f;
        daysElapsed = 0f;
        yearsElapsed = 0f;
        orbitalProgress = 0f;
        
        // Re-determine start date/time (in case useCurrentDateTime changed or time needs updating)
        if (useCurrentDateTime)
        {
            DateTime now = DateTime.Now;
            startDate = now.ToString("dd-MM-yyyy");
            startTime = now.ToString("HH:mm:ss");
            startDateTime = now;
        }
        else
        {
            startDate = manualStartDate;
            startTime = manualStartTime;
            // Re-parse manual date/time
            try
            {
                string[] dateParts = startDate.Split('-');
                string[] timeParts = startTime.Split(':');
                int day = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[2]);
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                int second = int.Parse(timeParts[2]);
                startDateTime = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                startDateTime = DateTime.Now;
            }
        }
        
        currentDate = startDate;
        currentDateTime = startDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        
        if (earthTransform != null && sunTransform != null)
        {
            InitializeTracking();
        }
    }
    
    // ========== GUI DISPLAY ==========
    
    void OnGUI()
    {
        if (!isTracking) return;
        
        // Style for date and time display
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        
        // Create display text with date, time, and G value
        string timeString = currentDateTime.Split(' ')[1]; // Extract HH:mm:ss
        string displayText = $"Date: {currentDate}\nTime: {timeString}\nG: {initialGValue:E2}";
        
        // Calculate size for centered text
        Vector2 textSize = style.CalcSize(new GUIContent(displayText));
        
        // Position at bottom center of screen
        float xPosition = (Screen.width - textSize.x) / 2f;
        float yPosition = Screen.height - textSize.y - 30f; // 30px from bottom
        
        // Draw the label
        GUI.Label(new Rect(xPosition, yPosition, textSize.x, textSize.y), displayText, style);
    }
}

