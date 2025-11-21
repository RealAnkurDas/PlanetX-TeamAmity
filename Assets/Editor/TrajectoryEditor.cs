#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TrajectoryData))]
public class TrajectoryEditor : Editor
{
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
    
    private int selectedPhaseIndex = 0;
    private bool addWaypointMode = false;
    
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private int GetMissionDay(int phaseIndex, TrajectoryData trajectory)
    {
        DateTime phaseDate = DateTime.Parse(PhaseDates[phaseIndex]);
        return (phaseDate - trajectory.missionStartDate).Days;
    }
    
    public override void OnInspectorGUI()
    {
        TrajectoryData trajectory = (TrajectoryData)target;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🚀 Mission Trajectory Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        // Phase selection
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Select Mission Phase:", EditorStyles.boldLabel);
        
        Color originalBG = GUI.backgroundColor;
        GUI.backgroundColor = PhaseColors[selectedPhaseIndex];
        selectedPhaseIndex = EditorGUILayout.Popup("Current Phase", selectedPhaseIndex, PhaseNames);
        GUI.backgroundColor = originalBG;
        
        EditorGUILayout.LabelField("📅 Date: " + PhaseDates[selectedPhaseIndex]);
        DateTime phaseDate = DateTime.Parse(PhaseDates[selectedPhaseIndex]);
        int dayFromStart = (phaseDate - trajectory.missionStartDate).Days;
        EditorGUILayout.LabelField($"📆 Mission Day: {dayFromStart}");
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(5);
        
        // Add waypoint mode toggle
        EditorGUILayout.BeginVertical("box");
        Color btnColor = GUI.backgroundColor;
        GUI.backgroundColor = addWaypointMode ? Color.green : Color.white;
        
        string buttonText = addWaypointMode 
            ? "✓ ACTIVE: Click in Scene View to Add Waypoint" 
            : "Start Adding Waypoints";
        
        if (GUILayout.Button(buttonText, GUILayout.Height(40)))
        {
            addWaypointMode = !addWaypointMode;
            SceneView.RepaintAll();
            Debug.Log($"Waypoint mode: {(addWaypointMode ? "ENABLED" : "DISABLED")}");
        }
        GUI.backgroundColor = btnColor;
        
        if (addWaypointMode)
        {
            EditorGUILayout.HelpBox(
                $"🎯 Click in Scene view to place waypoints for {PhaseNames[selectedPhaseIndex]}\n" +
                "💡 Tip: Use Top view (Scene Gizmo) for easier 2D placement\n" +
                "💡 Scale: 1 Unity unit = ~1 billion meters", 
                MessageType.Info
            );
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // Waypoint list
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"📍 Waypoints: {trajectory.waypoints.Count}", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("#", EditorStyles.boldLabel, GUILayout.Width(30));
        EditorGUILayout.LabelField("Phase", EditorStyles.boldLabel, GUILayout.Width(150));
        EditorGUILayout.LabelField("Day", EditorStyles.boldLabel, GUILayout.Width(50));
        EditorGUILayout.LabelField("Position (B.m)", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField("", GUILayout.Width(25));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        for (int i = 0; i < trajectory.waypoints.Count; i++)
        {
            var wp = trajectory.waypoints[i];
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(30));
            EditorGUILayout.LabelField(wp.phaseName, GUILayout.Width(150));
            EditorGUILayout.LabelField($"{wp.missionDay}", GUILayout.Width(50));
            
            Vector3 posB = wp.position / 1e9f;  // Convert to billions
            EditorGUILayout.LabelField($"({posB.x:F2}, {posB.y:F2}, {posB.z:F2})", GUILayout.Width(200));
            
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("×", GUILayout.Width(25), GUILayout.Height(18)))
            {
                trajectory.waypoints.RemoveAt(i);
                EditorUtility.SetDirty(trajectory);
                break;
            }
            GUI.backgroundColor = originalBG;
            
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // Utility buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Clear All Waypoints", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Waypoints", 
                "Are you sure you want to delete all waypoints?", "Yes", "Cancel"))
            {
                trajectory.waypoints.Clear();
                EditorUtility.SetDirty(trajectory);
            }
        }
        
        if (GUILayout.Button("Sort by Mission Day", GUILayout.Height(30)))
        {
            trajectory.waypoints.Sort((a, b) => a.missionDay.CompareTo(b.missionDay));
            EditorUtility.SetDirty(trajectory);
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Stats
        if (trajectory.waypoints.Count > 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("📊 Trajectory Stats", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Waypoints: {trajectory.waypoints.Count}");
            EditorGUILayout.LabelField($"Mission Duration: {trajectory.GetTotalMissionDays()} days");
            EditorGUILayout.LabelField($"Start: {trajectory.missionStartDate:yyyy-MM-dd}");
            if (trajectory.waypoints.Count > 0)
            {
                var lastWp = trajectory.waypoints[trajectory.waypoints.Count - 1];
                DateTime endDate = trajectory.missionStartDate.AddDays(lastWp.missionDay);
                EditorGUILayout.LabelField($"End: {endDate:yyyy-MM-dd}");
            }
            EditorGUILayout.EndVertical();
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(trajectory);
        }
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        TrajectoryData trajectory = (TrajectoryData)target;
        if (trajectory == null) return;
        
        if (addWaypointMode)
        {
            // Draw hint in scene view
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 400, 80));
            GUILayout.Box($"Click to add waypoint for:\n{PhaseNames[selectedPhaseIndex]}\n(Day {GetMissionDay(selectedPhaseIndex, trajectory)})", 
                GUILayout.Width(380), GUILayout.Height(70));
            GUILayout.EndArea();
            Handles.EndGUI();
            
            // Keep control to prevent deselection
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            
            Event e = Event.current;
            
            // Debug to see if events are coming through
            if (e.type == EventType.MouseDown)
            {
                Debug.Log($"Mouse clicked! Button: {e.button}, Position: {e.mousePosition}");
            }
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero);  // XZ plane (Y=0)
                
                Debug.Log($"Raycasting from: {ray.origin}, direction: {ray.direction}");
                
                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 position = ray.GetPoint(distance);
                    
                    Debug.Log($"Hit plane at distance: {distance}, position: {position}");
                    
                    DateTime phaseDate = DateTime.Parse(PhaseDates[selectedPhaseIndex]);
                    int missionDay = (phaseDate - trajectory.missionStartDate).Days;
                    
                    var waypoint = new TrajectoryData.Waypoint(
                        position,
                        missionDay,
                        PhaseNames[selectedPhaseIndex],
                        PhaseDates[selectedPhaseIndex]
                    );
                    
                    trajectory.waypoints.Add(waypoint);
                    
                    EditorUtility.SetDirty(trajectory);
                    
                    // Keep the asset selected
                    Selection.activeObject = trajectory;
                    
                    // Repaint inspector
                    Repaint();
                    SceneView.RepaintAll();
                    
                    e.Use();
                    
                    Debug.Log($"✓ Added waypoint #{trajectory.waypoints.Count} at position {position / 1e9f} billion meters (Day {missionDay}, {PhaseNames[selectedPhaseIndex]})");
                }
                else
                {
                    Debug.Log("Raycast did not hit plane");
                }
            }
        }
        
        // ALWAYS draw existing waypoints (even when not in add mode)
        if (trajectory.waypoints.Count > 0)
        {
            // Draw path first (behind spheres)
            if (trajectory.waypoints.Count > 1)
            {
                Handles.color = Color.cyan;
                for (int i = 0; i < trajectory.waypoints.Count - 1; i++)
                {
                    Handles.DrawLine(trajectory.waypoints[i].position, trajectory.waypoints[i + 1].position, 5f);
                }
            }
            
            // Draw waypoints
            for (int i = 0; i < trajectory.waypoints.Count; i++)
            {
                var wp = trajectory.waypoints[i];
                
                // Find phase color
                int phaseIdx = Array.IndexOf(PhaseNames, wp.phaseName);
                Color phaseColor = phaseIdx >= 0 ? PhaseColors[phaseIdx] : Color.yellow;
                
                Handles.color = phaseColor;
                
                // Draw sphere at waypoint
                float sphereSize = HandleUtility.GetHandleSize(wp.position) * 0.1f;
                Handles.SphereHandleCap(0, wp.position, Quaternion.identity, sphereSize, EventType.Repaint);
                
                // Draw label
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 12;
                style.fontStyle = FontStyle.Bold;
                Handles.Label(wp.position, $"#{i + 1} {wp.phaseName}\nDay {wp.missionDay}", style);
            }
        }
        
        // Draw cursor highlight when in add mode
        if (addWaypointMode)
        {
            Handles.color = PhaseColors[selectedPhaseIndex];
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            
            if (plane.Raycast(ray, out float dist))
            {
                Vector3 cursorPos = ray.GetPoint(dist);
                float discSize = HandleUtility.GetHandleSize(cursorPos) * 0.2f;
                Handles.DrawWireDisc(cursorPos, Vector3.up, discSize);
                Handles.DrawLine(cursorPos - Vector3.right * discSize, cursorPos + Vector3.right * discSize);
                Handles.DrawLine(cursorPos - Vector3.forward * discSize, cursorPos + Vector3.forward * discSize);
            }
        }
    }
}
#endif

