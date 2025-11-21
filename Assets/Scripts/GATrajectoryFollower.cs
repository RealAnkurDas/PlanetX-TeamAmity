using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

/// <summary>
/// Reads GA-optimized trajectory from CSV and positions spacecraft based on simulation date
/// Works on both Editor and Android builds using StreamingAssets
/// </summary>
public class GATrajectoryFollower : MonoBehaviour
{
    [Header("Trajectory Data")]
    [Tooltip("CSV filename in StreamingAssets/Trajectories/ folder")]
    [SerializeField] private string csvFileName = "trajectory_justitia_output.csv";
    
    [Header("Simulation Reference")]
    [Tooltip("Drag the SimulationManager GameObject here to sync with simulation time")]
    [SerializeField] private EphemerisBasedSimulation simulationManager;
    
    [Header("Scale Settings")]
    [Tooltip("Unity scale: 1 unit = 1 billion meters")]
    [SerializeField] private float unityScale = 1e9f; // 1 billion meters per Unity unit
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool drawGizmos = true;
    
    private List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
    private const double AU_TO_METERS = 149597870700.0; // 1 AU in meters
    
    [System.Serializable]
    private class TrajectoryPoint
    {
        public DateTime dateTime;
        public Vector3 position; // In Unity units (billions of meters)
        
        public TrajectoryPoint(DateTime dt, Vector3 pos)
        {
            dateTime = dt;
            position = pos;
        }
    }
    
    void Start()
    {
        StartCoroutine(LoadTrajectoryCoroutine());
        
        if (simulationManager == null)
        {
            simulationManager = FindFirstObjectByType<EphemerisBasedSimulation>();
            if (simulationManager != null)
            {
                Debug.Log("GATrajectoryFollower: Auto-found SimulationManager");
            }
            else
            {
                Debug.LogError("GATrajectoryFollower: No SimulationManager found! Assign it in Inspector.");
                enabled = false;
            }
        }
    }
    
