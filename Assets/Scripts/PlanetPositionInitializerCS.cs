using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Native C# implementation using Astronomy Engine
/// No API server required - all calculations done locally
/// </summary>
public class PlanetPositionInitializerCS : MonoBehaviour
{
    [Header("Date/Time Settings")]
    [Tooltip("Use current system date and time instead of manual entry")]
    [SerializeField] private bool useCurrentDateTime = true;
    
    [Tooltip("Manual date in dd-mm-yyyy format (only used if 'Use Current DateTime' is false)")]
    [SerializeField] private string manualDate = "05-11-2025";
    
    [Tooltip("Manual time in HH:MM:SS format (only used if 'Use Current DateTime' is false)")]
    [SerializeField] private string manualTime = "20:00:00";
    
    [Header("Scale Configuration")]
    [SerializeField] private float scaleToUnityUnits = 1e-9f; // Divide by 10^9 to convert metres to billions of metres
    
    [Header("Status")]
    [SerializeField] private bool positionsLoaded = false;
    [SerializeField] private string actualDateUsed = "";
    [SerializeField] private string actualTimeUsed = "";
    
    private Dictionary<string, Vector3> planetPositions = new Dictionary<string, Vector3>();
    private PlanetPositionCalculatorCS calculator;
    private string date;
    private string time;
    
    void Awake()
    {
        // Create calculator component
        calculator = gameObject.AddComponent<PlanetPositionCalculatorCS>();
        
        // Determine which date/time to use
        if (useCurrentDateTime)
        {
            DateTime now = DateTime.UtcNow; // Use UTC for consistency
            date = now.ToString("dd-MM-yyyy");
            time = now.ToString("HH:mm:ss");
            Debug.Log($"Using current date/time (UTC): {date} {time}");
        }
        else
        {
            date = manualDate;
            time = manualTime;
            Debug.Log($"Using manual date/time: {date} {time}");
        }
        
        actualDateUsed = date;
        actualTimeUsed = time;
        
        // Calculate positions using native C# Astronomy Engine
        InitializePlanetPositions();
    }
    
    void InitializePlanetPositions()
    {
        Debug.Log("Calculating planet positions using Astronomy Engine...");
        
        try
        {
            // Get positions from calculator
            PlanetPositions positions = calculator.GetPlanetPositions(date, time);
            
            if (positions == null)
            {
                Debug.LogError("Failed to calculate planet positions!");
                return;
            }
            
            // Convert positions to Unity coordinates
            planetPositions["mercury"] = ConvertPosition(positions.mercury);
            planetPositions["venus"] = ConvertPosition(positions.venus);
            planetPositions["earth"] = ConvertPosition(positions.earth);
            planetPositions["mars"] = ConvertPosition(positions.mars);
            planetPositions["jupiter"] = ConvertPosition(positions.jupiter);
            planetPositions["saturn"] = ConvertPosition(positions.saturn);
            planetPositions["uranus"] = ConvertPosition(positions.uranus);
            planetPositions["neptune"] = ConvertPosition(positions.neptune);
            
            // Apply positions to GameObjects
            ApplyPositionsToGameObjects();
            
            positionsLoaded = true;
            Debug.Log($"Successfully loaded positions for {planetPositions.Count} planets at {date} {time}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to calculate planet positions: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    Vector3 ConvertPosition(Vector3Double pos)
    {
        // Convert from metres to Unity units (billions of metres)
        // Also remap axes: Unity uses Y-up, astronomical coords use Z-up
        
        float x = (float)(pos.x * scaleToUnityUnits);
        float y = (float)(pos.z * scaleToUnityUnits); // Z becomes Y (up axis)
        float z = (float)(pos.y * scaleToUnityUnits); // Y becomes Z
        
        return new Vector3(x, y, z);
    }
    
    void ApplyPositionsToGameObjects()
    {
        // Find all celestial objects
        GameObject[] celestials = GameObject.FindGameObjectsWithTag("Celestial");
        
        foreach (GameObject celestial in celestials)
        {
            string objectName = celestial.name.ToLower();
            
            // Try to match by name (handles variations like "Earth", "earth", "Earth_Planet", etc.)
            foreach (var kvp in planetPositions)
            {
                if (objectName.Contains(kvp.Key))
                {
                    celestial.transform.position = kvp.Value;
                    Debug.Log($"Set {celestial.name} position to {kvp.Value} (scaled from calculation)");
                    break;
                }
            }
        }
        
        // Special handling for the Sun (should be at origin for heliocentric coords)
        GameObject sun = GameObject.Find("Sun Sphere");
        if (sun != null)
        {
            sun.transform.position = Vector3.zero;
            Debug.Log("Set Sun Sphere position to origin (0, 0, 0)");
        }
        else
        {
            Debug.LogWarning("Sun Sphere GameObject not found! Heliocentric reference may be incorrect.");
        }
    }
    
    /// <summary>
    /// Public accessor for actual date used (for SimulationTimeTracker)
    /// </summary>
    public string GetActualDate() => actualDateUsed;
    
    /// <summary>
    /// Public accessor for actual time used (for SimulationTimeTracker)
    /// </summary>
    public string GetActualTime() => actualTimeUsed;
}

