using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Detects if running in AR mode and disables incompatible components
/// Attach to camera controller or other scripts that need different behavior in AR
/// </summary>
public class ARModeDetector : MonoBehaviour
{
    [Header("Components to Disable in AR")]
    [Tooltip("Components that should be disabled when in AR mode")]
    [SerializeField] private MonoBehaviour[] disableInAR;
    
    [Header("Status")]
    [SerializeField] private bool isARMode = false;
    
    void Awake()
    {
        // Check if XR Origin exists in scene (indicates AR mode)
        var xrOrigin = FindFirstObjectByType<XROrigin>();
        isARMode = (xrOrigin != null);
        
        if (isARMode)
        {
            Debug.Log("ARModeDetector: AR Mode detected - disabling incompatible components");
            
            // Disable components that don't work in AR
            foreach (var component in disableInAR)
            {
                if (component != null)
                {
                    component.enabled = false;
                    Debug.Log($"ARModeDetector: Disabled {component.GetType().Name}");
                }
            }
        }
        else
        {
            Debug.Log("ARModeDetector: Regular mode (no XR Origin found)");
        }
    }
    
    /// <summary>
    /// Check if currently in AR mode
    /// </summary>
    public bool IsARMode()
    {
        return isARMode;
    }
    
    /// <summary>
    /// Static method to check AR mode from any script
    /// </summary>
    public static bool CheckARMode()
    {
        return FindFirstObjectByType<XROrigin>() != null;
    }
}

