using UnityEngine;
using CosineKitty;
using System;
using System.Reflection;

/// <summary>
/// Receives launch parameters from Kotlin via Intent extras and applies them to spacecraft
/// Used for user-controlled trajectory simulations
/// Positions spacecraft based on MBR Explorer ephemeris at simulation start date
/// </summary>
public class KotlinLaunchReceiver : MonoBehaviour
{
    [Header("Spacecraft Reference")]
    [SerializeField] private GameObject spacecraft;
    
    [Header("Launch Mode")]
    [Tooltip("Choose how to interpret launch parameters")]
    [SerializeField] private LaunchMode launchMode = LaunchMode.DirectVelocity;
    
    [Header("Default Values - Angle & Speed Mode (Editor Testing)")]
    [Tooltip("Launch speed (m/s in Unity scale)")]
    [SerializeField] private float defaultLaunchSpeed = 0.032f;  // ~Earth orbital velocity
    
    [Tooltip("Horizontal angle - 0°=forward, 90°=right, 180°=back, 270°=left")]
    [SerializeField] private float defaultHorizontalAngle = 0f;
    
    [Tooltip("Vertical angle - 0°=horizontal, 90°=straight up, -90°=straight down")]
    [SerializeField] private float defaultVerticalAngle = 0f;
    
    [Header("Default Values - Direct Velocity Mode")]
    [Tooltip("Default velocity X (m/s in Unity scale)")]
    [SerializeField] private float defaultVelocityX = 0.01f;
    
    [Tooltip("Default velocity Y (m/s in Unity scale)")]
    [SerializeField] private float defaultVelocityY = 0.0f;
    
    [Tooltip("Default velocity Z (m/s in Unity scale)")]
    [SerializeField] private float defaultVelocityZ = 0.03f;
    
    [Header("Random Velocity Settings (User Scene)")]
    [Tooltip("Enable random velocity on launch (when no values received from Kotlin)")]
    [SerializeField] private bool useRandomVelocity = true;
    
    [Tooltip("Random velocity X range (min, max)")]
    [SerializeField] private Vector2 randomVelocityXRange = new Vector2(-1e-5f, 1e-5f);
    
    [Tooltip("Random velocity Y range (min, max)")]
    [SerializeField] private Vector2 randomVelocityYRange = new Vector2(-1e-5f, 1e-5f);
    
    [Tooltip("Random velocity Z range (min, max)")]
    [SerializeField] private Vector2 randomVelocityZRange = new Vector2(1e-5f, 1e-4f);
    
    public enum LaunchMode
    {
        AngleAndSpeed,      // Use speed + horizontal angle + vertical angle
        DirectVelocity      // Use vx, vy, vz directly
    }
    
    [Header("Launch Position")]
    [Tooltip("Use MBR Explorer ephemeris position at simulation start date (recommended)")]
    [SerializeField] private bool useMBREphemerisPosition = true;
    
    [Tooltip("Automatically position spacecraft at Earth's location (fallback if ephemeris not available)")]
    [SerializeField] private bool launchFromEarth = true;
    
    [Tooltip("Manual launch position (only if Launch From Earth and Ephemeris are disabled)")]
    [SerializeField] private Vector3 customLaunchPosition = Vector3.zero;
    
    [Tooltip("Offset from MBR Explorer position (in Unity units)")]
    [SerializeField] private Vector3 mbrPositionOffset = Vector3.zero;
    
    [Header("Earth Reference Frame")]
    [Tooltip("Add Earth's orbital velocity as base (launch relative to Earth)")]
    [SerializeField] private bool launchRelativeToEarth = true;
    
    [Tooltip("Earth's orbital velocity in Unity scale (m/s) - auto-updated if Earth found")]
    [SerializeField] private Vector3 earthOrbitalVelocity = new Vector3(0.01f, 0f, 0.03f);
    
    private GameObject earthObject;
    private EphemerisBasedSimulation simulationManager;
    private AsteroidEphemerisReader mbrEphemerisReader;
    
