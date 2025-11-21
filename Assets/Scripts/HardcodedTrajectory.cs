using UnityEngine;
using System;
using System.Collections.Generic;

public class HardcodedTrajectory : MonoBehaviour
{
    [Header("Simulation Reference")]
    [Tooltip("Drag the SimulationManager GameObject here to sync speeds")]
    [SerializeField] private EphemerisBasedSimulation simulationManager;
    
    [Header("Mission Time")]
    public int startMissionDay = 1;
    public bool autoProgress = true;
    [Tooltip("Only used if no SimulationManager assigned")]
    public float fallbackTimeScale = 86400f;  // 1 day per second
    
    
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private float missionTimeSeconds = 0f;
    private int currentMissionDay = 1;
    
    [System.Serializable]
    public class TimedWaypoint
    {
        public Vector3 position;
        public DateTime targetDate;
        public double missionSeconds;  // Calculated from targetDate
        
        public TimedWaypoint(Vector3 pos, DateTime date)
        {
            position = pos;
            targetDate = date;
        }
    }
    
    private List<TimedWaypoint> timedWaypoints;
    private DateTime missionStartDate = new DateTime(2028, 3, 3);  // Launch date
    
    void Start()
    {
        // Waypoints with their target dates from the mission plan
        timedWaypoints = new List<TimedWaypoint>
        {
            new TimedWaypoint(new Vector3(-142.218002f, 16.7680168f, 38.6810112f), new DateTime(2028, 3, 3)),    // Launch
            new TimedWaypoint(new Vector3(-144f, 0f, 0f), new DateTime(2028, 4, 15)),
            new TimedWaypoint(new Vector3(-120f, 0f, -53f), new DateTime(2028, 5, 20)),
            new TimedWaypoint(new Vector3(-53f, 0f, -88f), new DateTime(2028, 6, 20)),
            new TimedWaypoint(new Vector3(54f, -36.5995827f, -68f), new DateTime(2028, 7, 10)),                 // Venus GA
            new TimedWaypoint(new Vector3(116f, -36.5995827f, 43f), new DateTime(2028, 9, 1)),
            new TimedWaypoint(new Vector3(94f, -36.5995827f, 159f), new DateTime(2028, 11, 1)),
            new TimedWaypoint(new Vector3(20f, -36.5995827f, 216f), new DateTime(2029, 1, 1)),
            new TimedWaypoint(new Vector3(-124f, -36.5995827f, 204f), new DateTime(2029, 3, 1)),
            new TimedWaypoint(new Vector3(-196f, -36.5995827f, 109f), new DateTime(2029, 4, 15)),
            new TimedWaypoint(new Vector3(-192f, -36.5995827f, 6f), new DateTime(2029, 5, 1)),
            new TimedWaypoint(new Vector3(-141f, -33.980835f, -81f), new DateTime(2029, 5, 15)),
            new TimedWaypoint(new Vector3(-29f, -33.980835f, -117f), new DateTime(2029, 5, 24)),                // Earth GA
            new TimedWaypoint(new Vector3(73f, -33.980835f, -94f), new DateTime(2029, 8, 1)),
            new TimedWaypoint(new Vector3(156f, -33.980835f, 24f), new DateTime(2029, 11, 1)),
            new TimedWaypoint(new Vector3(162f, -33.980835f, 166f), new DateTime(2030, 1, 1)),
            new TimedWaypoint(new Vector3(102f, -33.980835f, 256f), new DateTime(2030, 1, 20)),
            new TimedWaypoint(new Vector3(-1f, 1.19440985f, 309f), new DateTime(2030, 2, 18)),                  // Westerwald
            new TimedWaypoint(new Vector3(-184f, 10.8671513f, 275f), new DateTime(2030, 6, 14)),                // Chimaera
            new TimedWaypoint(new Vector3(-290f, 10.8671513f, 172f), new DateTime(2030, 10, 1)),
            new TimedWaypoint(new Vector3(-303.911957f, 25.2730656f, 35.4405975f), new DateTime(2031, 1, 14)), // Rockox
            new TimedWaypoint(new Vector3(-242f, 25.2730656f, -73f), new DateTime(2031, 5, 1)),
            new TimedWaypoint(new Vector3(-92f, 25.2730656f, -163f), new DateTime(2031, 8, 1)),
            new TimedWaypoint(new Vector3(127f, -75.4827805f, -134f), new DateTime(2031, 9, 23)),               // Mars GA
            new TimedWaypoint(new Vector3(233f, -75.4827805f, -72f), new DateTime(2032, 2, 1)),
            new TimedWaypoint(new Vector3(321f, -75.4827805f, 87f), new DateTime(2032, 5, 1)),
            new TimedWaypoint(new Vector3(260.939056f, -90.5532303f, 348.547516f), new DateTime(2032, 7, 24)),  // 2000 VA28
            new TimedWaypoint(new Vector3(59f, -90.5532303f, 450f), new DateTime(2032, 12, 15)),                // 1998 RC76
            new TimedWaypoint(new Vector3(-170f, -90.5532303f, 417f), new DateTime(2033, 3, 1)),
            new TimedWaypoint(new Vector3(-353.733154f, -8.91720581f, 255.229736f), new DateTime(2033, 9, 2)),  // 1999 SG6
            new TimedWaypoint(new Vector3(-389f, -8.91720581f, 77f), new DateTime(2034, 3, 1)),
            new TimedWaypoint(new Vector3(-358.700928f, 19.8580894f, -69.8066406f), new DateTime(2034, 8, 1)),
            new TimedWaypoint(new Vector3(-192f, 29.9936085f, -236f), new DateTime(2034, 10, 30)),              // Justitia Arrival
            new TimedWaypoint(new Vector3(163.997528f, 16.971941f, -264.596039f), new DateTime(2035, 3, 1)),
            new TimedWaypoint(new Vector3(163.997528f, 16.971941f, -264.596039f), new DateTime(2035, 5, 1))     // Deploy Lander
        };
        
        // Calculate mission seconds for each waypoint
        foreach (var wp in timedWaypoints)
        {
            wp.missionSeconds = (wp.targetDate - missionStartDate).TotalSeconds;
        }
        
        Debug.Log($"✅ Trajectory initialized with {timedWaypoints.Count} timed waypoints");
        Debug.Log($"   Mission start: {missionStartDate:yyyy-MM-dd}");
        Debug.Log($"   First waypoint: {timedWaypoints[0].targetDate:yyyy-MM-dd} at {timedWaypoints[0].position}");
        Debug.Log($"   Last waypoint: {timedWaypoints[timedWaypoints.Count - 1].targetDate:yyyy-MM-dd} at {timedWaypoints[timedWaypoints.Count - 1].position}");
    }
    
