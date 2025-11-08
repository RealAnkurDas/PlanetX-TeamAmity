using UnityEngine;
using UnityEngine.InputSystem;
using CosineKitty;
using System;
using System.Collections.Generic;

/// <summary>
/// Ephemeris-based solar system simulation
/// Updates planet positions from astronomical calculations at each timestep
/// Uses GravitySimulator for small bodies (spacecraft, asteroids)
/// </summary>
public class EphemerisBasedSimulation : MonoBehaviour
{
    [Header("Simulation Time")]
    [Tooltip("Use current system date and time as starting point")]
    [SerializeField] private bool useCurrentDateTime = true;
    
    [Tooltip("Manual start date (dd-mm-yyyy)")]
    [SerializeField] private string manualStartDate = "05-11-2025";
    
    [Tooltip("Manual start time (HH:mm:ss)")]
    [SerializeField] private string manualStartTime = "20:00:00";
    
    [Header("Simulation Limits")]
    [Tooltip("Stop simulation when end date is reached")]
    [SerializeField] private bool enableEndDate = true;
    
    [Tooltip("End date (yyyy-MM-dd) - Matches asteroid data range")]
    [SerializeField] private string endDateString = "2060-01-01";
    
    [Header("Time Step Control")]
    [Tooltip("Simulation seconds per real second (1 = real-time, 86400 = 1 day per second)")]
    [SerializeField] private double timeStepMultiplier = 1.0; // Real-time by default
    
    [Tooltip("Minimum timestep multiplier")]
    [SerializeField] private double minTimeStep = 1.0;
    
    [Tooltip("Maximum timestep multiplier")]
    [SerializeField] private double maxTimeStep = 31536000.0; // 1 year per second
    
    [Header("Scale Settings")]
    [Tooltip("Convert metres to Unity units (1e-9 = billions of metres)")]
    [SerializeField] private float scaleToUnityUnits = 1e-9f;
    
    [Header("Asteroids")]
    [Tooltip("Asteroid ephemeris readers (for Justitia, etc.)")]
    [SerializeField] private AsteroidEphemerisReader[] asteroidReaders;
    
    [Header("Status")]
    [SerializeField] private bool isRunning = false;
    [SerializeField] private string currentSimulationDate = "";
    [SerializeField] private string currentSimulationTime = "";
    
    // Conversion constant
    private const double AU_TO_METRES = 149597870700.0;
    
    // Simulation state
    private AstroTime currentAstroTime;
    private DateTime startDateTime;
    private DateTime endDate;
    private bool hasReachedEnd = false;
    private GravitySimulator gravitySimulator;
    
    // Planet GameObjects cache
    private Dictionary<Body, GameObject> planetObjects = new Dictionary<Body, GameObject>();
    private GameObject sunObject;
    
    // Small bodies (spacecraft, asteroids) that use GravitySimulator
    private List<GameObject> smallBodies = new List<GameObject>();
    private StateVector[] smallBodyStates;
    
    // Debug
    private int frameCount = 0;
    
    void Start()
    {
        // Wait a bit for other Start() methods to complete
        StartCoroutine(DelayedInitialization());
    }
    
    System.Collections.IEnumerator DelayedInitialization()
    {
        // Wait for end of frame to ensure all Start() methods have run
        yield return new WaitForEndOfFrame();
        // Wait one more frame for good measure
        yield return null;
        
        Debug.Log("EphemerisSim: Starting delayed initialization...");
        InitializeSimulation();
    }
    
    void Update()
    {
        // Press R to reinitialize gravity simulator (for debugging spacecraft issues)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && smallBodies.Count > 0)
        {
            Debug.Log("EphemerisSim: Manual reinitialization requested (R key)");
            InitializeGravitySimulator();
        }
    }
    