    // Constants for coordinate conversion
    private const double AU_TO_METRES = 149597870700.0;
    private const double KM_TO_METRES = 1000.0;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    void Start()
    {
        // Auto-find spacecraft if not assigned
        if (spacecraft == null)
        {
            spacecraft = GameObject.Find("MBR Explorer");
            if (spacecraft == null)
            {
                spacecraft = GameObject.FindGameObjectWithTag("SmallBody");
            }
            
            if (spacecraft != null)
            {
                Debug.Log($"KotlinLaunchReceiver: Auto-found spacecraft: {spacecraft.name}");
            }
            else
            {
                Debug.LogError("KotlinLaunchReceiver: No spacecraft found! Make sure MBR Explorer exists and has 'SmallBody' tag.");
                enabled = false;
                return;
            }
        }
        
        // Find simulation manager and MBR ephemeris reader
        if (useMBREphemerisPosition)
        {
            simulationManager = FindFirstObjectByType<EphemerisBasedSimulation>();
            if (simulationManager != null)
            {
                Debug.Log("KotlinLaunchReceiver: Found EphemerisBasedSimulation");
            }
            else
            {
                Debug.LogWarning("KotlinLaunchReceiver: EphemerisBasedSimulation not found! Falling back to Earth position.");
                useMBREphemerisPosition = false;
            }
            
            // Find MBR Explorer ephemeris reader
            AsteroidEphemerisReader[] allReaders = FindObjectsByType<AsteroidEphemerisReader>(FindObjectsSortMode.None);
            foreach (AsteroidEphemerisReader reader in allReaders)
            {
                if (reader != null && reader.asteroidName.Equals("MBR Explorer", StringComparison.OrdinalIgnoreCase))
                {
                    mbrEphemerisReader = reader;
                    Debug.Log($"KotlinLaunchReceiver: Found MBR Explorer ephemeris reader");
                    break;
                }
            }
            
            if (mbrEphemerisReader == null)
            {
                Debug.LogWarning("KotlinLaunchReceiver: MBR Explorer ephemeris reader not found! Falling back to Earth position.");
                useMBREphemerisPosition = false;
            }
        }
        
        // Find Earth if launching from Earth (fallback)
        if (launchFromEarth || (!useMBREphemerisPosition && launchFromEarth))
        {
            earthObject = GameObject.Find("Earth");
            if (earthObject != null)
            {
                Debug.Log($"KotlinLaunchReceiver: Found Earth at {earthObject.transform.position}");
                
                // Get Earth's actual velocity if it has a Rigidbody
                Rigidbody earthRb = earthObject.GetComponent<Rigidbody>();
                if (earthRb != null && earthRb.linearVelocity.magnitude > 0.001f)
                {
                    earthOrbitalVelocity = earthRb.linearVelocity;
                    Debug.Log($"KotlinLaunchReceiver: Using Earth's actual velocity: {earthOrbitalVelocity}");
                }
            }
            else
            {
                Debug.LogWarning("KotlinLaunchReceiver: Earth not found! Using custom launch position instead.");
                launchFromEarth = false;
            }
        }
        
        // Read Intent parameters from Kotlin
        #if UNITY_ANDROID && !UNITY_EDITOR
        ReadIntentParameters();
        #else
        // In Editor, use random or default values for testing
        Debug.Log($"KotlinLaunchReceiver: Running in Editor, using {(useRandomVelocity ? "random" : "default")} values (Mode: {launchMode})");
        
        if (launchMode == LaunchMode.AngleAndSpeed)
        {
            if (useRandomVelocity)
            {
                // Generate random speed and angles
                float randomSpeed = UnityEngine.Random.Range(1e-5f, 1e-4f);
                float randomHAngle = UnityEngine.Random.Range(0f, 360f);
                float randomVAngle = UnityEngine.Random.Range(-45f, 45f);
                ApplyParametersAngleMode(randomSpeed, randomHAngle, randomVAngle);
            }
            else
            {
                ApplyParametersAngleMode(defaultLaunchSpeed, defaultHorizontalAngle, defaultVerticalAngle);
            }
        }
        else
        {
            if (useRandomVelocity)
            {
                // Generate random velocity values
                float randomVx = UnityEngine.Random.Range(randomVelocityXRange.x, randomVelocityXRange.y);
                float randomVy = UnityEngine.Random.Range(randomVelocityYRange.x, randomVelocityYRange.y);
                float randomVz = UnityEngine.Random.Range(randomVelocityZRange.x, randomVelocityZRange.y);
                Debug.Log($"🎲 Generated random velocity: X={randomVx:F6}, Y={randomVy:F6}, Z={randomVz:F6}");
                ApplyParametersVelocityMode(randomVx, randomVy, randomVz);
            }
            else
            {
                ApplyParametersVelocityMode(defaultVelocityX, defaultVelocityY, defaultVelocityZ);
            }
        }
        #endif
    }
    