    IEnumerator LoadTrajectoryCoroutine()
    {
        // Build path to CSV in StreamingAssets
        string filePath = Path.Combine("Trajectories", csvFileName);
        string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
        
        Debug.Log($"GATrajectoryFollower: Loading CSV from {fullPath}");
        
        string csvContent = "";
        
        // On Android, StreamingAssets is inside compressed APK, use UnityWebRequest
        #if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(fullPath);
        yield return www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"GATrajectoryFollower: Failed to load CSV: {www.error}");
            Debug.LogError($"  Path tried: {fullPath}");
            enabled = false;
            yield break;
        }
        
        csvContent = www.downloadHandler.text;
        Debug.Log($"GATrajectoryFollower: CSV loaded via UnityWebRequest ({csvContent.Length} characters)");
        
        #else
        // On Editor/PC, can use File.ReadAllText directly
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"GATrajectoryFollower: CSV file not found at {fullPath}");
            enabled = false;
            yield break;
        }
        
        csvContent = File.ReadAllText(fullPath);
        Debug.Log($"GATrajectoryFollower: CSV loaded via File.ReadAllText ({csvContent.Length} characters)");
        yield return null;
        #endif
        
        // Parse CSV content
        ParseCSV(csvContent);
    }
    
    void ParseCSV(string csvContent)
    {
        try
        {
            string[] lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Log($"GATrajectoryFollower: Parsing {lines.Length} lines...");
            
            // Skip header (line 0)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                
                string[] parts = line.Split(',');
                if (parts.Length < 5) continue;
                
                string type = parts[0].Trim();
                
                // Only process spacecraft entries
                if (type != "spacecraft") continue;
                
                // Parse position (in AU)
                float x_au = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y_au = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z_au = float.Parse(parts[3], CultureInfo.InvariantCulture);
                
                // Convert AU to Unity units (billions of meters)
                // AU -> meters -> billions of meters
                float x = (float)(x_au * AU_TO_METERS / unityScale);
                float y = (float)(z_au * AU_TO_METERS / unityScale); // Z becomes Y (Unity up)
                float z = (float)(y_au * AU_TO_METERS / unityScale); // Y becomes Z (Unity forward)
                
                Vector3 position = new Vector3(x, y, z);
                
                // Parse date-time
                string dateTimeStr = parts[4].Trim();
                DateTime dateTime = DateTime.ParseExact(dateTimeStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                
                trajectoryPoints.Add(new TrajectoryPoint(dateTime, position));
            }
            
            if (trajectoryPoints.Count == 0)
            {
                Debug.LogError("GATrajectoryFollower: No spacecraft entries found in CSV!");
                enabled = false;
                return;
            }
            
            Debug.Log($"✅ GATrajectoryFollower: Loaded {trajectoryPoints.Count} trajectory points");
            Debug.Log($"   Start: {trajectoryPoints[0].dateTime:yyyy-MM-dd} at {trajectoryPoints[0].position}");
            Debug.Log($"   End: {trajectoryPoints[trajectoryPoints.Count - 1].dateTime:yyyy-MM-dd} at {trajectoryPoints[trajectoryPoints.Count - 1].position}");
        }
        catch (Exception e)
        {
            Debug.LogError($"GATrajectoryFollower: Error parsing CSV: {e.Message}");
            Debug.LogError($"  Stack trace: {e.StackTrace}");
            enabled = false;
        }
    }
    
    void Update()
    {
        if (trajectoryPoints.Count == 0) return;
        if (simulationManager == null) return;
        
        // Get current simulation date
        DateTime currentDate = simulationManager.GetCurrentDateTime();
        
        // Find position at current date
        Vector3 position = GetPositionAtDate(currentDate);
        transform.position = position;
        
        if (showDebugInfo && Time.frameCount % 120 == 0) // Every 2 seconds
        {
            float distanceAU = position.magnitude * unityScale / (float)AU_TO_METERS;
            Debug.Log($"🚀 MBR Explorer | Date: {currentDate:yyyy-MM-dd HH:mm} | Pos: {position} | Distance: {distanceAU:F3} AU");
        }
    }
    
    /// <summary>
    /// Get spacecraft position at a specific date using linear interpolation
    /// </summary>
    private Vector3 GetPositionAtDate(DateTime targetDate)
    {
        // Handle edge cases
        if (targetDate <= trajectoryPoints[0].dateTime)
            return trajectoryPoints[0].position;
        
        if (targetDate >= trajectoryPoints[trajectoryPoints.Count - 1].dateTime)
            return trajectoryPoints[trajectoryPoints.Count - 1].position;
        
        // Find surrounding points
        TrajectoryPoint before = null;
        TrajectoryPoint after = null;
        
        for (int i = 0; i < trajectoryPoints.Count - 1; i++)
        {
            if (trajectoryPoints[i].dateTime <= targetDate && trajectoryPoints[i + 1].dateTime > targetDate)
            {
                before = trajectoryPoints[i];
                after = trajectoryPoints[i + 1];
                break;
            }
        }
        
        if (before == null || after == null)
            return trajectoryPoints[0].position;
        
        // Linear interpolation based on time
        double totalSeconds = (after.dateTime - before.dateTime).TotalSeconds;
        double elapsedSeconds = (targetDate - before.dateTime).TotalSeconds;
        float t = (float)(elapsedSeconds / totalSeconds);
        
        return Vector3.Lerp(before.position, after.position, t);
    }
    
    void OnDrawGizmos()
    {
        if (!drawGizmos || trajectoryPoints == null || trajectoryPoints.Count == 0) return;
        
        // Draw trajectory path
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // Cyan
        for (int i = 0; i < trajectoryPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(trajectoryPoints[i].position, trajectoryPoints[i + 1].position);
        }
        
        // Draw start point (green)
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(trajectoryPoints[0].position, 5f);
        
        // Draw end point (red)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(trajectoryPoints[trajectoryPoints.Count - 1].position, 5f);
        
        // Draw current position (yellow, larger)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 8f);
        }
        
        // Draw line from Sun to spacecraft
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawLine(Vector3.zero, transform.position);
    }
    
    /// <summary>
    /// Get current distance from Sun in AU
    /// </summary>
    public float GetDistanceFromSunAU()
    {
        return transform.position.magnitude * unityScale / (float)AU_TO_METERS;
    }
    
    /// <summary>
    /// Get current mission day (days since first trajectory point)
    /// </summary>
    public int GetCurrentMissionDay()
    {
        if (simulationManager == null || trajectoryPoints.Count == 0) return 0;
        
        DateTime currentDate = simulationManager.GetCurrentDateTime();
        return (currentDate - trajectoryPoints[0].dateTime).Days;
    }
}