    void InitializeSimulation()
    {
        // Determine starting date/time
        if (useCurrentDateTime)
        {
            startDateTime = DateTime.UtcNow;
            Debug.Log($"EphemerisSim: Using current date/time (UTC): {startDateTime:dd-MM-yyyy HH:mm:ss}");
        }
        else
        {
            try
            {
                string[] dateParts = manualStartDate.Split('-');
                string[] timeParts = manualStartTime.Split(':');
                
                int day = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int year = int.Parse(dateParts[2]);
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                int second = int.Parse(timeParts[2]);
                
                startDateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                Debug.Log($"EphemerisSim: Using manual date/time: {startDateTime:dd-MM-yyyy HH:mm:ss}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse date/time: {e.Message}. Using current time.");
                startDateTime = DateTime.UtcNow;
            }
        }
        
        // Parse end date limit
        if (enableEndDate)
        {
            try
            {
                endDate = DateTime.ParseExact(endDateString, "yyyy-MM-dd", 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.AssumeUniversal);
                Debug.Log($"EphemerisSim: Simulation will stop at {endDate:yyyy-MM-dd}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse end date '{endDateString}': {e.Message}. End date disabled.");
                enableEndDate = false;
            }
        }
        
        // Create AstroTime
        currentAstroTime = new AstroTime(startDateTime);
        UpdateDateTimeDisplay();
        
        // Find and cache planet GameObjects
        CachePlanetObjects();
        
        // Find small bodies (tagged as "SmallBody" - spacecraft, asteroids, etc.)
        CacheSmallBodies();
        
        // Set initial planet positions
        UpdatePlanetPositions();
        
        // Initialize gravity simulator for small bodies if any
        if (smallBodies.Count > 0)
        {
            InitializeGravitySimulator();
        }
        
        isRunning = true;
        Debug.Log($"EphemerisSim: Simulation initialized and running!");
    }
    
    void CachePlanetObjects()
    {
        // Find all celestial objects (planets)
        GameObject[] celestials = GameObject.FindGameObjectsWithTag("Celestial");
        Debug.Log($"EphemerisSim: Found {celestials.Length} objects with 'Celestial' tag");
        
        foreach (GameObject obj in celestials)
        {
            string name = obj.name.ToLower();
            
            if (name.Contains("mercury"))
            {
                planetObjects[Body.Mercury] = obj;
                Debug.Log($"  - Cached Mercury: {obj.name}");
            }
            else if (name.Contains("venus"))
            {
                planetObjects[Body.Venus] = obj;
                Debug.Log($"  - Cached Venus: {obj.name}");
            }
            else if (name.Contains("earth"))
            {
                planetObjects[Body.Earth] = obj;
                Debug.Log($"  - Cached Earth: {obj.name}");
            }
            else if (name.Contains("mars"))
            {
                planetObjects[Body.Mars] = obj;
                Debug.Log($"  - Cached Mars: {obj.name}");
            }
            else if (name.Contains("jupiter"))
            {
                planetObjects[Body.Jupiter] = obj;
                Debug.Log($"  - Cached Jupiter: {obj.name}");
            }
            else if (name.Contains("saturn"))
            {
                planetObjects[Body.Saturn] = obj;
                Debug.Log($"  - Cached Saturn: {obj.name}");
            }
            else if (name.Contains("uranus"))
            {
                planetObjects[Body.Uranus] = obj;
                Debug.Log($"  - Cached Uranus: {obj.name}");
            }
            else if (name.Contains("neptune"))
            {
                planetObjects[Body.Neptune] = obj;
                Debug.Log($"  - Cached Neptune: {obj.name}");
            }
        }
        
        // Find Sun
        sunObject = GameObject.Find("Sun Sphere");
        if (sunObject == null) sunObject = GameObject.Find("Sun");
        
        if (sunObject != null)
        {
            Debug.Log($"EphemerisSim: Found Sun: {sunObject.name}");
        }
        else
        {
            Debug.LogWarning("EphemerisSim: Sun not found!");
        }
        
        Debug.Log($"EphemerisSim: Total planets cached: {planetObjects.Count}");
    }
    
    void CacheSmallBodies()
    {
        // Find objects tagged as "SmallBody" (spacecraft, asteroids)
        GameObject[] smalls = GameObject.FindGameObjectsWithTag("SmallBody");
        smallBodies.AddRange(smalls);
        
        if (smallBodies.Count > 0)
        {
            Debug.Log($"EphemerisSim: Found {smallBodies.Count} small bodies for gravity simulation");
        }
    }
    