    void ReadIntentParameters()
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent"))
            {
                // Determine launch mode from Kotlin
                string modeStr = intent.Call<string>("getStringExtra", "launchMode");
                bool useAngleMode = (modeStr == "angleAndSpeed");
                
                if (useAngleMode)
                {
                    // Angle + Speed mode
                    float speed = intent.Call<float>("getFloatExtra", "launchSpeed", float.MinValue);
                    float horizontalAngle = intent.Call<float>("getFloatExtra", "horizontalAngle", float.MinValue);
                    float verticalAngle = intent.Call<float>("getFloatExtra", "verticalAngle", float.MinValue);
                    
                    // Check if values were actually provided (not defaults)
                    if (speed != float.MinValue && horizontalAngle != float.MinValue && verticalAngle != float.MinValue)
                    {
                        Debug.Log($"✅ Received from Kotlin (Angle Mode): speed={speed:F6}, horizontal={horizontalAngle}°, vertical={verticalAngle}°");
                        ApplyParametersAngleMode(speed, horizontalAngle, verticalAngle);
                    }
                    else if (useRandomVelocity)
                    {
                        // No values from Kotlin, use random
                        float randomSpeed = UnityEngine.Random.Range(1e-5f, 1e-4f);
                        float randomHAngle = UnityEngine.Random.Range(0f, 360f);
                        float randomVAngle = UnityEngine.Random.Range(-45f, 45f);
                        Debug.Log($"🎲 No Kotlin values - Using random (Angle Mode): speed={randomSpeed:F6}, horizontal={randomHAngle}°, vertical={randomVAngle}°");
                        ApplyParametersAngleMode(randomSpeed, randomHAngle, randomVAngle);
                    }
                    else
                    {
                        // Use defaults
                        ApplyParametersAngleMode(defaultLaunchSpeed, defaultHorizontalAngle, defaultVerticalAngle);
                    }
                }
                else
                {
                    // Direct velocity mode
                    float vx = intent.Call<float>("getFloatExtra", "velocityX", float.MinValue);
                    float vy = intent.Call<float>("getFloatExtra", "velocityY", float.MinValue);
                    float vz = intent.Call<float>("getFloatExtra", "velocityZ", float.MinValue);
                    
                    // Check if values were actually provided (not defaults)
                    if (vx != float.MinValue && vy != float.MinValue && vz != float.MinValue)
                    {
                        Debug.Log($"✅ Received from Kotlin (Velocity Mode): vx={vx:F6}, vy={vy:F6}, vz={vz:F6}");
                        ApplyParametersVelocityMode(vx, vy, vz);
                    }
                    else if (useRandomVelocity)
                    {
                        // No values from Kotlin, use random
                        float randomVx = UnityEngine.Random.Range(randomVelocityXRange.x, randomVelocityXRange.y);
                        float randomVy = UnityEngine.Random.Range(randomVelocityYRange.x, randomVelocityYRange.y);
                        float randomVz = UnityEngine.Random.Range(randomVelocityZRange.x, randomVelocityZRange.y);
                        Debug.Log($"🎲 No Kotlin values - Using random velocity: X={randomVx:F6}, Y={randomVy:F6}, Z={randomVz:F6}");
                        ApplyParametersVelocityMode(randomVx, randomVy, randomVz);
                    }
                    else
                    {
                        // Use defaults
                        ApplyParametersVelocityMode(defaultVelocityX, defaultVelocityY, defaultVelocityZ);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"KotlinLaunchReceiver: Error reading Intent: {e.Message}");
            Debug.Log($"Falling back to {(useRandomVelocity ? "random" : "default")} values");
            
            if (launchMode == LaunchMode.AngleAndSpeed)
            {
                if (useRandomVelocity)
                {
                    float randomSpeed = UnityEngine.Random.Range(1e-5f, 1e-4f);
                    float randomHAngle = UnityEngine.Random.Range(0f, 360f);
                    float randomVAngle = UnityEngine.Random.Range(-45f, 45f);
                    ApplyParametersAngleMode(randomSpeed, randomHAngle, randomVAngle);
                }
                else
                {
                    ApplyParametersAngleMode(defaultLaunchSpeed, defaultHorizontalAngle, defaultVerticalAngle);
                }
            }
            else
            {
                if (useRandomVelocity)
                {
                    float randomVx = UnityEngine.Random.Range(randomVelocityXRange.x, randomVelocityXRange.y);
                    float randomVy = UnityEngine.Random.Range(randomVelocityYRange.x, randomVelocityYRange.y);
                    float randomVz = UnityEngine.Random.Range(randomVelocityZRange.x, randomVelocityZRange.y);
                    ApplyParametersVelocityMode(randomVx, randomVy, randomVz);
                }
                else
                {
                    ApplyParametersVelocityMode(defaultVelocityX, defaultVelocityY, defaultVelocityZ);
                }
            }
        }
    }
    
