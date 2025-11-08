using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Reads JPL Horizons vector table for asteroids
/// Provides position lookup by date/time with interpolation
/// </summary>
public class AsteroidEphemerisReader : MonoBehaviour
{
    [System.Serializable]
    public class AsteroidDataPoint
    {
        public DateTime dateTime;
        public double x_km;  // Heliocentric X in kilometers
        public double y_km;  // Heliocentric Y in kilometers
        public double z_km;  // Heliocentric Z in kilometers
        
        public AsteroidDataPoint(DateTime dt, double x, double y, double z)
        {
            dateTime = dt;
            x_km = x;
            y_km = y;
            z_km = z;
        }
    }
    
    [Header("Ephemeris File")]
    [Tooltip("JPL Horizons vector table text file")]
    public TextAsset ephemerisFile;
    
    [Header("Asteroid Info")]
    public string asteroidName = "Justitia";
    
    [Tooltip("Mass in kilograms (set automatically based on asteroid name)")]
    [SerializeField] private double massKg = 1.4e17;
    
    [Header("Status")]
    [SerializeField] private int dataPointsLoaded = 0;
    [SerializeField] private string dateRangeStart = "";
    [SerializeField] private string dateRangeEnd = "";
    
    private List<AsteroidDataPoint> dataPoints = new List<AsteroidDataPoint>();
    private bool isLoaded = false;
    
