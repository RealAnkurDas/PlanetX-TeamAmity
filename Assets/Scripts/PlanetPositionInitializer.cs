using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlanetPositionInitializer : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private string apiBaseUrl = "http://localhost:8000";
    
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
    private string date;
    private string time;
    
    void Awake()
    {
        // Determine which date/time to use
        if (useCurrentDateTime)
        {
            DateTime now = DateTime.Now;
            date = now.ToString("dd-MM-yyyy");
            time = now.ToString("HH:mm:ss");
            Debug.Log($"Using current date/time: {date} {time}");
        }
        else
        {
            date = manualDate;
            time = manualTime;
            Debug.Log($"Using manual date/time: {date} {time}");
        }
        
        actualDateUsed = date;
        actualTimeUsed = time;
        
        // This runs before Start(), ensuring positions are set before Orbit.Start()
        StartCoroutine(InitializePlanetPositions());
    }
    
    IEnumerator InitializePlanetPositions()
    {
        Debug.Log("Fetching planet positions from API...");
        
        // Construct API URL
        string url = $"{apiBaseUrl}/get_coords?date={date}&time={time}";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send request
            yield return request.SendWebRequest();
            
            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to fetch planet positions: {request.error}");
                Debug.LogError($"URL: {url}");
                Debug.LogError("Make sure the Python API server is running on http://localhost:8000");
                yield break;
            }
            
            // Parse JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Received response: {jsonResponse.Substring(0, Mathf.Min(200, jsonResponse.Length))}...");
            
            try
            {
                PlanetAPIResponse response = JsonUtility.FromJson<PlanetAPIResponse>(jsonResponse);
                
                // Convert positions
                planetPositions["mercury"] = ConvertPosition(response.planets.mercury);
                planetPositions["venus"] = ConvertPosition(response.planets.venus);
                planetPositions["earth"] = ConvertPosition(response.planets.earth);
                planetPositions["mars"] = ConvertPosition(response.planets.mars);
                planetPositions["jupiter"] = ConvertPosition(response.planets.jupiter);
                planetPositions["saturn"] = ConvertPosition(response.planets.saturn);
                planetPositions["uranus"] = ConvertPosition(response.planets.uranus);
                planetPositions["neptune"] = ConvertPosition(response.planets.neptune);
                
                // Apply positions to GameObjects
                ApplyPositionsToGameObjects();
                
                positionsLoaded = true;
                Debug.Log($"Successfully loaded positions for {planetPositions.Count} planets at {date} {time}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse API response: {e.Message}");
                Debug.LogError($"Response: {jsonResponse}");
            }
        }
    }
    
    Vector3 ConvertPosition(PlanetPosition pos)
    {
        // Convert from metres to Unity units (billions of metres)
        // Also note: Unity uses Y-up, but astronomical coords use Z-up
        // You may need to adjust axes depending on your scene setup
        
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
                    Debug.Log($"Set {celestial.name} position to {kvp.Value} (scaled from API)");
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
    
    // JSON data structures for parsing API response
    [Serializable]
    public class PlanetAPIResponse
    {
        public string status;
        public string date;
        public string time;
        public PlanetsData planets;
    }
    
    [Serializable]
    public class PlanetsData
    {
        public PlanetPosition mercury;
        public PlanetPosition venus;
        public PlanetPosition earth;
        public PlanetPosition mars;
        public PlanetPosition jupiter;
        public PlanetPosition saturn;
        public PlanetPosition uranus;
        public PlanetPosition neptune;
    }
    
    [Serializable]
    public class PlanetPosition
    {
        public double x;
        public double y;
        public double z;
    }
}