    /// <summary>
    /// Apply launch parameters using Speed + Angles (intuitive mode)
    /// Launch plane is perpendicular to Earth's orbital motion (XY plane if Earth moves in Z)
    /// </summary>
    void ApplyParametersAngleMode(float speed, float horizontalAngle, float verticalAngle)
    {
        if (spacecraft == null)
        {
            Debug.LogError("KotlinLaunchReceiver: Spacecraft is null!");
            return;
        }
        
        // Convert angles to radians
        float hRadians = horizontalAngle * Mathf.Deg2Rad;
        float vRadians = verticalAngle * Mathf.Deg2Rad;
        
        // Calculate launch velocity in Earth's reference frame
        // Launch plane is XY (perpendicular to Earth's Z motion)
        // Horizontal angle: rotation in XY plane (0° = +X, 90° = +Y, 180° = -X, 270° = -Y)
        // Vertical angle: tilt toward/away from Z (0° = in plane, +90° = along +Z, -90° = along -Z)
        
        float planarSpeed = speed * Mathf.Cos(vRadians);     // Speed in launch plane (XY)
        float launchVx = planarSpeed * Mathf.Cos(hRadians);  // X component in launch plane
        float launchVy = planarSpeed * Mathf.Sin(hRadians);  // Y component in launch plane
        float launchVz = speed * Mathf.Sin(vRadians);        // Z component (perpendicular to plane)
        
        // Add Earth's orbital velocity if launching relative to Earth
        float finalVx, finalVy, finalVz;
        if (launchRelativeToEarth)
        {
            finalVx = earthOrbitalVelocity.x + launchVx;
            finalVy = earthOrbitalVelocity.y + launchVy;
            finalVz = earthOrbitalVelocity.z + launchVz;
            
            if (showDebugInfo)
            {
                Debug.Log("========================================");
                Debug.Log("🚀 ANGLE & SPEED MODE (Earth Reference Frame)");
                Debug.Log($"   Launch plane: Perpendicular to Earth's motion");
                Debug.Log($"   Input: Speed={speed:F6}, H={horizontalAngle}°, V={verticalAngle}°");
                Debug.Log($"   Launch velocity (relative): ({launchVx:F6}, {launchVy:F6}, {launchVz:F6})");
                Debug.Log($"   Earth orbital velocity: {earthOrbitalVelocity}");
                Debug.Log($"   Final velocity (absolute): ({finalVx:F6}, {finalVy:F6}, {finalVz:F6})");
                Debug.Log($"   Final speed: {Mathf.Sqrt(finalVx*finalVx + finalVy*finalVy + finalVz*finalVz):F6} m/s");
                Debug.Log("========================================");
            }
        }
        else
        {
            finalVx = launchVx;
            finalVy = launchVy;
            finalVz = launchVz;
            
            if (showDebugInfo)
            {
                Debug.Log("========================================");
                Debug.Log("🚀 ANGLE & SPEED MODE (Absolute Frame)");
                Debug.Log($"   Input: Speed={speed:F6}, H={horizontalAngle}°, V={verticalAngle}°");
                Debug.Log($"   Velocity: ({finalVx:F6}, {finalVy:F6}, {finalVz:F6})");
                Debug.Log("========================================");
            }
        }
        
        // Apply the calculated velocity
        ApplyVelocity(finalVx, finalVy, finalVz, horizontalAngle, verticalAngle);
    }
    