    void Update()
    {
        if (timedWaypoints == null || timedWaypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints!");
            return;
        }
        
        // Get current simulation date/time
        DateTime currentSimDate;
        
        if (simulationManager != null)
        {
            // Sync with simulation date
            currentSimDate = simulationManager.GetCurrentDateTime();
        }
        else
        {
            // Fallback: calculate from our own time
            if (autoProgress)
            {
                missionTimeSeconds += Time.deltaTime * fallbackTimeScale;
            }
            currentSimDate = missionStartDate.AddSeconds(missionTimeSeconds);
        }
        
        // Calculate seconds since mission start
        double currentMissionSeconds = (currentSimDate - missionStartDate).TotalSeconds;
        
        // Get position for current date - Catmull-Rom handles smoothness
        Vector3 position = GetPositionAtTime(currentMissionSeconds);
        transform.position = position;
        
        if (showDebugInfo && Time.frameCount % 60 == 0)
        {
            Debug.Log($"🚀 Date: {currentSimDate:yyyy-MM-dd} | Position: {position}");
        }
    }
    
    private Vector3 GetPositionAtTime(double missionSeconds)
    {
        if (timedWaypoints.Count == 1) return timedWaypoints[0].position;
        
        // Find surrounding waypoints based on time
        TimedWaypoint before = null;
        TimedWaypoint after = null;
        
        for (int i = 0; i < timedWaypoints.Count; i++)
        {
            if (timedWaypoints[i].missionSeconds <= missionSeconds)
            {
                before = timedWaypoints[i];
            }
            else
            {
                after = timedWaypoints[i];
                break;
            }
        }
        
        // Edge cases
        if (before == null) return timedWaypoints[0].position;
        if (after == null) return timedWaypoints[timedWaypoints.Count - 1].position;
        
        // Interpolate based on time - use RAW time ratio (no easing per segment)
        double timeBetweenWaypoints = after.missionSeconds - before.missionSeconds;
        double timeFromBefore = missionSeconds - before.missionSeconds;
        float t = (float)(timeFromBefore / timeBetweenWaypoints);
        t = Mathf.Clamp01(t);
        
        // Use Catmull-Rom for inherently smooth curves
        // This creates smooth velocity changes across multiple waypoints
        int beforeIndex = timedWaypoints.IndexOf(before);
        int afterIndex = timedWaypoints.IndexOf(after);
        
        if (timedWaypoints.Count > 3)
        {
            // Get 4 control points for Catmull-Rom
            int i0 = Mathf.Max(0, beforeIndex - 1);
            int i1 = beforeIndex;
            int i2 = afterIndex;
            int i3 = Mathf.Min(timedWaypoints.Count - 1, afterIndex + 1);
            
            // Standard Catmull-Rom for smooth curves
            return CatmullRom(
                timedWaypoints[i0].position,
                timedWaypoints[i1].position,
                timedWaypoints[i2].position,
                timedWaypoints[i3].position,
                t
            );
        }
        else
        {
            // Simple linear interpolation
            return Vector3.Lerp(before.position, after.position, t);
        }
    }
    
    /// <summary>
    /// Standard Catmull-Rom spline - naturally smooth velocity
    /// </summary>
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
    
    void OnDrawGizmos()
    {
        if (timedWaypoints == null || timedWaypoints.Count == 0) return;
        
        // Draw sun at origin
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.zero, 10f);
        
        // Draw waypoints
        Gizmos.color = Color.yellow;
        foreach (var wp in timedWaypoints)
        {
            Gizmos.DrawSphere(wp.position, 5f);
        }
        
        // Draw path (smooth interpolation preview)
        Gizmos.color = Color.cyan;
        double totalMissionSeconds = (timedWaypoints[timedWaypoints.Count - 1].targetDate - missionStartDate).TotalSeconds;
        
        int segments = 200;
        for (int i = 0; i < segments; i++)
        {
            double t1 = ((double)i / segments) * totalMissionSeconds;
            double t2 = ((double)(i + 1) / segments) * totalMissionSeconds;
            
            Vector3 pos1 = GetPositionAtTime(t1);
            Vector3 pos2 = GetPositionAtTime(t2);
            
            Gizmos.DrawLine(pos1, pos2);
        }
        
        // Draw current position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 15f);
        }
    }
}

