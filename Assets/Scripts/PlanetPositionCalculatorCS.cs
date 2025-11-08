using UnityEngine;
using CosineKitty; // Astronomy Engine namespace
using System;

/// <summary>
/// Native C# calculator for planet positions using Astronomy Engine
/// Replaces the Python API server functionality
/// </summary>
public class PlanetPositionCalculatorCS : MonoBehaviour
{
    // Conversion constant: 1 AU to metres
    private const double AU_TO_METRES = 149597870700.0;
    
    /// <summary>
    /// Get heliocentric positions of all planets for a given date and time
    /// </summary>
    public PlanetPositions GetPlanetPositions(DateTime dateTime)
    {
        // Create AstroTime from DateTime
        AstroTime time = new AstroTime(dateTime);
        
        PlanetPositions positions = new PlanetPositions();
        
        // Calculate heliocentric positions for each planet
        positions.mercury = GetHeliocentricPosition(Body.Mercury, time);
        positions.venus = GetHeliocentricPosition(Body.Venus, time);
        positions.earth = GetHeliocentricPosition(Body.Earth, time);
        positions.mars = GetHeliocentricPosition(Body.Mars, time);
        positions.jupiter = GetHeliocentricPosition(Body.Jupiter, time);
        positions.saturn = GetHeliocentricPosition(Body.Saturn, time);
        positions.uranus = GetHeliocentricPosition(Body.Uranus, time);
        positions.neptune = GetHeliocentricPosition(Body.Neptune, time);
        
        Debug.Log($"Calculated heliocentric positions for {dateTime:yyyy-MM-dd HH:mm:ss} UTC");
        
        return positions;
    }
    
    /// <summary>
    /// Get heliocentric position for a single planet
    /// </summary>
    private Vector3Double GetHeliocentricPosition(Body planet, AstroTime time)
    {
        // Get heliocentric position (relative to Sun)
        // Astronomy Engine provides this directly via HelioVector
        AstroVector vector = Astronomy.HelioVector(planet, time);
        
        // Convert from AU to metres
        double x_metres = vector.x * AU_TO_METRES;
        double y_metres = vector.y * AU_TO_METRES;
        double z_metres = vector.z * AU_TO_METRES;
        
        return new Vector3Double(x_metres, y_metres, z_metres);
    }
    
    /// <summary>
    /// Get position for current system time
    /// </summary>
    public PlanetPositions GetCurrentPlanetPositions()
    {
        return GetPlanetPositions(DateTime.UtcNow);
    }
    
    /// <summary>
    /// Get position for specific date/time string (dd-mm-yyyy HH:mm:ss)
    /// </summary>
    public PlanetPositions GetPlanetPositions(string date, string time)
    {
        try
        {
            string[] dateParts = date.Split('-');
            string[] timeParts = time.Split(':');
            
            int day = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[2]);
            
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);
            int second = int.Parse(timeParts[2]);
            
            DateTime dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            
            return GetPlanetPositions(dateTime);
        }
        catch (Exception e)
        {
            Debug.LogError($"PlanetCalculator: Failed to parse date/time: {e.Message}");
            return null;
        }
    }
}

// Data structures for planet positions
[Serializable]
public class Vector3Double
{
    public double x;
    public double y;
    public double z;
    
    public Vector3Double(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public override string ToString()
    {
        return $"({x:E2}, {y:E2}, {z:E2})";
    }
}

[Serializable]
public class PlanetPositions
{
    public Vector3Double mercury;
    public Vector3Double venus;
    public Vector3Double earth;
    public Vector3Double mars;
    public Vector3Double jupiter;
    public Vector3Double saturn;
    public Vector3Double uranus;
    public Vector3Double neptune;
}