    /// <summary>
    /// Apply launch parameters using direct velocity components (advanced mode)
    /// </summary>
    void ApplyParametersVelocityMode(float vx, float vy, float vz)
    {
        if (showDebugInfo)
        {
            Debug.Log("========================================");
            Debug.Log("🚀 DIRECT VELOCITY MODE");
            Debug.Log($"   Velocity: ({vx:F6}, {vy:F6}, {vz:F6})");
            Debug.Log("========================================");
        }
        
        ApplyVelocity(vx, vy, vz, 0f, 0f);
    }
    
    /// <summary>
    /// Common method to apply velocity to spacecraft
    /// </summary>
    void ApplyVelocity(float vx, float vy, float vz, float displayHAngle, float displayVAngle)
    {
        // Set launch position based on MBR Explorer ephemeris
        if (useMBREphemerisPosition && simulationManager != null && mbrEphemerisReader != null)
        {
            Vector3 mbrPosition = GetMBREphemerisPosition();
            if (mbrPosition != Vector3.zero)
            {
                spacecraft.transform.position = mbrPosition + mbrPositionOffset;
                if (showDebugInfo)
                {
                    Debug.Log($"✅ Launch position: MBR Explorer ephemeris at {spacecraft.transform.position}");
                    Debug.Log($"   (from MBR Explorer position: {mbrPosition}, offset: {mbrPositionOffset})");
                }
            }
            else
            {
                // Fallback to Earth if ephemeris position failed
                if (earthObject != null)
                {
                    spacecraft.transform.position = earthObject.transform.position;
                    if (showDebugInfo)
                    {
                        Debug.Log($"✅ Launch position: Earth (fallback) at {spacecraft.transform.position}");
                    }
                }
            }
        }
        else if (launchFromEarth && earthObject != null)
        {
            // Position spacecraft at Earth's current location
            spacecraft.transform.position = earthObject.transform.position;
            if (showDebugInfo)
            {
                Debug.Log($"✅ Launch position: Earth at {spacecraft.transform.position}");
            }
        }
        else if (customLaunchPosition != Vector3.zero)
        {
            // Use custom position
            spacecraft.transform.position = customLaunchPosition;
            if (showDebugInfo)
            {
                Debug.Log($"✅ Launch position: Custom at {customLaunchPosition}");
            }
        }
        else
        {
            // Keep spacecraft's current position
            if (showDebugInfo)
            {
                Debug.Log($"✅ Launch position: Current at {spacecraft.transform.position}");
            }
        }
        
        // Rotate spacecraft to match velocity direction
        Vector3 velocity = new Vector3(vx, vy, vz);
        if (velocity.magnitude > 0.001f)
        {
            spacecraft.transform.rotation = Quaternion.LookRotation(velocity);
        }
        
        // Apply velocity to Rigidbody
        Rigidbody rb = spacecraft.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reset first to clear any previous state
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Set new velocity
            rb.linearVelocity = velocity;
            
            if (showDebugInfo)
            {
                Debug.Log($"✅ Velocity applied: ({vx:F6}, {vy:F6}, {vz:F6})");
                Debug.Log($"   Speed (magnitude): {velocity.magnitude:F6} m/s");
                Debug.Log($"   Rigidbody mass: {rb.mass} kg");
                Debug.Log($"   Position: {spacecraft.transform.position}");
                Debug.Log($"   Rotation: {spacecraft.transform.rotation.eulerAngles}");
            }
        }
        else
        {
            Debug.LogError("KotlinLaunchReceiver: Spacecraft has no Rigidbody component!");
            return;
        }
        
        // Disable conflicting scripts
        SpacecraftInitialVelocity velocityScript = spacecraft.GetComponent<SpacecraftInitialVelocity>();
        if (velocityScript != null)
        {
            velocityScript.enabled = false;
            if (showDebugInfo)
            {
                Debug.Log("✅ SpacecraftInitialVelocity disabled");
            }
        }
        
        GATrajectoryFollower gaScript = spacecraft.GetComponent<GATrajectoryFollower>();
        if (gaScript != null)
        {
            gaScript.enabled = false;
            if (showDebugInfo)
            {
                Debug.Log("✅ GATrajectoryFollower disabled");
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log("========================================");
            Debug.Log("🚀 SPACECRAFT LAUNCH READY");
            Debug.Log($"   Name: {spacecraft.name}");
            Debug.Log($"   Tag: {spacecraft.tag}");
            Debug.Log("   ✅ Gravity simulation will now control trajectory");
            Debug.Log("========================================");
        }
    }
    
