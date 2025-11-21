using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MissionTrajectory", menuName = "Mission/Trajectory Data")]
public class TrajectoryData : ScriptableObject
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 position;  // Position in meters (billions)
        public int missionDay;
        public string phaseName;
        public string phaseDate;
        
        public Waypoint(Vector3 pos, int day, string phase, string date)
        {
            position = pos;
            missionDay = day;
            phaseName = phase;
            phaseDate = date;
        }
    }
    
    public List<Waypoint> waypoints = new List<Waypoint>();
    public DateTime missionStartDate = new DateTime(2028, 3, 3);
    
    /// <summary>
    /// Get interpolated position at a specific mission day
    /// </summary>
    public Vector3 GetPositionAtDay(int missionDay)
    {
        if (waypoints.Count == 0) return Vector3.zero;
        if (waypoints.Count == 1) return waypoints[0].position;
        
        // Find surrounding waypoints
        Waypoint before = null;
        Waypoint after = null;
        
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i].missionDay <= missionDay)
            {
                before = waypoints[i];
            }
            else
            {
                after = waypoints[i];
                break;
            }
        }
        
        // Edge cases
        if (before == null) return waypoints[0].position;
        if (after == null) return waypoints[waypoints.Count - 1].position;
        
        // Smooth interpolation using Catmull-Rom spline
        float t = (float)(missionDay - before.missionDay) / (float)(after.missionDay - before.missionDay);
        
        // Get 4 points for Catmull-Rom (p0, p1, p2, p3)
        int beforeIndex = waypoints.IndexOf(before);
        int afterIndex = waypoints.IndexOf(after);
        
        Vector3 p0 = beforeIndex > 0 ? waypoints[beforeIndex - 1].position : before.position;
        Vector3 p1 = before.position;
        Vector3 p2 = after.position;
        Vector3 p3 = afterIndex < waypoints.Count - 1 ? waypoints[afterIndex + 1].position : after.position;
        
        return CatmullRom(p0, p1, p2, p3, t);
    }
    
    /// <summary>
    /// Catmull-Rom spline interpolation for smooth curves
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
    
    /// <summary>
    /// Get current phase at mission day
    /// </summary>
    public string GetPhaseAtDay(int missionDay)
    {
        Waypoint current = null;
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            if (waypoints[i].missionDay <= missionDay)
            {
                current = waypoints[i];
                break;
            }
        }
        return current != null ? current.phaseName : "Launch";
    }
    
    /// <summary>
    /// Get total mission duration in days
    /// </summary>
    public int GetTotalMissionDays()
    {
        if (waypoints.Count == 0) return 0;
        return waypoints[waypoints.Count - 1].missionDay;
    }
}