    // Asteroid mass database (in kilograms)
    private static readonly Dictionary<string, double> asteroidMassDatabase = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        { "Justitia", 1.4e17 },     // 58 km diameter
        { "Chimaera", 2.1e14 },     // ~8 km diameter
        { "Rockox", 1.5e14 },       // 5.24 km diameter
        { "Westerwald", 1.3e13 },   // 2.27 km diameter
        { "Ousha", 3.4e13 },        // ~3 km diameter
        { "Moza", 1.6e13 },         // ~2.5 km diameter
        { "Ghaf", 8.4e12 }          // ~2 km diameter
    };
    
    // Earth mass constant for conversion
    private const double EARTH_MASS_KG = 5.972e24;
    
    void Awake()
    {
        // Set mass from database based on asteroid name
        if (asteroidMassDatabase.TryGetValue(asteroidName, out double mass))
        {
            massKg = mass;
            Debug.Log($"AsteroidEphemeris ({asteroidName}): Mass set to {massKg:E2} kg ({GetMassInEarthMasses():E2} Earth masses)");
        }
        else
        {
            Debug.LogWarning($"AsteroidEphemeris ({asteroidName}): Unknown asteroid, using default mass.");
        }
        
        // Parse in Awake to ensure data is ready before other scripts use it
        if (ephemerisFile != null)
        {
            ParseEphemerisFile();
        }
        else
        {
            Debug.LogError($"AsteroidEphemeris ({asteroidName}): No ephemeris file assigned!");
        }
    }
    
    void ParseEphemerisFile()
    {
        Debug.Log($"AsteroidEphemeris ({asteroidName}): Parsing ephemeris file...");
        Debug.Log($"  File has {ephemerisFile.text.Length} characters");
        
        string[] lines = ephemerisFile.text.Split('\n');
        Debug.Log($"  File has {lines.Length} lines");
        
        bool headerFound = false;
        int linesProcessed = 0;
        
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            // Skip empty lines
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            // Check for CSV header
            if (line.Contains("JDTDB") && line.Contains("Date") && line.Contains("X_km"))
            {
                headerFound = true;
                Debug.Log($"  Found CSV header at line {i}");
                continue;
            }
            
            // Parse CSV data rows (after header found)
            if (headerFound && !line.StartsWith("$$"))
            {
                try
                {
                    // Parse CSV line
                    string[] fields = line.Split(',');
                    
                    if (fields.Length >= 5) // Need at least JDTDB, Date, X, Y, Z
                    {
                        // Field 1 is the date string (e.g., " A.D. 2025-Nov-05 00:00:00.0000")
                        string dateStr = fields[1].Trim();
                        DateTime date = ParseCSVDateField(dateStr);
                        
                        // Fields 2, 3, 4 are X, Y, Z in kilometers
                        double x_km = double.Parse(fields[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                        double y_km = double.Parse(fields[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                        double z_km = double.Parse(fields[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
                        
                        dataPoints.Add(new AsteroidDataPoint(date, x_km, y_km, z_km));
                        linesProcessed++;
                    }
                }
                catch (Exception e)
                {
                    if (linesProcessed < 3)
                    {
                        Debug.LogWarning($"AsteroidEphemeris: Failed to parse line {i}: {e.Message}");
                        Debug.LogWarning($"  Line: {line.Substring(0, Math.Min(100, line.Length))}...");
                    }
                }
            }
        }
        
        Debug.Log($"  Successfully parsed {linesProcessed} data lines");

        
        dataPointsLoaded = dataPoints.Count;
        
        if (dataPoints.Count > 0)
        {
            dataPoints.Sort((a, b) => a.dateTime.CompareTo(b.dateTime));
            dateRangeStart = dataPoints[0].dateTime.ToString("yyyy-MM-dd");
            dateRangeEnd = dataPoints[dataPoints.Count - 1].dateTime.ToString("yyyy-MM-dd");
            isLoaded = true;
            
            Debug.Log($"AsteroidEphemeris ({asteroidName}): Loaded {dataPoints.Count} data points");
            Debug.Log($"  Date range: {dateRangeStart} to {dateRangeEnd}");
        }
        else
        {
            Debug.LogError($"AsteroidEphemeris ({asteroidName}): No data points found in file!");
        }
    }
    
    DateTime ParseCSVDateField(string dateField)
    {
        // Parse CSV date field: " A.D. 2025-Nov-05 00:00:00.0000"
        string dateStr = dateField.Replace("A.D.", "").Trim();
        
        // Remove milliseconds if present: "2025-Nov-05 00:00:00.0000" -> "2025-Nov-05 00:00:00"
        int dotIndex = dateStr.LastIndexOf('.');
        if (dotIndex > 0)
        {
            dateStr = dateStr.Substring(0, dotIndex);
        }
        
        // Parse: "2025-Nov-05 00:00:00"
        DateTime result = DateTime.ParseExact(
            dateStr,
            "yyyy-MMM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal
        );
        
        return result;
    }
    
    DateTime ParseDateLine(string line)
    {
        // Legacy parser for old format: "2460984.500000000 = A.D. 2025-Nov-05 00:00:00.0000 TDB"
        int adIndex = line.IndexOf("A.D. ");
        if (adIndex < 0) throw new Exception("A.D. not found");
        
        string dateStr = line.Substring(adIndex + 5).Trim();
        // Extract just the date/time part: "2025-Nov-05 00:00:00.0000"
        int tdbIndex = dateStr.IndexOf(" TDB");
        if (tdbIndex > 0)
        {
            dateStr = dateStr.Substring(0, tdbIndex).Trim();
        }
        
        // Parse: "2025-Nov-05 00:00:00.0000"
        DateTime result = DateTime.ParseExact(
            dateStr,
            "yyyy-MMM-dd HH:mm:ss.ffff",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal
        );
        
        return result;
    }
    
    Vector3Double ParsePositionLine(string line)
    {
        // Parse: " X =-3.833228159916487E+08 Y = 7.241479176341898E+06 Z = 1.402013991181966E+07"
        try
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            double x = 0, y = 0, z = 0;
            
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i] == "X")
                {
                    // Next part should be "=" followed by value, or value with =
                    string val = parts[i + 1].Replace("=", "");
                    x = double.Parse(val, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                else if (parts[i] == "Y")
                {
                    string val = parts[i + 1].Replace("=", "");
                    y = double.Parse(val, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
                else if (parts[i] == "Z")
                {
                    string val = parts[i + 1].Replace("=", "");
                    z = double.Parse(val, NumberStyles.Float, CultureInfo.InvariantCulture);
                }
            }
            
            return new Vector3Double(x, y, z);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to parse position line: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Get asteroid position at a specific time (with interpolation)
    /// Returns position in kilometers (heliocentric)
    /// </summary>
    public Vector3Double GetPositionAtTime(DateTime targetTime)
    {
        if (!isLoaded || dataPoints.Count == 0)
        {
            Debug.LogWarning($"AsteroidEphemeris ({asteroidName}): Data not loaded!");
            return null;
        }
        
        // Check if target time is within range
        if (targetTime < dataPoints[0].dateTime)
        {
            Debug.LogWarning($"AsteroidEphemeris ({asteroidName}): Target time before data range. Using first data point.");
            var first = dataPoints[0];
            return new Vector3Double(first.x_km, first.y_km, first.z_km);
        }
        
        if (targetTime > dataPoints[dataPoints.Count - 1].dateTime)
        {
            Debug.LogWarning($"AsteroidEphemeris ({asteroidName}): Target time after data range. Using last data point.");
            var last = dataPoints[dataPoints.Count - 1];
            return new Vector3Double(last.x_km, last.y_km, last.z_km);
        }
        
        // Find the two data points to interpolate between
        AsteroidDataPoint before = null;
        AsteroidDataPoint after = null;
        
        for (int i = 0; i < dataPoints.Count - 1; i++)
        {
            if (dataPoints[i].dateTime <= targetTime && dataPoints[i + 1].dateTime >= targetTime)
            {
                before = dataPoints[i];
                after = dataPoints[i + 1];
                break;
            }
        }
        
        if (before == null || after == null)
        {
            // Shouldn't happen, but fallback to nearest
            return FindNearestDataPoint(targetTime);
        }
        
        // Linear interpolation
        double totalSeconds = (after.dateTime - before.dateTime).TotalSeconds;
        double elapsedSeconds = (targetTime - before.dateTime).TotalSeconds;
        double t = elapsedSeconds / totalSeconds; // 0 to 1
        
        double x = Lerp(before.x_km, after.x_km, t);
        double y = Lerp(before.y_km, after.y_km, t);
        double z = Lerp(before.z_km, after.z_km, t);
        
        return new Vector3Double(x, y, z);
    }
    
    Vector3Double FindNearestDataPoint(DateTime targetTime)
    {
        AsteroidDataPoint nearest = dataPoints[0];
        double minDiff = Math.Abs((targetTime - dataPoints[0].dateTime).TotalSeconds);
        
        foreach (var point in dataPoints)
        {
            double diff = Math.Abs((targetTime - point.dateTime).TotalSeconds);
            if (diff < minDiff)
            {
                minDiff = diff;
                nearest = point;
            }
        }
        
        return new Vector3Double(nearest.x_km, nearest.y_km, nearest.z_km);
    }
    
    double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
    
    /// <summary>
    /// Check if ephemeris covers a specific date
    /// </summary>
    public bool CoversDate(DateTime date)
    {
        if (!isLoaded || dataPoints.Count == 0) return false;
        return date >= dataPoints[0].dateTime && date <= dataPoints[dataPoints.Count - 1].dateTime;
    }
    
    /// <summary>
    /// Get date range info
    /// </summary>
    public (DateTime start, DateTime end) GetDateRange()
    {
        if (!isLoaded || dataPoints.Count == 0)
            return (DateTime.MinValue, DateTime.MinValue);
        
        return (dataPoints[0].dateTime, dataPoints[dataPoints.Count - 1].dateTime);
    }
    
    /// <summary>
    /// Get asteroid mass in kilograms
    /// </summary>
    public double GetMassKg()
    {
        return massKg;
    }
    
    /// <summary>
    /// Get asteroid mass in Earth masses
    /// </summary>
    public double GetMassInEarthMasses()
    {
        return massKg / EARTH_MASS_KG;
    }
    
    /// <summary>
    /// Get mass of any asteroid from the database (static method)
    /// </summary>
    public static double GetAsteroidMass(string name)
    {
        if (asteroidMassDatabase.TryGetValue(name, out double mass))
        {
            return mass;
        }
        return 0;
    }
}

