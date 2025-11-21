using UnityEngine;

/// <summary>
/// Diagnostic tool to check if spacecraft is properly set up for gravity simulation
/// Attach to MBR Explorer and check console for status
/// </summary>
public class GravitySimulationDiagnostic : MonoBehaviour
{
    [Header("Check Interval")]
    [SerializeField] private float checkInterval = 2f; // Check every 2 seconds
    
    private float timeSinceLastCheck = 0f;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private EphemerisBasedSimulation simulation;
    
    void Start()
    {
        lastPosition = transform.position;
        
        simulation = FindFirstObjectByType<EphemerisBasedSimulation>();
        
        // Initial check
        Invoke(nameof(RunDiagnostic), 1f); // Check after 1 second
    }
    
    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        
        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f;
            RunDiagnostic();
        }
    }
    
    void RunDiagnostic()
    {
        Debug.Log("========================================");
        Debug.Log("🔍 GRAVITY SIMULATION DIAGNOSTIC");
        Debug.Log($"   Spacecraft: {gameObject.name}");
        
        // Check 1: Tag
        if (gameObject.tag == "SmallBody")
        {
            Debug.Log("   ✅ Tag: SmallBody (correct!)");
        }
        else
        {
            Debug.LogError($"   ❌ Tag: {gameObject.tag} (WRONG! Should be 'SmallBody')");
        }
        
        // Check 2: Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log("   ✅ Rigidbody: Present");
            Debug.Log($"      Mass: {rb.mass} kg");
            Debug.Log($"      Use Gravity: {rb.useGravity} (should be FALSE)");
            Debug.Log($"      Is Kinematic: {rb.isKinematic} (should be FALSE)");
            Debug.Log($"      Velocity: {rb.linearVelocity}");
            Debug.Log($"      Speed: {rb.linearVelocity.magnitude:F6} m/s");
            
            if (rb.useGravity)
            {
                Debug.LogError("   ❌ Use Gravity is ON! Turn it OFF for N-body simulation!");
            }
            else
            {
                Debug.Log("   ✅ Use Gravity: OFF (correct!)");
            }
            
            if (rb.linearVelocity.magnitude < 0.001f)
            {
                Debug.LogWarning("   ⚠️ Velocity is nearly ZERO! Spacecraft will fall into Sun!");
            }
            else
            {
                Debug.Log($"   ✅ Velocity: {rb.linearVelocity.magnitude:F6} m/s (good!)");
            }
            
            lastVelocity = rb.linearVelocity;
        }
        else
        {
            Debug.LogError("   ❌ NO RIGIDBODY! Gravity simulation won't work!");
        }
        
        // Check 3: Position change
        Vector3 positionChange = transform.position - lastPosition;
        float distanceMoved = positionChange.magnitude;
        
        if (distanceMoved > 0.01f)
        {
            Debug.Log($"   ✅ MOVING: Moved {distanceMoved:F3} units since last check");
            Debug.Log($"      Direction: {positionChange.normalized}");
        }
        else if (distanceMoved > 0.0001f)
        {
            Debug.Log($"   ⚠️ SLOW MOVEMENT: Only {distanceMoved:F6} units");
        }
        else
        {
            Debug.LogWarning("   ❌ NOT MOVING! Position unchanged!");
        }
        
        lastPosition = transform.position;
        
        // Check 4: Simulation status
        if (simulation != null)
        {
            Debug.Log("   ✅ EphemerisBasedSimulation: Found");
            // Can't access isRunning directly (it's private), but if it exists we're good
        }
        else
        {
            Debug.LogError("   ❌ EphemerisBasedSimulation: NOT FOUND!");
        }
        
        // Check 5: Current position
        Debug.Log($"   Position: {transform.position}");
        float distanceFromSun = transform.position.magnitude;
        Debug.Log($"   Distance from Sun: {distanceFromSun:F2} Unity units");
        
        Debug.Log("========================================");
    }
}