    /// <summary>
    /// Public method to change velocity during runtime (optional, for future use)
    /// </summary>
    public void UpdateVelocity(float vx, float vy, float vz)
    {
        if (spacecraft != null)
        {
            Rigidbody rb = spacecraft.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(vx, vy, vz);
                Debug.Log($"Velocity updated to: ({vx}, {vy}, {vz})");
            }
        }
    }
    
    /// <summary>
    /// Public method to change launch angle during runtime (optional, for future use)
    /// </summary>
    public void UpdateLaunchAngle(float angle)
    {
        if (spacecraft != null)
        {
            spacecraft.transform.rotation = Quaternion.Euler(0, angle, 0);
            Debug.Log($"Launch angle updated to: {angle}°");
        }
    }
    
    /// <summary>
    /// Get MBR Explorer position from ephemeris at simulation start date
    /// </summary>
    Vector3 GetMBREphemerisPosition()
    {
        try
        {
            // Get simulation start date
            if (simulationManager == null || mbrEphemerisReader == null)
            {
                return Vector3.zero;
            }
            
            // Access the start date from EphemerisBasedSimulation using reflection
            // Since startDateTime is private, we'll use a different approach
            // Get current simulation time from EphemerisBasedSimulation
            DateTime simulationDate = DateTime.UtcNow;
            
            // Try to get start date via reflection
            var startDateField = typeof(EphemerisBasedSimulation).GetField("startDateTime", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (startDateField != null)
            {
                simulationDate = (DateTime)startDateField.GetValue(simulationManager);
            }
            else
            {
                // Fallback: Use current simulation time from AstroTime
                var currentTimeField = typeof(EphemerisBasedSimulation).GetField("currentAstroTime",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (currentTimeField != null)
                {
                    AstroTime astroTime = (AstroTime)currentTimeField.GetValue(simulationManager);
                    if (astroTime != null)
                    {
                        simulationDate = astroTime.ToUtcDateTime();
                    }
                }
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"KotlinLaunchReceiver: Getting MBR Explorer position for date: {simulationDate:yyyy-MM-dd HH:mm:ss} UTC");
            }
            
            // Get MBR Explorer position from ephemeris (in kilometers, heliocentric)
            Vector3Double mbrPos_km = mbrEphemerisReader.GetPositionAtTime(simulationDate);
            
            if (mbrPos_km == null)
            {
                Debug.LogWarning("KotlinLaunchReceiver: Failed to get MBR Explorer position from ephemeris!");
                return Vector3.zero;
            }
            
            // Get scale from simulation manager
            float scale = 1e-9f; // Default scale
            var scaleField = typeof(EphemerisBasedSimulation).GetField("scaleToUnityUnits",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (scaleField != null)
            {
                scale = (float)scaleField.GetValue(simulationManager);
            }
            
            // Convert from kilometers to Unity coordinates
            // Ephemeris is in heliocentric km, same coordinate system as asteroid readers
            double x_m = mbrPos_km.x * KM_TO_METRES;
            double y_m = mbrPos_km.y * KM_TO_METRES;
            double z_m = mbrPos_km.z * KM_TO_METRES;
            
            // Apply scale and axis conversion (same as EphemerisBasedSimulation.ConvertKilometersToUnity)
            float x = (float)(x_m * scale);
            float y = (float)(z_m * scale); // Z becomes Y (up axis)
            float z = (float)(y_m * scale); // Y becomes Z
            
            Vector3 unityPos = new Vector3(x, y, z);
            
            if (showDebugInfo)
            {
                Debug.Log($"KotlinLaunchReceiver: MBR Explorer ephemeris position:");
                Debug.Log($"   Date: {simulationDate:yyyy-MM-dd HH:mm:ss} UTC");
                Debug.Log($"   Ephemeris (km): X={mbrPos_km.x:E2}, Y={mbrPos_km.y:E2}, Z={mbrPos_km.z:E2}");
                Debug.Log($"   Unity position: {unityPos}");
            }
            
            return unityPos;
        }
        catch (Exception e)
        {
            Debug.LogError($"KotlinLaunchReceiver: Error getting MBR Explorer ephemeris position: {e.Message}");
            return Vector3.zero;
        }
    }
}