    void InitializeGravitySimulator()
    {
        // Create initial state vectors for small bodies
        StateVector[] initialStates = new StateVector[smallBodies.Count];
        
        for (int i = 0; i < smallBodies.Count; i++)
        {
            // Get current position and velocity from Rigidbody
            Transform t = smallBodies[i].transform;
            Rigidbody rb = smallBodies[i].GetComponent<Rigidbody>();
            
            // Convert Unity position to astronomical units (heliocentric)
            Vector3 unityPos = t.position;
            if (sunObject != null)
            {
                unityPos -= sunObject.transform.position; // Make relative to Sun
            }
            
            // Scale back to metres, then to AU
            double x_au = (unityPos.x / scaleToUnityUnits) / AU_TO_METRES;
            double y_au = (unityPos.z / scaleToUnityUnits) / AU_TO_METRES; // Unity Z → Astro Y
            double z_au = (unityPos.y / scaleToUnityUnits) / AU_TO_METRES; // Unity Y → Astro Z
            
            // Convert velocity (if has Rigidbody)
            double vx_au_per_day = 0, vy_au_per_day = 0, vz_au_per_day = 0;
            if (rb != null)
            {
                // Convert Unity velocity to AU per day
                double metersPerSecond_to_AUperDay = 86400.0 / AU_TO_METRES;
                Vector3 vel = rb.linearVelocity / scaleToUnityUnits; // Scale to real metres/sec
                vx_au_per_day = vel.x * metersPerSecond_to_AUperDay;
                vy_au_per_day = vel.z * metersPerSecond_to_AUperDay;
                vz_au_per_day = vel.y * metersPerSecond_to_AUperDay;
                
                Debug.Log($"  {smallBodies[i].name}: Unity vel={rb.linearVelocity}, AU/day=({vx_au_per_day:F6}, {vy_au_per_day:F6}, {vz_au_per_day:F6})");
                
                // Warn if velocity is nearly zero
                if (rb.linearVelocity.magnitude < 0.001f)
                {
                    Debug.LogWarning($"  ⚠️ {smallBodies[i].name} has ZERO VELOCITY! Will fall into Sun!");
                    Debug.LogWarning($"  → Wait a moment, then press 'R' key to reinitialize with correct velocity");
                }
            }
            else
            {
                Debug.LogWarning($"  {smallBodies[i].name}: NO RIGIDBODY! Velocity will be zero.");
            }
            
            initialStates[i] = new StateVector(x_au, y_au, z_au, vx_au_per_day, vy_au_per_day, vz_au_per_day, currentAstroTime);
        }
        
        // Create gravity simulator
        gravitySimulator = new GravitySimulator(Body.Sun, currentAstroTime, initialStates);
        smallBodyStates = new StateVector[smallBodies.Count];
        
        Debug.Log($"EphemerisSim: GravitySimulator initialized with {smallBodies.Count} small bodies");
    }
    
    void FixedUpdate()
    {
        if (!isRunning)
        {
            if (frameCount == 0)
            {
                Debug.LogWarning("EphemerisSim: Not running! Check isRunning flag in Inspector.");
            }
            frameCount++;
            return;
        }
        
        // Check if we've reached the end date limit
        if (enableEndDate && !hasReachedEnd)
        {
            DateTime currentDateTime = currentAstroTime.ToUtcDateTime();
            if (currentDateTime >= endDate)
            {
                hasReachedEnd = true;
                isRunning = false;
                Debug.Log($"EphemerisSim: Reached end date {endDate:yyyy-MM-dd}. Simulation stopped.");
                Debug.Log($"  Final simulation date: {currentDateTime:yyyy-MM-dd HH:mm:ss}");
                return;
            }
        }
        
        // Advance simulation time
        double deltaSeconds = Time.fixedDeltaTime * timeStepMultiplier;
        double deltaDays = deltaSeconds / 86400.0; // Convert to days
        
        // Update AstroTime
        currentAstroTime = currentAstroTime.AddDays(deltaDays);
        UpdateDateTimeDisplay();
        
        // Update planet positions from ephemeris
        UpdatePlanetPositions();
        
        // Update asteroid positions from their ephemeris files
        UpdateAsteroidPositions();
        
        // Update small bodies using GravitySimulator
        if (gravitySimulator != null && smallBodies.Count > 0)
        {
            UpdateSmallBodies(deltaDays);
        }
        
        // Debug output every 50 frames
        frameCount++;
        if (frameCount % 50 == 0)
        {
            Debug.Log($"EphemerisSim: Frame {frameCount}, Date: {currentSimulationDate}, Time: {currentSimulationTime}, Planets: {planetObjects.Count}");
        }
    }
    
