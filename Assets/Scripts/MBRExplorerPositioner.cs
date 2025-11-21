using UnityEngine;
using System;

public class MBRExplorerPositioner : MonoBehaviour
{
    [Header("Trajectory")]
    [SerializeField] private TrajectoryData trajectoryData;
    
    [Header("Mission Time Control")]
    [SerializeField] private int currentMissionDay = 1;  // Start at Day 1
    [SerializeField] private bool autoProgress = true;
    [SerializeField] private float timeScale = 60f;  // 1 real second = 60 mission seconds
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private float missionTimeSeconds = 0f;
    
    void Start()
    {
        if (trajectoryData == null)
        {
            Debug.LogError("TrajectoryData not assigned to MBR Explorer!");
            enabled = false;
            return;
        }
        
        UpdatePosition(currentMissionDay);
        
        if (showDebugInfo)
        {
            Debug.Log($"MBR Explorer initialized with {trajectoryData.waypoints.Count} waypoints");
            Debug.Log($"Mission duration: {trajectoryData.GetTotalMissionDays()} days");
        }
    }
    
    void Update()
    {
        if (trajectoryData == null) return;
        
        if (autoProgress)
        {
            // Update mission time based on time scale
            missionTimeSeconds += Time.deltaTime * timeScale;
            currentMissionDay = Mathf.FloorToInt(missionTimeSeconds / 86400f);
            
            // Clamp to mission duration
            int maxDays = trajectoryData.GetTotalMissionDays();
            if (currentMissionDay > maxDays)
            {
                currentMissionDay = maxDays;
                autoProgress = false;  // Stop at end
            }
        }
        
        UpdatePosition(currentMissionDay);
    }
    
    private void UpdatePosition(int missionDay)
    {
        Vector3 position = trajectoryData.GetPositionAtDay(missionDay);
        transform.position = position;
        
        if (showDebugInfo && Time.frameCount % 60 == 0)
        {
            string phase = trajectoryData.GetPhaseAtDay(missionDay);
            float distanceBillionMeters = position.magnitude / 1e9f;
            Debug.Log($"Day {missionDay} | Phase: {phase} | Distance: {distanceBillionMeters:F2}B meters");
        }
    }
    
    // Public control methods
    
    /// <summary>
    /// Set time scale dynamically (matches Unity time step changes)
    /// </summary>
    public void SetTimeScale(float newScale)
    {
        timeScale = newScale;
    }
    
    /// <summary>
    /// Jump to specific mission day
    /// </summary>
    public void SetMissionDay(int day)
    {
        currentMissionDay = day;
        missionTimeSeconds = day * 86400f;
        UpdatePosition(currentMissionDay);
    }
    
    /// <summary>
    /// Jump to specific date
    /// </summary>
    public void SetPositionByDate(DateTime targetDate)
    {
        int missionDay = (targetDate - trajectoryData.missionStartDate).Days;
        SetMissionDay(missionDay);
    }
    
    /// <summary>
    /// Pause/Resume simulation
    /// </summary>
    public void SetPaused(bool paused)
    {
        autoProgress = !paused;
    }
    
    /// <summary>
    /// Get current position in billions of meters
    /// </summary>
    public Vector3 GetPositionBillionMeters()
    {
        return transform.position / 1e9f;
    }
    
    /// <summary>
    /// Get distance from sun in AU
    /// </summary>
    public float GetDistanceFromSunAU()
    {
        return transform.position.magnitude / 149597870700f;
    }
    
    void OnDrawGizmos()
    {
        if (trajectoryData == null || trajectoryData.waypoints.Count == 0) return;
        
        // Draw trajectory path
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        for (int i = 0; i < trajectoryData.waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(
                trajectoryData.waypoints[i].position, 
                trajectoryData.waypoints[i + 1].position
            );
        }
        
        // Draw waypoints
        Gizmos.color = Color.yellow;
        foreach (var wp in trajectoryData.waypoints)
        {
            Gizmos.DrawSphere(wp.position, 5e9f);
        }
        
        // Draw current position (larger, red sphere)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 15e9f);
        
        // Draw line from sun to spacecraft
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawLine(Vector3.zero, transform.position);
    }
}

