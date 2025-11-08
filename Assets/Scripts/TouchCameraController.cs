using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object to orbit around (Sun by default)")]
    [SerializeField] private Transform target;
    
    [Tooltip("Auto-find Sun if target not set")]
    [SerializeField] private bool autoFindSun = true;
    
    [Header("Rotation Settings")]
    [Tooltip("Speed of camera rotation when dragging")]
    [SerializeField] private float rotationSpeed = 0.5f;
    
    [Tooltip("Smooth the rotation")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    
    [Header("Zoom Settings")]
    [Tooltip("Speed of zoom in/out")]
    [SerializeField] private float zoomSpeed = 0.5f;
    
    [Tooltip("Minimum distance from target")]
    [SerializeField] private float minZoomDistance = 10f;
    
    [Tooltip("Maximum distance from target")]
    [SerializeField] private float maxZoomDistance = 1000f;
    
    [Tooltip("Smooth the zoom")]
    [SerializeField] private float zoomSmoothTime = 0.1f;
    
    [Header("Desktop Controls (Optional)")]
    [Tooltip("Enable mouse controls for testing in editor")]
    [SerializeField] private bool enableMouseControls = true;
    
    [Tooltip("Mouse scroll zoom speed")]
    [SerializeField] private float mouseZoomSpeed = 10f;
    
    [Header("Field of View Settings")]
    [Tooltip("Default field of view")]
    [SerializeField] private float defaultFOV = 60f;
    
    [Tooltip("Smooth FOV transitions")]
    [SerializeField] private float fovSmoothTime = 0.3f;
    
    // Private variables
    private float currentDistance;
    private float targetDistance;
    private float zoomVelocity;
    
    private Camera cam;
    private float currentFOV;
    private float targetFOV;
    private float fovVelocity;
    
    private Vector2 rotationVelocity;
    private Vector2 currentRotation;
    private Vector2 targetRotation;
    
    private Vector2 lastTouchPosition;
    private float lastPinchDistance;
    
    void OnEnable()
    {
        // Enable Enhanced Touch for new Input System
        EnhancedTouchSupport.Enable();
    }
    
    void OnDisable()
    {
        // Disable Enhanced Touch when not needed
        EnhancedTouchSupport.Disable();
    }
    
    void Start()
    {
        // Get camera component
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            currentFOV = cam.fieldOfView;
            targetFOV = currentFOV;
            defaultFOV = currentFOV; // Store initial FOV as default
        }
        
        // Auto-find Sun if target not set
        if (target == null && autoFindSun)
        {
            GameObject sun = GameObject.Find("Sun Sphere");
            if (sun == null) sun = GameObject.Find("Sun");
            
            if (sun != null)
            {
                target = sun.transform;
                Debug.Log($"TouchCameraController: Auto-found target - {sun.name}");
            }
            else
            {
                Debug.LogWarning("TouchCameraController: Could not find Sun! Please assign target manually.");
            }
        }
        
        if (target != null)
        {
            // Initialize distance
            currentDistance = Vector3.Distance(transform.position, target.position);
            targetDistance = currentDistance;
            
            // Initialize rotation based on current camera orientation
            Vector3 angles = transform.eulerAngles;
            currentRotation = new Vector2(angles.y, angles.x);
            targetRotation = currentRotation;
        }
    }
    
    void Update()
    {
        if (target == null) return;
        
        // Handle touch input (mobile)
        if (Touch.activeTouches.Count > 0)
        {
            HandleTouchInput();
        }
        // Handle mouse input (desktop testing)
        else if (enableMouseControls)
        {
            HandleMouseInput();
        }
        
        // Smooth zoom
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, zoomSmoothTime);
        currentDistance = Mathf.Clamp(currentDistance, minZoomDistance, maxZoomDistance);
        
        // Smooth rotation
        currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, targetRotation.x, ref rotationVelocity.x, rotationSmoothTime);
        currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, targetRotation.y, ref rotationVelocity.y, rotationSmoothTime);
        
        // Clamp vertical rotation to avoid flipping
        currentRotation.y = Mathf.Clamp(currentRotation.y, -89f, 89f);
        
        // Smooth FOV
        if (cam != null)
        {
            currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref fovVelocity, fovSmoothTime);
            cam.fieldOfView = currentFOV;
        }
        
        // Apply rotation and position
        UpdateCameraTransform();
    }
    
    void HandleTouchInput()
    {
        var touches = Touch.activeTouches;
        
        // Two finger pinch - Zoom
        if (touches.Count == 2)
        {
            Touch touch0 = touches[0];
            Touch touch1 = touches[1];
            
            // Calculate pinch distance
            float currentPinchDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
            
            if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved || 
                touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                // Calculate pinch delta
                float pinchDelta = currentPinchDistance - lastPinchDistance;
                
                // Adjust zoom based on pinch
                targetDistance -= pinchDelta * zoomSpeed * Time.deltaTime;
                targetDistance = Mathf.Clamp(targetDistance, minZoomDistance, maxZoomDistance);
            }
            
            lastPinchDistance = currentPinchDistance;
        }
        // Single finger drag - Rotate
        else if (touches.Count == 1)
        {
            Touch touch = touches[0];
            
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = touch.delta;
                
                // Update target rotation
                targetRotation.x += delta.x * rotationSpeed;
                targetRotation.y -= delta.y * rotationSpeed;
                
                // Clamp vertical rotation
                targetRotation.y = Mathf.Clamp(targetRotation.y, -89f, 89f);
            }
        }
    }
    
    void HandleMouseInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;
        
        // Mouse drag - Rotate
        if (mouse.leftButton.isPressed)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            
            targetRotation.x += mouseDelta.x * rotationSpeed * Time.deltaTime;
            targetRotation.y -= mouseDelta.y * rotationSpeed * Time.deltaTime;
            
            // Clamp vertical rotation
            targetRotation.y = Mathf.Clamp(targetRotation.y, -89f, 89f);
        }
        
        // Mouse scroll - Zoom
        Vector2 scrollDelta = mouse.scroll.ReadValue();
        float scroll = scrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * mouseZoomSpeed * 0.01f; // Adjust scale
            targetDistance = Mathf.Clamp(targetDistance, minZoomDistance, maxZoomDistance);
        }
    }
    
    void UpdateCameraTransform()
    {
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        
        // Calculate position (orbit around target)
        Vector3 direction = rotation * Vector3.back; // -forward
        Vector3 position = target.position + direction * currentDistance;
        
        // Apply to camera
        transform.position = position;
        transform.LookAt(target);
    }
    
    // Public methods for external control
    
    /// <summary>
    /// Set a new target to orbit around
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            targetDistance = Vector3.Distance(transform.position, target.position);
            currentDistance = targetDistance;
        }
    }
    
    /// <summary>
    /// Set zoom distance
    /// </summary>
    public void SetZoomDistance(float distance)
    {
        targetDistance = Mathf.Clamp(distance, minZoomDistance, maxZoomDistance);
    }
    
    /// <summary>
    /// Reset camera to initial position
    /// </summary>
    public void ResetCamera()
    {
        if (target != null)
        {
            currentRotation = Vector2.zero;
            targetRotation = Vector2.zero;
            UpdateCameraTransform();
        }
    }
    
    /// <summary>
    /// Focus on a specific planet
    /// </summary>
    public void FocusOnObject(Transform obj, float distance = 0f)
    {
        SetTarget(obj);
        
        if (distance > 0f)
        {
            SetZoomDistance(distance);
        }
        else
        {
            // Auto-calculate distance based on object scale
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                float objectSize = renderer.bounds.size.magnitude;
                SetZoomDistance(objectSize * 3f);
            }
        }
    }
    
    /// <summary>
    /// Set camera field of view
    /// </summary>
    public void SetFieldOfView(float fov)
    {
        targetFOV = Mathf.Clamp(fov, 1f, 179f);
    }
    
    /// <summary>
    /// Reset field of view to default
    /// </summary>
    public void ResetFieldOfView()
    {
        targetFOV = defaultFOV;
    }
    
    /// <summary>
    /// Get current field of view
    /// </summary>
    public float GetFieldOfView()
    {
        return cam != null ? cam.fieldOfView : defaultFOV;
    }
}