    void UpdatePlanetPositions()
    {
        if (planetObjects.Count == 0)
        {
            Debug.LogWarning("EphemerisSim: No planets cached! Make sure planets have 'Celestial' tag.");
            return;
        }
        
        // Update each planet's position based on current time
        foreach (var kvp in planetObjects)
        {
            Body planet = kvp.Key;
            GameObject planetObj = kvp.Value;
            
            if (planetObj == null)
            {
                Debug.LogWarning($"EphemerisSim: Planet GameObject for {planet} is null!");
                continue;
            }
            
            // Get heliocentric position from ephemeris
            AstroVector helioPos = Astronomy.HelioVector(planet, currentAstroTime);
            
            // Convert to Unity coordinates
            Vector3 unityPos = ConvertAstroToUnity(helioPos);
            planetObj.transform.position = unityPos;
        }
        
        // Keep Sun at origin
        if (sunObject != null)
        {
            sunObject.transform.position = Vector3.zero;
        }
    }
    
    void UpdateAsteroidPositions()
    {
        if (asteroidReaders == null || asteroidReaders.Length == 0) return;
        
        DateTime currentDateTime = currentAstroTime.ToUtcDateTime();
        
        foreach (AsteroidEphemerisReader reader in asteroidReaders)
        {
            if (reader == null) continue;
            
            // Get position from ephemeris file
            Vector3Double pos_km = reader.GetPositionAtTime(currentDateTime);
            
            if (pos_km != null)
            {
                // Convert from km to Unity coordinates
                Vector3 unityPos = ConvertKilometersToUnity(pos_km);
                
                // Find and update the asteroid GameObject
                GameObject asteroid = GameObject.Find(reader.asteroidName);
                if (asteroid != null)
                {
                    asteroid.transform.position = unityPos;
                }
            }
        }
    }
    
    Vector3 ConvertKilometersToUnity(Vector3Double pos_km)
    {
        // Convert from km to metres, then to Unity units
        double x_m = pos_km.x * 1000.0;
        double y_m = pos_km.y * 1000.0;
        double z_m = pos_km.z * 1000.0;
        
        // Apply scale and axis conversion
        float x = (float)(x_m * scaleToUnityUnits);
        float y = (float)(z_m * scaleToUnityUnits); // Z becomes Y (up axis)
        float z = (float)(y_m * scaleToUnityUnits); // Y becomes Z
        
        return new Vector3(x, y, z);
    }
    
    void UpdateSmallBodies(double deltaDays)
    {
        // Update gravity simulation
        gravitySimulator.Update(currentAstroTime, smallBodyStates);
        
        // Apply updated positions to GameObjects
        for (int i = 0; i < smallBodies.Count; i++)
        {
            StateVector state = smallBodyStates[i];
            
            // Convert from AU to Unity coordinates
            Vector3 unityPos = ConvertAstroToUnity_StateVector(state);
            smallBodies[i].transform.position = unityPos;
            
            // Update velocity if has Rigidbody
            Rigidbody rb = smallBodies[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 unityVel = ConvertAstroVelocityToUnity(state);
                rb.linearVelocity = unityVel;
            }
        }
    }
    
