using UnityEngine;
using System;

[Serializable]
public class SpacecraftPartSetting
{
    public string partName;
    public GameObject partObject;
    public float zoomDistance = 2f;
    public float fieldOfView = -1f;
    
    [Header("Camera Positioning")]
    [Tooltip("Manual camera position (leave at 0,0,0 to auto-calculate)")]
    public Vector3 cameraPosition = Vector3.zero;
    
    [Tooltip("Manual camera rotation (Euler angles)")]
    public Vector3 cameraRotation = Vector3.zero;
    
    [Tooltip("Use manual position/rotation instead of auto-calculate")]
    public bool useManualPositioning = false;
    
    [TextArea(3, 10)]
    public string partDescription = "Part information goes here...";
}

public class MBRPartSelector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TouchCameraController cameraController;
    [SerializeField] private Transform mbrRoot;
    
    [Header("Display Settings")]
    [SerializeField] private float menuLeftOffset = 20f;
    [SerializeField] private float menuTopOffset = 20f;
    
    [Header("Overview Settings")]
    [Tooltip("Zoom distance for Overview mode (full spacecraft view)")]
    [SerializeField] private float overviewZoomDistance = 5f;
    
    [Tooltip("Use manual camera position for Overview")]
    [SerializeField] private bool useManualOverviewPosition = false;
    
    [Tooltip("Manual camera position for Overview")]
    [SerializeField] private Vector3 overviewCameraPosition = new Vector3(0, 2, -5);
    
    [Tooltip("Manual camera rotation for Overview")]
    [SerializeField] private Vector3 overviewCameraRotation = Vector3.zero;
    
    [Header("Spacecraft Parts")]
    [SerializeField] private SpacecraftPartSetting[] partSettings = new SpacecraftPartSetting[]
    {
        new SpacecraftPartSetting { partName = "High gain antenna", zoomDistance = 2f },
        new SpacecraftPartSetting { partName = "Hydrazine thrusters", zoomDistance = 1.8f },
        new SpacecraftPartSetting { partName = "Propulsion thrusters", zoomDistance = 1.5f },
        new SpacecraftPartSetting { partName = "Sensors", zoomDistance = 1.8f },
        new SpacecraftPartSetting { partName = "Thermal control", zoomDistance = 2.2f },
        new SpacecraftPartSetting { partName = "Solar cells", zoomDistance = 2.5f },
        new SpacecraftPartSetting { partName = "Hinge", zoomDistance = 1.5f }
    };
    
    private string currentPart = "Overview";
    private bool isDropdownOpen = false;
    
    public delegate void PartSelectedDelegate(SpacecraftPartSetting part);
    public event PartSelectedDelegate OnPartSelected;
    
    void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<TouchCameraController>();
            if (cameraController != null)
            {
                Debug.Log("MBRPartSelector: Found TouchCameraController");
            }
        }
        
        if (mbrRoot == null)
        {
            mbrRoot = GameObject.Find("MBR FINAL - Copy")?.transform;
            if (mbrRoot != null)
            {
                Debug.Log("MBRPartSelector: Found MBR FINAL - Copy");
            }
        }
        
        AutoAssignPartSpheres();
        
        // Hide all spheres at start
        HideAllPartSpheres();
        
        if (cameraController != null && mbrRoot != null)
        {
            cameraController.SetTarget(mbrRoot);
            
            // Use manual overview position if enabled
            if (useManualOverviewPosition)
            {
                cameraController.SetCameraTransform(overviewCameraPosition, overviewCameraRotation);
                Debug.Log($"MBRPartSelector: Initial Overview with manual position");
            }
            else
            {
                cameraController.SetZoomDistance(overviewZoomDistance);
                Debug.Log($"MBRPartSelector: Initial Overview, zoom: {overviewZoomDistance}");
            }
        }
    }
    
    void AutoAssignPartSpheres()
    {
        GameObject poi = GameObject.Find("POI");
        if (poi == null)
        {
            Debug.LogWarning("MBRPartSelector: POI GameObject not found!");
            return;
        }
        
        Transform[] children = poi.GetComponentsInChildren<Transform>();
        int sphereIndex = 0;
        
        foreach (Transform child in children)
        {
            if (child == poi.transform) continue;
            
            if (sphereIndex < partSettings.Length && partSettings[sphereIndex].partObject == null)
            {
                partSettings[sphereIndex].partObject = child.gameObject;
                Debug.Log($"MBRPartSelector: Auto-assigned '{child.name}' to '{partSettings[sphereIndex].partName}'");
                sphereIndex++;
            }
        }
    }
    
    void OnGUI()
    {
        float scale = Screen.height / 800f;
        
        GUIStyle dropdownButtonStyle = new GUIStyle(GUI.skin.button);
        dropdownButtonStyle.fontSize = Mathf.RoundToInt(16 * scale);
        dropdownButtonStyle.fontStyle = FontStyle.Bold;
        dropdownButtonStyle.alignment = TextAnchor.MiddleLeft;
        dropdownButtonStyle.normal.textColor = Color.cyan;
        
        float buttonWidth = 180f * scale;
        float buttonHeight = 35f * scale;
        float scaledLeftOffset = menuLeftOffset * scale;
        float scaledTopOffset = menuTopOffset * scale;
        
        string dropdownText = isDropdownOpen ? $"▼ {currentPart}" : $"▶ {currentPart}";
        
        if (GUI.Button(new Rect(scaledLeftOffset, scaledTopOffset, buttonWidth, buttonHeight), dropdownText, dropdownButtonStyle))
        {
            isDropdownOpen = !isDropdownOpen;
        }
        
        if (isDropdownOpen)
        {
            GUIStyle optionStyle = new GUIStyle(GUI.skin.button);
            optionStyle.fontSize = Mathf.RoundToInt(14 * scale);
            optionStyle.fontStyle = FontStyle.Normal;
            optionStyle.alignment = TextAnchor.MiddleLeft;
            optionStyle.normal.textColor = Color.white;
            optionStyle.hover.textColor = Color.yellow;
            
            float yPosition = scaledTopOffset + buttonHeight + (2f * scale);
            float optionHeight = 28f * scale;
            float optionWidth = buttonWidth;
            
            if (GUI.Button(new Rect(scaledLeftOffset, yPosition, optionWidth, optionHeight), "Overview", optionStyle))
            {
                SetOverviewView();
                isDropdownOpen = false;
            }
            yPosition += optionHeight + (2f * scale);
            
            foreach (SpacecraftPartSetting setting in partSettings)
            {
                if (setting.partName == currentPart) continue;
                
                if (GUI.Button(new Rect(scaledLeftOffset, yPosition, optionWidth, optionHeight), setting.partName, optionStyle))
                {
                    SelectPart(setting);
                    isDropdownOpen = false;
                }
                
                yPosition += optionHeight + (2f * scale);
            }
        }
    }
    
    void SetOverviewView()
    {
        currentPart = "Overview";
        
        if (cameraController == null || mbrRoot == null) return;
        
        // Hide all part spheres in overview mode
        HideAllPartSpheres();
        
        cameraController.SetTarget(mbrRoot);
        
        // Apply manual positioning if enabled
        if (useManualOverviewPosition)
        {
            cameraController.SetCameraTransform(overviewCameraPosition, overviewCameraRotation);
            Debug.Log($"MBRPartSelector: Overview with manual position {overviewCameraPosition}, rotation {overviewCameraRotation}");
        }
        else
        {
            cameraController.SetZoomDistance(overviewZoomDistance);
            Debug.Log($"MBRPartSelector: Set to Overview, zoom: {overviewZoomDistance}");
        }
        
        cameraController.ResetFieldOfView();
        OnPartSelected?.Invoke(null);
    }
    
    void SelectPart(SpacecraftPartSetting setting)
    {
        if (cameraController == null)
        {
            Debug.LogError("MBRPartSelector: Camera controller not assigned!");
            return;
        }
        
        if (setting.partObject == null)
        {
            Debug.LogWarning($"MBRPartSelector: Part '{setting.partName}' has no sphere assigned!");
            return;
        }
        
        currentPart = setting.partName;
        
        // Hide all spheres first, then show only this part's spheres
        HideAllPartSpheres();
        ShowPartSpheres(setting);
        
        // Set camera target to the part sphere (for orbit center)
        cameraController.SetTarget(setting.partObject.transform);
        cameraController.SetMinimumZoomDistance(setting.zoomDistance);
        
        // Apply manual positioning if enabled
        if (setting.useManualPositioning)
        {
            Debug.Log($"★★★ MBRPartSelector: Part '{setting.partName}' using MANUAL positioning");
            Debug.Log($"  Setting camera to: Pos={setting.cameraPosition}, Rot={setting.cameraRotation}");
            
            // Set camera to exact position and rotation, syncing controller state
            cameraController.SetCameraTransform(setting.cameraPosition, setting.cameraRotation);
            
            // Verify camera is at correct position
            Camera cam = cameraController.GetComponent<Camera>();
            if (cam != null)
            {
                Debug.Log($"  Camera actual: Pos={cam.transform.position}, Rot={cam.transform.eulerAngles}");
            }
        }
        else
        {
            Debug.Log($"★★★ MBRPartSelector: Part '{setting.partName}' using AUTOMATIC positioning");
            // Automatic mode: Calculate good viewing angle for this part
            CalculateAndSetViewingAngle(setting);
        }
        
        // Set FOV if specified
        if (setting.fieldOfView > 0)
        {
            cameraController.SetFieldOfView(setting.fieldOfView);
        }
        else
        {
            cameraController.ResetFieldOfView();
        }
        
        Debug.Log($"MBRPartSelector: Selected '{setting.partName}', zoom: {setting.zoomDistance}");
        OnPartSelected?.Invoke(setting);
    }
    
    void HideAllPartSpheres()
    {
        foreach (var setting in partSettings)
        {
            if (setting.partObject != null)
            {
                // Hide the sphere and all its children
                SetVisibility(setting.partObject, false);
            }
        }
    }
    
    void ShowPartSpheres(SpacecraftPartSetting setting)
    {
        if (setting.partObject != null)
        {
            // Show the sphere and all its children
            SetVisibility(setting.partObject, true);
            Debug.Log($"MBRPartSelector: Showing spheres for '{setting.partName}'");
        }
    }
    
    void SetVisibility(GameObject obj, bool visible)
    {
        // Set visibility for this object
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
        
        // Set visibility for all children recursively
        foreach (Transform child in obj.transform)
        {
            SetVisibility(child.gameObject, visible);
        }
    }
    
    public string GetCurrentPart()
    {
        return currentPart;
    }
    
    public SpacecraftPartSetting GetCurrentPartSetting()
    {
        foreach (var setting in partSettings)
        {
            if (setting.partName == currentPart)
                return setting;
        }
        return null;
    }
    
    void CalculateAndSetViewingAngle(SpacecraftPartSetting setting)
    {
        if (mbrRoot == null || setting.partObject == null) return;
        
        // Calculate direction from MBR center to part
        Vector3 mbrCenter = mbrRoot.position;
        Vector3 partPos = setting.partObject.transform.position;
        Vector3 directionToPart = (partPos - mbrCenter).normalized;
        
        // Camera should be OPPOSITE this direction (looking at part from outside)
        Vector3 cameraDirection = -directionToPart;
        
        // Calculate camera position
        Vector3 cameraPos = partPos + cameraDirection * setting.zoomDistance;
        
        // Calculate rotation to look at the part
        Quaternion lookRotation = Quaternion.LookRotation(partPos - cameraPos);
        Vector3 eulerRotation = lookRotation.eulerAngles;
        
        // Set camera transform with proper syncing
        cameraController.SetCameraTransform(cameraPos, eulerRotation);
        
        Debug.Log($"MBRPartSelector: Auto-calculated view for '{setting.partName}' from direction {cameraDirection}");
    }
}

