using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TrajectoryEditorRuntime : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TrajectoryData trajectoryData;
    [SerializeField] private Camera editorCamera;
    
    [Header("Mission Phases")]
    [SerializeField] private int currentPhaseIndex = 0;
    
    [Header("Visual Settings")]
    [SerializeField] private float waypointSphereSize = 1e10f;
    [SerializeField] private Color cursorColor = Color.green;
    [SerializeField] private float gridSize = 1e11f;
    
    private bool editModeActive = false;
    
    private static readonly string[] PhaseNames = {
        "Launch", "Venus Gravity Assist", "Earth Gravity Assist", 
        "Westerwald", "Chimaera", "Rockox", "Mars Gravity Assist",
        "2000 VA28", "1998 RC76", "1999 SG6", "Justitia Arrival", "Deploy Lander"
    };
    
    private static readonly string[] PhaseDates = {
        "2028-03-03", "2028-07-10", "2029-05-24", "2030-02-18",
        "2030-06-14", "2031-01-14", "2031-09-23", "2032-07-24",
        "2032-12-15", "2033-09-02", "2034-10-30", "2035-05-01"
    };
    
    private static readonly Color[] PhaseColors = {
        new Color(1f, 0f, 0f),      // Red
        new Color(1f, 0.53f, 0f),   // Orange
        new Color(1f, 1f, 0f),      // Yellow
        new Color(0.53f, 1f, 0f),   // Light Green
        new Color(0f, 1f, 0f),      // Green
        new Color(0f, 1f, 1f),      // Cyan
        new Color(0f, 0.53f, 1f),   // Blue
        new Color(0.29f, 0f, 0.51f),// Indigo
        new Color(0.53f, 0f, 1f),   // Violet
        new Color(1f, 0f, 1f),      // Pink
        new Color(1f, 0f, 0.53f),   // Magenta
        new Color(0.53f, 0f, 0f)    // Dark Red
    };
    
    private Vector3? cursorPosition = null;
    
    void Start()
    {
        if (editorCamera == null)
        {
            editorCamera = Camera.main;
            Debug.Log($"Auto-assigned camera: {editorCamera?.name ?? "NULL"}");
        }
        else
        {
            Debug.Log($"Using assigned camera: {editorCamera.name}");
        }
        
        if (editorCamera == null)
        {
            Debug.LogError("No camera found! Assign a camera in Inspector.");
        }
        
        if (trajectoryData == null)
        {
            Debug.LogError("TrajectoryData not assigned!");
        }
    }
    
    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        
        if (keyboard == null || mouse == null) return;
        
        // Toggle edit mode
        if (keyboard.eKey.wasPressedThisFrame)
        {
            editModeActive = !editModeActive;
            Debug.Log($"Trajectory Edit Mode: {(editModeActive ? "ON" : "OFF")} - Press E to toggle");
            
            if (editModeActive)
            {
                Debug.Log($"Current Phase: {PhaseNames[currentPhaseIndex]} (Press 1-9, 0, -, = to switch phases)");
            }
        }
        
        if (!editModeActive) return;
        
        // Phase selection with number keys
        if (keyboard.digit1Key.wasPressedThisFrame) { currentPhaseIndex = 0; Debug.Log($"Phase: {PhaseNames[0]}"); }
        if (keyboard.digit2Key.wasPressedThisFrame) { currentPhaseIndex = 1; Debug.Log($"Phase: {PhaseNames[1]}"); }
        if (keyboard.digit3Key.wasPressedThisFrame) { currentPhaseIndex = 2; Debug.Log($"Phase: {PhaseNames[2]}"); }
        if (keyboard.digit4Key.wasPressedThisFrame) { currentPhaseIndex = 3; Debug.Log($"Phase: {PhaseNames[3]}"); }
        if (keyboard.digit5Key.wasPressedThisFrame) { currentPhaseIndex = 4; Debug.Log($"Phase: {PhaseNames[4]}"); }
        if (keyboard.digit6Key.wasPressedThisFrame) { currentPhaseIndex = 5; Debug.Log($"Phase: {PhaseNames[5]}"); }
        if (keyboard.digit7Key.wasPressedThisFrame) { currentPhaseIndex = 6; Debug.Log($"Phase: {PhaseNames[6]}"); }
        if (keyboard.digit8Key.wasPressedThisFrame) { currentPhaseIndex = 7; Debug.Log($"Phase: {PhaseNames[7]}"); }
        if (keyboard.digit9Key.wasPressedThisFrame) { currentPhaseIndex = 8; Debug.Log($"Phase: {PhaseNames[8]}"); }
        if (keyboard.digit0Key.wasPressedThisFrame) { currentPhaseIndex = 9; Debug.Log($"Phase: {PhaseNames[9]}"); }
        if (keyboard.minusKey.wasPressedThisFrame) { currentPhaseIndex = 10; Debug.Log($"Phase: {PhaseNames[10]}"); }
        if (keyboard.equalsKey.wasPressedThisFrame) { currentPhaseIndex = 11; Debug.Log($"Phase: {PhaseNames[11]}"); }
        
        // Clamp phase index
        currentPhaseIndex = Mathf.Clamp(currentPhaseIndex, 0, PhaseNames.Length - 1);
        
        // Update cursor position
        UpdateCursorPosition();
        
        // Add waypoint on click
        if (mouse.leftButton.wasPressedThisFrame && cursorPosition.HasValue)
        {
            AddWaypoint(cursorPosition.Value);
        }
        
        // Undo last waypoint
        if (keyboard.backspaceKey.wasPressedThisFrame && trajectoryData != null && trajectoryData.waypoints.Count > 0)
        {
            trajectoryData.waypoints.RemoveAt(trajectoryData.waypoints.Count - 1);
            SaveTrajectory();
            Debug.Log($"Removed last waypoint. Remaining: {trajectoryData.waypoints.Count}");
        }
        
        // Export to CSV
        if (keyboard.xKey.wasPressedThisFrame)
        {
            ExportToCSV();
        }
    }
    
    private void UpdateCursorPosition()
    {
        var mouse = Mouse.current;
        if (mouse == null || editorCamera == null)
        {
            cursorPosition = null;
            return;
        }
        
        Vector2 mousePos = mouse.position.ReadValue();
        
        // Debug camera and mouse info
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"🎥 Camera: {editorCamera.name} at {editorCamera.transform.position}");
            Debug.Log($"🖱️ Mouse screen pos: {mousePos}");
        }
        
        Ray ray = editorCamera.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Vector3.up, Vector3.zero);  // XZ plane at Y=0
        
        // Debug ray info
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"🔦 Ray origin: {ray.origin}, direction: {ray.direction}");
        }
        
        if (plane.Raycast(ray, out float distance))
        {
            cursorPosition = ray.GetPoint(distance);
            
            // Debug output every 60 frames to avoid spam
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"📍 Cursor Position: {cursorPosition.Value} meters = {cursorPosition.Value / 1e9f} billion meters, distance: {distance}");
            }
        }
        else
        {
            cursorPosition = null;
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log("❌ Ray did not hit plane");
            }
        }
    }
    
    private void AddWaypoint(Vector3 position)
    {
        if (trajectoryData == null) return;
        
        Debug.Log($"🎯 AddWaypoint called with position: {position} (raw), {position / 1e9f} (billion meters)");
        
        DateTime phaseDate = DateTime.Parse(PhaseDates[currentPhaseIndex]);
        int missionDay = (phaseDate - trajectoryData.missionStartDate).Days;
        
        var waypoint = new TrajectoryData.Waypoint(
            position,
            missionDay,
            PhaseNames[currentPhaseIndex],
            PhaseDates[currentPhaseIndex]
        );
        
        Debug.Log($"📦 Waypoint created - Position: {waypoint.position}, Day: {waypoint.missionDay}, Phase: {waypoint.phaseName}");
        
        trajectoryData.waypoints.Add(waypoint);
        
        Debug.Log($"📋 Waypoint added to list. Total count: {trajectoryData.waypoints.Count}");
        
        // SAVE TO DISK IMMEDIATELY
        SaveTrajectory();
        
        Debug.Log($"✓ COMPLETE: Waypoint #{trajectoryData.waypoints.Count} saved - {PhaseNames[currentPhaseIndex]} (Day {missionDay})");
    }
    
    private void SaveTrajectory()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(trajectoryData);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("💾 Trajectory saved!");