    Vector3 ConvertAstroToUnity(AstroVector astroVec)
    {
        // Convert from AU to metres
        double x_m = astroVec.x * AU_TO_METRES;
        double y_m = astroVec.y * AU_TO_METRES;
        double z_m = astroVec.z * AU_TO_METRES;
        
        // Convert to Unity coordinates and scale
        float x = (float)(x_m * scaleToUnityUnits);
        float y = (float)(z_m * scaleToUnityUnits); // Astro Z → Unity Y
        float z = (float)(y_m * scaleToUnityUnits); // Astro Y → Unity Z
        
        return new Vector3(x, y, z);
    }
    
    Vector3 ConvertAstroToUnity_StateVector(StateVector state)
    {
        // Convert from AU to metres, then to Unity units
        double x_m = state.x * AU_TO_METRES;
        double y_m = state.y * AU_TO_METRES;
        double z_m = state.z * AU_TO_METRES;
        
        float x = (float)(x_m * scaleToUnityUnits);
        float y = (float)(z_m * scaleToUnityUnits);
        float z = (float)(y_m * scaleToUnityUnits);
        
        return new Vector3(x, y, z);
    }
    
    Vector3 ConvertAstroVelocityToUnity(StateVector state)
    {
        // StateVector velocity is in AU per day
        // Convert to Unity units per second
        
        double au_per_day_to_m_per_sec = AU_TO_METRES / 86400.0;
        
        double vx_m_per_sec = state.vx * au_per_day_to_m_per_sec;
        double vy_m_per_sec = state.vy * au_per_day_to_m_per_sec;
        double vz_m_per_sec = state.vz * au_per_day_to_m_per_sec;
        
        float vx = (float)(vx_m_per_sec * scaleToUnityUnits);
        float vy = (float)(vz_m_per_sec * scaleToUnityUnits);
        float vz = (float)(vy_m_per_sec * scaleToUnityUnits);
        
        return new Vector3(vx, vy, vz);
    }
    
    void UpdateDateTimeDisplay()
    {
        DateTime current = currentAstroTime.ToUtcDateTime();
        currentSimulationDate = current.ToString("dd-MM-yyyy");
        currentSimulationTime = current.ToString("HH:mm:ss");
    }
    
    // ========== PUBLIC API ==========
    
    /// <summary>
    /// Set simulation speed multiplier (1 = real-time, higher = faster)
    /// </summary>
    public void SetTimeStepMultiplier(double multiplier)
    {
        timeStepMultiplier = Mathf.Clamp((float)multiplier, (float)minTimeStep, (float)maxTimeStep);
        Debug.Log($"EphemerisSim: Time step multiplier set to {timeStepMultiplier:E2} (simulation seconds per real second)");
    }
    
    /// <summary>
    /// Get current simulation DateTime
    /// </summary>
    public DateTime GetCurrentDateTime()
    {
        return currentAstroTime.ToUtcDateTime();
    }
    
    /// <summary>
    /// Get current simulation date string
    /// </summary>
    public string GetCurrentDate() => currentSimulationDate;
    
    /// <summary>
    /// Get current simulation time string
    /// </summary>
    public string GetCurrentTime() => currentSimulationTime;
    
    /// <summary>
    /// Pause/Resume simulation
    /// </summary>
    public void TogglePause()
    {
        isRunning = !isRunning;
        Debug.Log($"EphemerisSim: {(isRunning ? "Running" : "Paused")}");
    }
    
    /// <summary>
    /// Reset simulation to starting time
    /// </summary>
    public void ResetSimulation()
    {
        currentAstroTime = new AstroTime(startDateTime);
        UpdateDateTimeDisplay();
        UpdatePlanetPositions();
        
        if (gravitySimulator != null && smallBodies.Count > 0)
        {
            InitializeGravitySimulator();
        }
        
        Debug.Log("EphemerisSim: Reset to starting time");
    }
    
    /// <summary>
    /// Get planet position at any time (doesn't affect simulation)
    /// </summary>
    public Vector3 GetPlanetPositionAtTime(Body planet, DateTime dateTime)
    {
        AstroTime time = new AstroTime(dateTime);
        AstroVector pos = Astronomy.HelioVector(planet, time);
        return ConvertAstroToUnity(pos);
    }
}

