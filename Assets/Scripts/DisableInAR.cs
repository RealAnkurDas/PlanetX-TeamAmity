using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Simple script that disables this GameObject in AR mode
/// Use this on trails, text objects, or other components that don't work well in AR
/// </summary>
public class DisableInAR : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("What to disable: GameObject or just specific components")]
    [SerializeField] private bool disableGameObject = true;
    
    [Tooltip("If not disabling GameObject, disable these components")]
    [SerializeField] private MonoBehaviour[] componentsToDisable;
    
    void Awake()
    {
        // Check if XR Origin exists (indicates AR mode)
        bool isARMode = (FindFirstObjectByType<XROrigin>() != null);
        
        if (isARMode)
        {
            if (disableGameObject)
            {
                Debug.Log($"DisableInAR: Disabling GameObject '{gameObject.name}' in AR mode");
                gameObject.SetActive(false);
            }
            else
            {
                // Disable specific components
                foreach (var component in componentsToDisable)
                {
                    if (component != null)
                    {
                        component.enabled = false;
                        Debug.Log($"DisableInAR: Disabled {component.GetType().Name} on '{gameObject.name}'");
                    }
                }
            }
        }
    }
}

