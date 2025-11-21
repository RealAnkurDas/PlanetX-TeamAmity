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
    
    [Tooltip("Minimum distance from target (can be overridden per target)")]
    [SerializeField] private float minZoomDistance = 10f;
    
    [Tooltip("Maximum distance from target")]
    [SerializeField] private float maxZoomDistance = 1000f;
    
    [Tooltip("Smooth the zoom")]
    [SerializeField] private float zoomSmoothTime = 0.1f;
    
    // Dynamic minimum zoom for current target
    private float dynamicMinZoom = -1f; // -1 means use default minZoomDistance
    
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
    private bool useManualPositioning = false; // Flag to skip auto-update when manually positioned
    
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
        
        // Debug: Log when input is being processed
        bool hasInput = false;
        
        // Handle touch input (mobile)
        if (Touch.activeTouches.Count > 0)
        {
            hasInput = true;
            HandleTouchInput();
        }
        // Handle mouse input (desktop testing)
        else if (enableMouseControls)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null && (mouse.leftButton.isPressed || Mathf.Abs(mouse.scroll.ReadValue().y) > 0.01f))
            {
                hasInput = true;
                HandleMouseInput();
            }
        }
        
        // Debug when camera is being moved by input
        if (hasInput && Time.frameCount % 30 == 0)
        {
            Debug.Log($"TouchCameraController: Processing input - Rotation: H={currentRotation.x:F1}, V={currentRotation.y:F1}, Distance={currentDistance:F2}");
        }
        
        // Smooth zoom
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, zoomSmoothTime);
        
        // Use dynamic min zoom if set, otherwise use default
        float effectiveMinZoom = (dynamicMinZoom > 0f) ? dynamicMinZoom : minZoomDistance;
        currentDistance = Mathf.Clamp(currentDistance, effectiveMinZoom, maxZoomDistance);
        
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
        // Clear manual positioning flag when user starts interacting
        useManualPositioning = false;
        
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
                
                // Use dynamic min zoom if set, otherwise use default
                float effectiveMinZoom = (dynamicMinZoom > 0f) ? dynamicMinZoom : minZoomDistance;
                targetDistance = Mathf.Clamp(targetDistance, effectiveMinZoom, maxZoomDistance);
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
        
        // Clear manual positioning flag when user starts interacting
        if (mouse.leftButton.isPressed || Mathf.Abs(mouse.scroll.ReadValue().y) > 0.01f)
        {
            if (useManualPositioning)
            {
                Debug.Log($"★ CLICK DETECTED - Unlocking manual positioning");
                Debug.Log($"  Current camera Y: {transform.position.y:F3}");
                Debug.Log($"  Stored angles - H: {currentRotation.x:F1}°, V: {currentRotation.y:F1}°");
                useManualPositioning = false;
            }
        }
        
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
            
            // Use dynamic min zoom if set, otherwise use default
            float effectiveMinZoom = (dynamicMinZoom > 0f) ? dynamicMinZoom : minZoomDistance;
            targetDistance = Mathf.Clamp(targetDistance, effectiveMinZoom, maxZoomDistance);
        }
    }
    
    void UpdateCameraTransform()
    {
        // Skip auto-update if manually positioned (wait for user input to reset)
        if (useManualPositioning)
        {
            return;
        }
        
        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        
        // Calculate position (orbit around target)
        Vector3 direction = rotation * Vector3.back; // -forward
        Vector3 position = target.position + direction * currentDistance;
        
        // Debug on first frame after unlocking
        if (Time.frameCount % 120 == 0) // Log every 2 seconds
        {
            Debug.Log($"UpdateCameraTransform: New Y: {position.y:F3}, Angles H:{currentRotation.x:F1}° V:{currentRotation.y:F1}°");
        }
        
        // Apply to camera
        transform.position = position;
        transform.LookAt(target.position);
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
        // Use dynamic min zoom if set, otherwise use default
        float effectiveMinZoom = (dynamicMinZoom > 0f) ? dynamicMinZoom : minZoomDistance;
        targetDistance = Mathf.Clamp(distance, effectiveMinZoom, maxZoomDistance);
    }
    
    /// <summary>
    /// Set minimum zoom distance for current target (prevents zooming past it)
    /// </summary>
    public void SetMinimumZoomDistance(float minDistance)
    {
        dynamicMinZoom = minDistance;
        
        // Also clamp current distance to respect new minimum
        float effectiveMinZoom = (dynamicMinZoom > 0f) ? dynamicMinZoom : minZoomDistance;
        targetDistance = Mathf.Max(targetDistance, effectiveMinZoom);
        currentDistance = Mathf.Max(currentDistance, effectiveMinZoom);
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
    
    /// <summary>
    /// Set camera to exact position and rotation, syncing internal state
    /// Used for manual positioning of camera views
    /// </summary>
    public void SetCameraTransform(Vector3 position, Vector3 eulerRotation)
    {
        // Set manual positioning flag to prevent UpdateCameraTransform from running
        useManualPositioning = true;
        
        // Set the transform directly
        transform.position = position;
        transform.rotation = Quaternion.Euler(eulerRotation);
        
        // Calculate and sync internal rotation variables
        if (target != null)
        {
            // Calculate current distance from new position to target
            currentDistance = Vector3.Distance(position, target.position);
            targetDistance = currentDistance;
            
            // Calculate rotation angles by REVERSING what UpdateCameraTransform() does
            // UpdateCameraTransform uses: Euler(currentRotation.y, currentRotation.x, 0) * Vector3.back
            // Vector3.back = (0, 0, -1), so we need the INVERSE direction
            
            // Get the direction from CAMERA TO TARGET (inverse of what we want)
            // Because Vector3.back points backward, we need backward-facing angles
            Vector3 directionToCamera = (target.position - position).normalized;
            
            // Extract horizontal angle (around Y-axis, stored in currentRotation.x)
            // This is the angle in the XZ plane
            float horizontalAngle = Mathf.Atan2(directionToCamera.x, directionToCamera.z) * Mathf.Rad2Deg;
            
            // Extract vertical angle (pitch, stored in currentRotation.y)
            // This is the angle from the horizontal plane (XZ) to the direction
            float verticalAngle = Mathf.Asin(directionToCamera.y) * Mathf.Rad2Deg;
            
            // Store in internal rotation format
            currentRotation.x = horizontalAngle; // Horizontal (yaw around Y-axis)
            currentRotation.y = verticalAngle;   // Vertical (pitch up/down)
            targetRotation = currentRotation;
            
            // Verify by calculating what position these angles would produce
            Quaternion testRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
            Vector3 testDirection = testRotation * Vector3.back;
            Vector3 testPosition = target.position + testDirection * currentDistance;
            
            Debug.Log($"TouchCameraController: Manual position set - Pos: {position}, Rot: {eulerRotation}");
            Debug.Log($"  Direction: {directionToCamera}, Distance: {currentDistance:F2}");
            Debug.Log($"  Calculated angles - H: {horizontalAngle:F1}°, V: {verticalAngle:F1}°");
            Debug.Log($"  Verification: Test position {testPosition}, Diff: {Vector3.Distance(position, testPosition):F3}");
            Debug.Log($"  Manual positioning flag SET");
        }
    }
}