#endif
    }
    
    private void ExportToCSV()
    {
        if (trajectoryData == null || trajectoryData.waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints to export!");
            return;
        }
        
        string filePath = Application.dataPath + "/ExportedTrajectory.csv";
        
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("waypoint_id,x_meters,y_meters,z_meters,x_billion_m,y_billion_m,z_billion_m,mission_day,phase_name,phase_date");
                
                // Write data
                for (int i = 0; i < trajectoryData.waypoints.Count; i++)
                {
                    var wp = trajectoryData.waypoints[i];
                    writer.WriteLine($"{i + 1},{wp.position.x},{wp.position.y},{wp.position.z}," +
                                   $"{wp.position.x / 1e9f},{wp.position.y / 1e9f},{wp.position.z / 1e9f}," +
                                   $"{wp.missionDay},{wp.phaseName},{wp.phaseDate}");
                }
            }
            
            Debug.Log($"✅ Exported {trajectoryData.waypoints.Count} waypoints to: {filePath}");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export CSV: {e.Message}");
        }
    }
    
    void OnGUI()
    {
        if (!editModeActive) return;
        
        // Draw UI panel
        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        
        GUI.backgroundColor = new Color(0, 0, 0, 0.8f);
        GUILayout.BeginVertical("box");
        GUI.backgroundColor = Color.white;
        
        GUILayout.Label("🚀 TRAJECTORY EDITOR MODE", GUI.skin.box);
        GUILayout.Space(5);
        
        // Current phase display
        GUI.backgroundColor = PhaseColors[currentPhaseIndex];
        GUILayout.Box($"Current Phase: {PhaseNames[currentPhaseIndex]}\nDate: {PhaseDates[currentPhaseIndex]}", 
            GUILayout.Height(40));
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(5);
        
        // Instructions
        GUILayout.Label("Controls:", GUI.skin.box);
        GUILayout.Label("• Click to place waypoint");
        GUILayout.Label("• 1-9, 0, -, = : Select phase");
        GUILayout.Label("• E : Toggle editor");
        GUILayout.Label("• Backspace : Undo last");
        GUILayout.Label("• X : Export to CSV");
        
        GUILayout.Space(5);
        GUILayout.Label($"Waypoints: {trajectoryData?.waypoints.Count ?? 0}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        // Draw cursor position if available
        if (cursorPosition.HasValue && trajectoryData != null)
        {
            Vector3 screenPos = editorCamera.WorldToScreenPoint(cursorPosition.Value);
            if (screenPos.z > 0)
            {
                GUI.color = PhaseColors[currentPhaseIndex];
                GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 20, 100, 20), 
                    "Click Here", GUI.skin.box);
                GUI.color = Color.white;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (trajectoryData == null) return;
        
        Camera cam = editorCamera != null ? editorCamera : Camera.main;
        if (cam == null) return;
        
        // Calculate dynamic scale based on camera distance
        Vector3 centerPoint = trajectoryData.waypoints.Count > 0 
            ? trajectoryData.waypoints[0].position 
            : Vector3.zero;
        float cameraDistance = Vector3.Distance(cam.transform.position, centerPoint);
        float dynamicScale = cameraDistance * 0.005f;  // 0.5% of camera distance
        
        // Draw grid on XZ plane
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        for (int x = -5; x <= 5; x++)
        {
            Gizmos.DrawLine(
                new Vector3(x * gridSize, 0, -5 * gridSize),
                new Vector3(x * gridSize, 0, 5 * gridSize)
            );
        }
        for (int z = -5; z <= 5; z++)
        {
            Gizmos.DrawLine(
                new Vector3(-5 * gridSize, 0, z * gridSize),
                new Vector3(5 * gridSize, 0, z * gridSize)
            );
        }
        
        // Draw sun at origin (scaled - small)
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.zero, dynamicScale * 1.5f);
        
        // Draw trajectory path (thicker lines)
        if (trajectoryData.waypoints.Count > 1)
        {
            for (int i = 0; i < trajectoryData.waypoints.Count - 1; i++)
            {
                // Draw multiple lines for thickness
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(trajectoryData.waypoints[i].position, trajectoryData.waypoints[i + 1].position);
                
                // Additional offset lines for thickness effect
                Vector3 offset = Vector3.up * (dynamicScale * 0.1f);
                Gizmos.DrawLine(
                    trajectoryData.waypoints[i].position + offset, 
                    trajectoryData.waypoints[i + 1].position + offset
                );
                Gizmos.DrawLine(
                    trajectoryData.waypoints[i].position - offset, 
                    trajectoryData.waypoints[i + 1].position - offset
                );
            }
        }
        
        // Draw waypoints (auto-scaled)
        for (int i = 0; i < trajectoryData.waypoints.Count; i++)
        {
            var wp = trajectoryData.waypoints[i];
            int phaseIdx = Array.IndexOf(PhaseNames, wp.phaseName);
            Color phaseColor = phaseIdx >= 0 ? PhaseColors[phaseIdx] : Color.yellow;
            
            // Last waypoint is bright green
            bool isLastWaypoint = (i == trajectoryData.waypoints.Count - 1);
            
            // Calculate size based on camera distance to this waypoint
            float waypointDistance = Vector3.Distance(cam.transform.position, wp.position);
            float scaledSize = waypointDistance * 0.003f;  // 0.3% of distance (smaller)
            
            if (isLastWaypoint)
            {
                // BRIGHT GREEN - most recent waypoint
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(wp.position, scaledSize * 3f);  // 3x larger
                
                // Pulsing white outline
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(wp.position, scaledSize * 3.5f);
                Gizmos.DrawWireSphere(wp.position, scaledSize * 4f);
            }
            else
            {
                // Phase-colored waypoints
                Gizmos.color = phaseColor;
                Gizmos.DrawSphere(wp.position, scaledSize);
                
                // Small white outline for visibility
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(wp.position, scaledSize * 1.1f);
            }
        }
        
        // Draw cursor position (very visible)
        if (editModeActive && cursorPosition.HasValue)
        {
            float cursorDist = Vector3.Distance(cam.transform.position, cursorPosition.Value);
            float cursorSize = cursorDist * 0.02f;
            
            // Pulsing colored circle
            Gizmos.color = PhaseColors[currentPhaseIndex];
            Gizmos.DrawWireSphere(cursorPosition.Value, cursorSize);
            Gizmos.DrawWireSphere(cursorPosition.Value, cursorSize * 1.5f);
            
            // Crosshair
            Gizmos.color = Color.white;
            Gizmos.DrawLine(cursorPosition.Value - Vector3.right * cursorSize * 2, 
                           cursorPosition.Value + Vector3.right * cursorSize * 2);
            Gizmos.DrawLine(cursorPosition.Value - Vector3.forward * cursorSize * 2, 
                           cursorPosition.Value + Vector3.forward * cursorSize * 2);
        }
    }
}

