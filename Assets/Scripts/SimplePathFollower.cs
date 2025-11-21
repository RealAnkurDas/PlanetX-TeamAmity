using UnityEngine;
using System.Collections.Generic;

public class SimplePathFollower : MonoBehaviour
{
    [Header("Path Configuration")]
    [Tooltip("Drag empty GameObjects here to define the path")]
    public Transform[] pathPoints;
    
    [Header("Mission Time")]
    public int startMissionDay = 1;
    public bool autoProgress = true;
    public float timeScale = 60f;  // 1 real second = 60 mission seconds
    
    [Header("Path Settings")]
    public bool useSmoothing = true;
    public float smoothness = 0.5f;  // 0 = linear, 1 = very smooth
    
    [Header("Debug")]
    public bool showPath = true;
    public Color pathColor = Color.cyan;
    
    private float missionTimeSeconds = 0f;
    private int currentMissionDay = 1;
    
    void Start()
    {
        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("No path points assigned! Create empty GameObjects and drag them to the Path Points array.");
            enabled = false;
            return;
        }
        
        currentMissionDay = startMissionDay;
        Debug.Log($"✓ Path follower initialized with {pathPoints.Length} points");
        
        // Debug: show all path point positions
        for (int i = 0; i < pathPoints.Length; i++)
        {
            if (pathPoints[i] != null)
            {
                Debug.Log($"  Point {i + 1}: {pathPoints[i].name} at {pathPoints[i].position}");
            }
            else
            {
                Debug.LogWarning($"  Point {i + 1}: NULL - please assign!");
            }
        }
    }
    
    void Update()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;
        
        if (autoProgress)
        {
            missionTimeSeconds += Time.deltaTime * timeScale;
            currentMissionDay = Mathf.FloorToInt(startMissionDay + missionTimeSeconds / 86400f);
        }
        
        // Map mission day to path progress (0 to 1)
        // Assume mission is 2600 days total
        float progress = (currentMissionDay - startMissionDay) / 2600f;
        progress = Mathf.Clamp01(progress);
        
        // Get position along path
        Vector3 position = GetPositionOnPath(progress);
        Vector3 oldPosition = transform.position;
        transform.position = position;
        
        // Debug every 2 seconds
        if (Time.frameCount % 120 == 0)
        {
            float moved = Vector3.Distance(oldPosition, position);
            Debug.Log($"🚀 Day {currentMissionDay} | Progress: {progress * 100:F3}% | Position: {position} | Moved: {moved:F2} units");
            Debug.Log($"   Time.deltaTime: {Time.deltaTime}, timeScale: {timeScale}, missionTimeSeconds: {missionTimeSeconds:F2}");
        }
    }
    
    private Vector3 GetPositionOnPath(float t)
    {
        if (pathPoints.Length == 1)
        {
            Vector3 pos = pathPoints[0].position;
            Debug.Log($"Single point: {pos / 1e9f}");
            return pos;
        }
        
        // Map t (0 to 1) to segment
        float scaledT = t * (pathPoints.Length - 1);
        int index = Mathf.FloorToInt(scaledT);
        float localT = scaledT - index;
        
        index = Mathf.Clamp(index, 0, pathPoints.Length - 2);
        
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"GetPositionOnPath: t={t:F3}, scaledT={scaledT:F3}, index={index}, localT={localT:F3}");
            Debug.Log($"  Point[{index}]: {pathPoints[index].name} = {pathPoints[index].position}");
            Debug.Log($"  Point[{index + 1}]: {pathPoints[index + 1].name} = {pathPoints[index + 1].position}");
        }
        
        if (useSmoothing && pathPoints.Length > 2)
        {
            // Catmull-Rom spline
            int i0 = Mathf.Max(0, index - 1);
            int i1 = index;
            int i2 = index + 1;
            int i3 = Mathf.Min(pathPoints.Length - 1, index + 2);
            
            Vector3 result = CatmullRom(
                pathPoints[i0].position,
                pathPoints[i1].position,
                pathPoints[i2].position,
                pathPoints[i3].position,
                localT
            );
            
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log($"  Catmull-Rom result: {result}");
            }
            
            return result;
        }
        else
        {
            // Linear interpolation
            Vector3 result = Vector3.Lerp(pathPoints[index].position, pathPoints[index + 1].position, localT);
            
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log($"  Linear interp result: {result}");
            }
            
            return result;
        }
    }
    
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
        if (!showPath || pathPoints == null || pathPoints.Length < 2) return;
        
        // Draw path points
        Gizmos.color = Color.yellow;
        foreach (var point in pathPoints)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, 5e9f);
            }
        }
        
        // Draw path
        Gizmos.color = pathColor;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                // Draw smooth path with multiple segments
                int segments = 20;
                for (int s = 0; s < segments; s++)
                {
                    float t1 = (float)s / segments;
                    float t2 = (float)(s + 1) / segments;
                    
                    float pathT1 = (i + t1) / (pathPoints.Length - 1);
                    float pathT2 = (i + t2) / (pathPoints.Length - 1);
                    
                    Vector3 pos1 = GetPositionOnPath(pathT1);
                    Vector3 pos2 = GetPositionOnPath(pathT2);
                    
                    Gizmos.DrawLine(pos1, pos2);
                }
            }
        }
        
        // Draw current position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1e10f);
        }
    }
}

