using UnityEngine;

/// <summary>
/// Simple spacecraft launch script that receives velocity from Kotlin (or uses defaults in Editor)
/// Works exactly like SpacecraftInitialVelocity but reads from Kotlin Intent extras
/// </summary>
public class SpacecraftKotlinLaunch : MonoBehaviour
{
    [Header("Default Velocity (Editor Testing)")]
    [Tooltip("Default velocity X (m/s in Unity scale)")]
    public float defaultVelocityX = 0.01f;
    
    [Tooltip("Default velocity Y (m/s in Unity scale)")]
    public float defaultVelocityY = 0.0f;
    
    [Tooltip("Default velocity Z (m/s in Unity scale)")]
    public float defaultVelocityZ = 0.03f;
    
    [Header("Launch from Earth")]
    [Tooltip("Position spacecraft at Earth's location on start")]
    public bool launchFromEarth = true;
    
    [Header("Debug")]
    [Tooltip("Show velocity updates in console")]
    public bool debugMode = true;
    
    private GameObject earthObject;
    
    void Start()
    {
        Debug.Log($"SpacecraftKotlinLaunch: Start() called on {gameObject.name}");
        
        // Find Earth if launching from Earth
        if (launchFromEarth)
        {
            earthObject = GameObject.Find("Earth");
            if (earthObject != null)
            {
                transform.position = earthObject.transform.position;
                Debug.Log($"✓ Spacecraft positioned at Earth: {transform.position}");
            }
            else
            {
                Debug.LogWarning("SpacecraftKotlinLaunch: Earth not found! Using current position.");
            }
        }
        
        // Get velocity from Kotlin or use defaults
        float vx, vy, vz;
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        // On Android: Read from Kotlin Intent
        ReadVelocityFromKotlin(out vx, out vy, out vz);
        #else
        // In Editor: Use default values
        vx = defaultVelocityX;
        vy = defaultVelocityY;
        vz = defaultVelocityZ;
        Debug.Log("SpacecraftKotlinLaunch: Running in Editor, using default values");
        #endif
        
        // Apply velocity to Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reset any existing velocity first
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Build velocity vector from components
            Vector3 initialVelocity = new Vector3(vx, vy, vz);
            
            // Set initial velocity
            rb.linearVelocity = initialVelocity;
            
            Debug.Log($"✓ Spacecraft '{gameObject.name}' initial velocity set to {initialVelocity}");
            Debug.Log($"  Components: X={vx:F6}, Y={vy:F6}, Z={vz:F6}");
            Debug.Log($"  Magnitude: {initialVelocity.magnitude:F6}");
            Debug.Log($"  Rigidbody mass: {rb.mass} kg, Use Gravity: {rb.useGravity}");
            Debug.Log($"  Tag: {gameObject.tag}");
            Debug.Log($"  Position: {transform.position}");
            
            // Warn if velocity is very small
            if (initialVelocity.magnitude < 0.001f)
            {
                Debug.LogWarning($"⚠️ Initial velocity is very small ({initialVelocity.magnitude:F6})! Spacecraft may fall into Sun!");
            }
            
            // Rotate spacecraft to match velocity direction
            if (initialVelocity.magnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(initialVelocity);
                Debug.Log($"  Spacecraft rotated to match velocity direction");
            }
        }
        else
        {
            Debug.LogError($"✗ Spacecraft '{gameObject.name}' has no Rigidbody component!");
        }
    }
    
    void ReadVelocityFromKotlin(out float vx, out float vy, out float vz)
    {
        // Default values
        vx = defaultVelocityX;
        vy = defaultVelocityY;
        vz = defaultVelocityZ;
        
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent"))
            {
                // Read velocity from Intent extras
                vx = intent.Call<float>("getFloatExtra", "velocityX", defaultVelocityX);
                vy = intent.Call<float>("getFloatExtra", "velocityY", defaultVelocityY);
                vz = intent.Call<float>("getFloatExtra", "velocityZ", defaultVelocityZ);
                
                Debug.Log($"✅ Received from Kotlin: velocityX={vx}, velocityY={vy}, velocityZ={vz}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SpacecraftKotlinLaunch: Error reading Intent: {e.Message}");
            Debug.Log("Using default values instead");
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

