using UnityEngine;

public class SpacecraftInitialVelocity : MonoBehaviour
{
    [Header("Initial Position (Optional)")]
    [Tooltip("Set starting position (leave at 0,0,0 to use current position)")]
    public Vector3 initialPosition = new Vector3(150, 0, 0);
    
    [Tooltip("Apply initial position on start")]
    public bool setInitialPosition = true;
    
    [Header("Initial Orbital Velocity")]
    [Tooltip("Velocity in Unity units per second - Type values directly, don't use arrows!")]
    [Space(5)]
    public float velocityX = 0f;
    public float velocityY = 0f;
    public float velocityZ = 0.03f;
    
    [Space(10)]
    [Tooltip("Quick presets for common orbits")]
    public VelocityPreset preset = VelocityPreset.EarthOrbit;
    
    public enum VelocityPreset
    {
        Custom,
        EarthOrbit,   // 0.03 in Z
        MarsOrbit,    // 0.024 in Z
        FastEscape,   // 0.05 in Z
        Stationary    // 0
    }
    
    [Header("Debug")]
    [Tooltip("Show velocity updates in console")]
    public bool debugMode = true;
    
    void Start()
    {
        Debug.Log($"SpacecraftInitialVelocity: Start() called on {gameObject.name}");
        
        // Apply preset if not custom
        ApplyPreset();
        
        // Reset position if requested
        if (setInitialPosition && initialPosition != Vector3.zero)
        {
            transform.position = initialPosition;
            Debug.Log($"✓ Spacecraft '{gameObject.name}' position set to {initialPosition}");
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reset any existing velocity first
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Build velocity vector from components
            Vector3 initialVelocity = new Vector3(velocityX, velocityY, velocityZ);
            
            // Set initial velocity
            rb.linearVelocity = initialVelocity;
            Debug.Log($"✓ Spacecraft '{gameObject.name}' initial velocity set to {initialVelocity}");
            Debug.Log($"  Components: X={velocityX:F6}, Y={velocityY:F6}, Z={velocityZ:F6}");
            Debug.Log($"  Magnitude: {initialVelocity.magnitude:F6}");
            Debug.Log($"  Rigidbody mass: {rb.mass} kg, Use Gravity: {rb.useGravity}");
            Debug.Log($"  Tag: {gameObject.tag}");
            Debug.Log($"  Position: {transform.position}");
            
            // Warn if velocity is very small
            if (initialVelocity.magnitude < 0.001f)
            {
                Debug.LogWarning($"⚠️ Initial velocity is very small ({initialVelocity.magnitude:F6})! Spacecraft may fall into Sun!");
            }
        }
        else
        {
            Debug.LogError($"✗ Spacecraft '{gameObject.name}' has no Rigidbody component!");
        }
    }
    
    void ApplyPreset()
    {
        switch (preset)
        {
            case VelocityPreset.EarthOrbit:
                velocityX = 0f;
                velocityY = 0f;
                velocityZ = 0.03f;
                Debug.Log("Applied preset: Earth Orbit (0, 0, 0.03)");
                break;
            case VelocityPreset.MarsOrbit:
                velocityX = 0f;
                velocityY = 0f;
                velocityZ = 0.024f;
                Debug.Log("Applied preset: Mars Orbit (0, 0, 0.024)");
                break;
            case VelocityPreset.FastEscape:
                velocityX = 0f;
                velocityY = 0f;
                velocityZ = 0.05f;
                Debug.Log("Applied preset: Fast Escape (0, 0, 0.05)");
                break;
            case VelocityPreset.Stationary:
                velocityX = 0f;
                velocityY = 0f;
                velocityZ = 0f;
                Debug.Log("Applied preset: Stationary (0, 0, 0)");
                break;
            case VelocityPreset.Custom:
                // Use whatever values are already set
                Debug.Log($"Using custom velocity: ({velocityX:F6}, {velocityY:F6}, {velocityZ:F6})");
                break;
        }
    }
    
    void FixedUpdate()
    {
        if (debugMode)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && Time.frameCount % 100 == 0) // Log every 100 frames
            {
                Debug.Log($"Spacecraft '{gameObject.name}' velocity: {rb.linearVelocity}, position: {transform.position}");
            }
        }
    }
}