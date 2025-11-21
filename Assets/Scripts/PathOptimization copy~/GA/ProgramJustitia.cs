using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CosineKitty;

namespace TrajectoryOptimization
{
    /// <summary>
    /// Standalone Genetic Algorithm for Earth-to-Justitia trajectory optimization
    /// 6 maneuvers, 8-year mission window
    /// </summary>
    class ProgramJustitia
    {
        // Constants
        public const double G = 6.67430e-11;           // Gravitational constant
        public const double AU = 1.496e11;             // AU in meters
        public const double KM_TO_M = 1000.0;
        
        public static readonly DateTime START_DATE = new DateTime(2028, 3, 2, 0, 0, 0, DateTimeKind.Utc);  // Actual launch
        public static readonly DateTime END_DATE = new DateTime(2035, 5, 31, 0, 0, 0, DateTimeKind.Utc); // Landing on Justitia
        
        // FIXED PLANETARY FLYBY DATES (from actual mission plan)
        public static readonly DateTime VENUS_FLYBY_DATE = new DateTime(2028, 7, 15, 0, 0, 0, DateTimeKind.Utc);  // July 2028
        public static readonly DateTime EARTH_FLYBY_DATE = new DateTime(2029, 5, 15, 0, 0, 0, DateTimeKind.Utc);  // May 2029
        public static readonly DateTime MARS_FLYBY_DATE = new DateTime(2031, 9, 15, 0, 0, 0, DateTimeKind.Utc);   // Sept 2031
        
        // Expected asteroid encounter window
        public static readonly DateTime ASTEROID_START_DATE = new DateTime(2030, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime ASTEROID_END_DATE = new DateTime(2033, 8, 31, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime JUSTITIA_ARRIVAL_DATE = new DateTime(2034, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        
        // Planet and asteroid masses (kg)
        public static readonly Dictionary<Body, double> PlanetMasses = new Dictionary<Body, double>
        {
            { Body.Sun, 1.989e30 },
            { Body.Mercury, 3.285e23 },
            { Body.Venus, 4.867e24 },
            { Body.Earth, 5.972e24 },
            { Body.Mars, 6.39e23 },
            { Body.Jupiter, 1.898e27 },
            { Body.Saturn, 5.683e26 },
        };
        
        public const double JUSTITIA_MASS_KG = 1.4e17;  // 58 km diameter asteroid
        
        // ===== LAUNCH PARAMETERS (FIXED) =====
        // Mission-specific launch constraints from trajectory design
        public const double C3_KM2_S2 = 16.1;                    // Characteristic energy (km²/s²)
        public const double DLA_DEG = -19.8;                     // Declination of Launch Asymptote (degrees)
        public static readonly DateTime LAUNCH_DATE = new DateTime(2028, 3, 2, 0, 0, 0, DateTimeKind.Utc);
        
        // ===== SPACECRAFT FUEL PARAMETERS =====
        // Based on MBR Explorer actual mission specs
        public const double SPACECRAFT_MASS_KG = 2464.9;         // Actual spacecraft mass at launch
        public const double SPACECRAFT_DRY_MASS_KG = 1500.0;     // Estimated dry mass
        public const double INITIAL_FUEL_MASS_KG = 964.9;        // Propellant mass (39% fuel fraction)
        public const double TOTAL_INITIAL_MASS_KG = 2464.9;      // Total launch mass
        public const double SPECIFIC_IMPULSE_S = 3000.0;         // Ion thruster Isp (efficient for deep space)
        public const double G0 = 9.81;                            // Standard gravity (m/s²)
        
        /// <summary>
        /// Calculate launch velocity from C3 and DLA
        /// C3 = characteristic energy (v_infinity²)
        /// DLA = declination of launch asymptote
        /// Returns velocity relative to Earth's inertial frame
        /// </summary>
        public static (double vx, double vy) CalculateLaunchVelocity()
        {
            // V_infinity is the hyperbolic excess velocity (velocity at "infinity" relative to Earth)
            double v_infinity_km_s = Math.Sqrt(C3_KM2_S2);  // km/s
            double v_infinity = v_infinity_km_s * 1000.0;   // Convert to m/s
            
            // DLA is the out-of-plane angle (declination)
            // For 2D ecliptic simulation, we project onto the ecliptic plane
            double dla_rad = DLA_DEG * Math.PI / 180.0;
            double v_ecliptic = v_infinity * Math.Cos(dla_rad);  // In-plane component
            
            // For asteroid belt mission with DLA = -19.8°:
            // Negative DLA means launching "down" relative to ecliptic
            // For outbound mission to asteroid belt, typical geometry is:
            // - Prograde (in Earth's direction) with radial outward component
            
            // Decompose into radial (X) and tangential (Y) components
            // For C3=16.1 and DLA=-19.8, this is a moderately energetic outbound trajectory
            // Typical split for asteroid belt mission: ~70% tangential, ~30% radial
            double angle = 23.0 * Math.PI / 180.0;  // Launch angle relative to Earth velocity (outbound)
            
            double vx = v_ecliptic * Math.Sin(angle);  // Radial component (away from Sun)
            double vy = v_ecliptic * Math.Cos(angle);  // Tangential component (prograde)
            
            return (vx, vy);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("🛰️  MBR EXPLORER - 7-ASTEROID GRAND TOUR MISSION OPTIMIZATION");
            Console.WriteLine("================================================================================");
            Console.WriteLine("UAE Asteroid Belt Mission | Launch: March 2028");
            Console.WriteLine($"Mission Timeline: {START_DATE:yyyy-MM-dd} → {END_DATE:yyyy-MM-dd} (7 years)");
            Console.WriteLine("Using real ephemeris data from CosineKitty + JPL Horizons");
            Console.WriteLine("================================================================================");
            Console.WriteLine("🎯 MISSION PROFILE (Following Actual MBR Explorer Plan):");
            Console.WriteLine("  Planetary Gravity Assists:");
            Console.WriteLine($"   • Venus  - {VENUS_FLYBY_DATE:MMM yyyy}");
            Console.WriteLine($"   • Earth  - {EARTH_FLYBY_DATE:MMM yyyy}");
            Console.WriteLine($"   • Mars   - {MARS_FLYBY_DATE:MMM yyyy}");
            Console.WriteLine("  Asteroid Flyby Targets:");
            Console.WriteLine("   1. Westerwald (10253)  - Science flyby");
            Console.WriteLine("   2. Chimaera (623)      - Science flyby");
            Console.WriteLine("   3. Rockox (13294)      - Science flyby");
            Console.WriteLine("   4. Moza (59980)        - Science flyby");
            Console.WriteLine("   5. Ousha (23871)       - Science flyby");
            Console.WriteLine("   6. Ghaf (88055)        - Science flyby");
            Console.WriteLine("   7. Justitia (269)      - 🏁 LANDING TARGET (Oct 2034)");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"🚀 LAUNCH PARAMETERS (FIXED):");
            var (fixedVx, fixedVy) = CalculateLaunchVelocity();
            double v_inf = Math.Sqrt(C3_KM2_S2);
            Console.WriteLine($"  • Launch Date: {LAUNCH_DATE:yyyy-MM-dd}");
            Console.WriteLine($"  • C3: {C3_KM2_S2:F1} km²/s²");
            Console.WriteLine($"  • DLA: {DLA_DEG:F1}°");
            Console.WriteLine($"  • V∞: {v_inf:F3} km/s ({v_inf*1000:F0} m/s)");
            Console.WriteLine($"  • Launch ΔV: [{fixedVx:F0}, {fixedVy:F0}] m/s");
            Console.WriteLine("=============================================================================");
            Console.WriteLine($"🛰️  SPACECRAFT SPECIFICATIONS:");
            Console.WriteLine($"  • Launch Mass: {SPACECRAFT_MASS_KG:F1} kg (actual)");
            Console.WriteLine($"  • Dry Mass: {SPACECRAFT_DRY_MASS_KG:F0} kg (estimated)");
            Console.WriteLine($"  • Propellant: {INITIAL_FUEL_MASS_KG:F1} kg ({INITIAL_FUEL_MASS_KG/TOTAL_INITIAL_MASS_KG*100:F1}%)");
            Console.WriteLine($"  • Propulsion: Ion Thruster (Isp={SPECIFIC_IMPULSE_S:F0}s)");
            Console.WriteLine($"  • Available Maneuvers: 6 deep-space burns");
            Console.WriteLine($"  • Gravity Assists: Venus, Earth, Mars, Jupiter, Saturn");
            Console.WriteLine("================================================================================");
            Console.WriteLine("GA Optimization: Maneuver Timing & ΔV to Hit Flyby Windows & Minimize Fuel");
            Console.WriteLine("================================================================================\n");
            
            // Parse command line arguments
            int populationSize = 50;
            int generations = 100;
            
            if (args.Length >= 1 && int.TryParse(args[0], out int pop))
                populationSize = pop;
            if (args.Length >= 2 && int.TryParse(args[1], out int gen))
                generations = gen;
            
            Console.WriteLine($"Population Size: {populationSize}");
            Console.WriteLine($"Generations: {generations}");
            Console.WriteLine();
            
            // Load ALL asteroid ephemeris data
            Console.WriteLine("Loading asteroid ephemeris data for 7-asteroid grand tour...");
            var asteroidNames = new List<string> { "Westerwald", "Chimaera", "Rockox", "Moza", "Ousha", "Ghaf", "Justitia" };
            var asteroidEphemerides = new Dictionary<string, AsteroidEphemeris>();
            
            bool allLoaded = true;
            foreach (var name in asteroidNames)
            {
                Console.Write($"  Loading {name}... ");
                var ephemeris = new AsteroidEphemeris(name);
                if (ephemeris.LoadData())
                {
                    asteroidEphemerides[name] = ephemeris;
                    Console.WriteLine($"✓ ({ephemeris.GetDataPointCount()} points)");
                }
                else
                {
                    Console.WriteLine($"✗ FAILED");
                    allLoaded = false;
                }
            }
            
            if (!allLoaded)
            {
                Console.WriteLine("\nERROR: Could not load all asteroid ephemeris data!");
                Console.WriteLine("Please ensure all asteroid .txt files exist in 'asteroid_ephemeris/' folder");
                return;
            }
            
            Console.WriteLine($"\n✓ Successfully loaded ephemeris for all 7 asteroids\n");
            
            // Run the GA
            var ga = new JustitiaGeneticAlgorithm(populationSize, generations, asteroidEphemerides);
            
            Stopwatch sw = Stopwatch.StartNew();
            var bestChromosome = ga.Run();
            sw.Stop();
            
            Console.WriteLine($"\n⏱️  Total execution time: {sw.Elapsed.TotalSeconds:F2} seconds");
            Console.WriteLine("=============================================================\n");
            
            // Simulate and export best trajectory with full detail (for single trajectory view)
            Console.WriteLine("Simulating best trajectory with full detail...\n");
            ga.SimulateAndExportTrajectory(bestChromosome);
            
            // Also export last 10 generations for comparison view
            Console.WriteLine("\nExporting last 10 generations for comparison visualization...\n");
            ga.SimulateAndExportMultipleTrajectories(bestChromosome);
        }
    }
    
    // =============================================================================
    // ASTEROID EPHEMERIS LOADER (Multi-Asteroid Support)
    // =============================================================================
    
    public class AsteroidEphemeris
    {
        private List<(DateTime time, double x_km, double y_km, double z_km)> dataPoints;
        public string AsteroidName { get; private set; }
        
        public AsteroidEphemeris(string asteroidName)
        {
            AsteroidName = asteroidName;
            dataPoints = new List<(DateTime, double, double, double)>();
        }
        
        public bool LoadData()
        {
            string filePath = Path.Combine("asteroid_ephemeris", $"{AsteroidName}.txt");
            
            // Also try relative path from GA folder
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine("..", "..", "asteroid_ephemeris", $"{AsteroidName}.txt");
            }
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"ERROR: Could not find {AsteroidName}.txt at {filePath}");
                return false;
            }
            
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                bool inDataSection = false;
                
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    
                    // Look for CSV header
                    if (line.Contains("JDTDB") && line.Contains("Calendar Date"))
                    {
                        inDataSection = true;
                        continue;
                    }
                    
                    // Check for end marker
                    if (line.StartsWith("$$EOE"))
                    {
                        break;
                    }
                    
                    if (inDataSection && line.Contains(","))
                    {
                        // Parse CSV line
                        var parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            try
                            {
                                // Format: JDTDB, Calendar Date, X, Y, Z, ...
                                string dateStr = parts[1].Trim();
                                DateTime dt = ParseAsteroidDate(dateStr);
                                
                                double x = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
                                double y = double.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
                                double z = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                                
                                dataPoints.Add((dt, x, y, z));
                            }
                            catch
                            {
                                // Skip malformed lines
                            }
                        }
                    }
                }
                
                return dataPoints.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR loading Justitia data: {ex.Message}");
                return false;
            }
        }
        
        private DateTime ParseAsteroidDate(string dateStr)
        {
            // Parse: " A.D. 2025-Nov-05 00:00:00.0000"
            dateStr = dateStr.Replace("A.D.", "").Trim();
            
            // Remove milliseconds
            int dotIndex = dateStr.LastIndexOf('.');
            if (dotIndex > 0)
            {
                dateStr = dateStr.Substring(0, dotIndex);
            }
            
            return DateTime.ParseExact(
                dateStr,
                "yyyy-MMM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal
            );
        }
        
        public Vector3D GetPositionAtTime(DateTime targetTime)
        {
            if (dataPoints.Count == 0)
                return new Vector3D(0, 0, 0);
            
            // Clamp to available data range
            if (targetTime <= dataPoints[0].time)
            {
                var first = dataPoints[0];
                return new Vector3D(first.x_km * ProgramJustitia.KM_TO_M, 
                                   first.y_km * ProgramJustitia.KM_TO_M, 
                                   first.z_km * ProgramJustitia.KM_TO_M);
            }
            
            if (targetTime >= dataPoints[dataPoints.Count - 1].time)
            {
                var last = dataPoints[dataPoints.Count - 1];
                return new Vector3D(last.x_km * ProgramJustitia.KM_TO_M, 
                                   last.y_km * ProgramJustitia.KM_TO_M, 
                                   last.z_km * ProgramJustitia.KM_TO_M);
            }
            
            // Find interpolation points
            for (int i = 0; i < dataPoints.Count - 1; i++)
            {
                if (dataPoints[i].time <= targetTime && dataPoints[i + 1].time >= targetTime)
                {
                    var before = dataPoints[i];
                    var after = dataPoints[i + 1];
                    
                    double totalSec = (after.time - before.time).TotalSeconds;
                    double elapsedSec = (targetTime - before.time).TotalSeconds;
                    double t = elapsedSec / totalSec;
                    
                    double x = Lerp(before.x_km, after.x_km, t) * ProgramJustitia.KM_TO_M;
                    double y = Lerp(before.y_km, after.y_km, t) * ProgramJustitia.KM_TO_M;
                    double z = Lerp(before.z_km, after.z_km, t) * ProgramJustitia.KM_TO_M;
                    
                    return new Vector3D(x, y, z);
                }
            }
            
            return new Vector3D(0, 0, 0);
        }
        
        private double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
        
        public int GetDataPointCount() => dataPoints.Count;
    }
    
    // =============================================================================
    // CHROMOSOME STRUCTURE (20 genes: 2 launch + 6*3 maneuvers)
    // =============================================================================
    
    public class JustitiaChromosome
    {
        public double[] Genes { get; set; }  // 21 genes total (1 launch angle + 6*3 maneuvers)
        public double Fitness { get; set; }
        
        public JustitiaChromosome()
        {
            Genes = new double[19];  // 1 launch angle + 6 maneuvers * 3 params
            Fitness = double.MaxValue;
        }
        
        public JustitiaChromosome Clone()
        {
            var clone = new JustitiaChromosome();
            Array.Copy(Genes, clone.Genes, Genes.Length);
            clone.Fitness = Fitness;
            return clone;
        }
        
        // Gene accessors
        public double LaunchAngleDeg { get => Genes[0]; set => Genes[0] = value; }  // Angle relative to Earth velocity (0-360°)
        
        // Calculate launch velocity from angle (C3 is fixed)
        public (double vx, double vy) GetLaunchVelocity()
        {
            double v_infinity_km_s = Math.Sqrt(ProgramJustitia.C3_KM2_S2);
            double v_infinity = v_infinity_km_s * 1000.0;  // m/s
            
            double dla_rad = ProgramJustitia.DLA_DEG * Math.PI / 180.0;
            double v_ecliptic = v_infinity * Math.Cos(dla_rad);  // Project to ecliptic
            
            double angle_rad = LaunchAngleDeg * Math.PI / 180.0;
            double vx = v_ecliptic * Math.Sin(angle_rad);
            double vy = v_ecliptic * Math.Cos(angle_rad);
            
            return (vx, vy);
        }
        
        // 6 maneuvers (gene indices shifted by 1 since we only have 1 gene for launch now)
        public double Maneuver1Time { get => Genes[1]; set => Genes[1] = value; }
        public double Maneuver1DeltaVX { get => Genes[2]; set => Genes[2] = value; }
        public double Maneuver1DeltaVY { get => Genes[3]; set => Genes[3] = value; }
        
        public double Maneuver2Time { get => Genes[4]; set => Genes[4] = value; }
        public double Maneuver2DeltaVX { get => Genes[5]; set => Genes[5] = value; }
        public double Maneuver2DeltaVY { get => Genes[6]; set => Genes[6] = value; }
        
        public double Maneuver3Time { get => Genes[7]; set => Genes[7] = value; }
        public double Maneuver3DeltaVX { get => Genes[8]; set => Genes[8] = value; }
        public double Maneuver3DeltaVY { get => Genes[9]; set => Genes[9] = value; }
        
        public double Maneuver4Time { get => Genes[10]; set => Genes[10] = value; }
        public double Maneuver4DeltaVX { get => Genes[11]; set => Genes[11] = value; }
        public double Maneuver4DeltaVY { get => Genes[12]; set => Genes[12] = value; }
        
        public double Maneuver5Time { get => Genes[13]; set => Genes[13] = value; }
        public double Maneuver5DeltaVX { get => Genes[14]; set => Genes[14] = value; }
        public double Maneuver5DeltaVY { get => Genes[15]; set => Genes[15] = value; }
        
        public double Maneuver6Time { get => Genes[16]; set => Genes[16] = value; }
        public double Maneuver6DeltaVX { get => Genes[17]; set => Genes[17] = value; }
        public double Maneuver6DeltaVY { get => Genes[18]; set => Genes[18] = value; }
    }
    
    // =============================================================================
    // TRAJECTORY RESULT
    // =============================================================================
    
    public class JustitiaTrajectoryResult
    {
        // Planetary flyby tracking (mission critical windows)
        public double VenusFlybyDistance { get; set; }
        public double VenusFlybyTimeDays { get; set; }
        public double EarthFlybyDistance { get; set; }
        public double EarthFlybyTimeDays { get; set; }
        public double MarsFlybyDistance { get; set; }
        public double MarsFlybyTimeDays { get; set; }
        
        // Asteroid encounter tracking (in mission order)
        public Dictionary<string, double> AsteroidMinDistances { get; set; }
        public Dictionary<string, double> AsteroidEncounterTimes { get; set; }
        
        public double TotalDeltaV { get; set; }
        public double FuelConsumedKg { get; set; }
        public double FuelRemainingKg { get; set; }
        public bool OutOfFuel { get; set; }
        public double TimeDays { get; set; }
        public bool Crashed { get; set; }
        public bool Escaped { get; set; }
        
        public JustitiaTrajectoryResult()
        {
            VenusFlybyDistance = double.PositiveInfinity;
            VenusFlybyTimeDays = 0;
            EarthFlybyDistance = double.PositiveInfinity;
            EarthFlybyTimeDays = 0;
            MarsFlybyDistance = double.PositiveInfinity;
            MarsFlybyTimeDays = 0;
            
            AsteroidMinDistances = new Dictionary<string, double>
            {
                { "Westerwald", double.PositiveInfinity },
                { "Chimaera", double.PositiveInfinity },
                { "Rockox", double.PositiveInfinity },
                { "Moza", double.PositiveInfinity },
                { "Ousha", double.PositiveInfinity },
                { "Ghaf", double.PositiveInfinity },
                { "Justitia", double.PositiveInfinity }
            };
            
            AsteroidEncounterTimes = new Dictionary<string, double>
            {
                { "Westerwald", 0 },
                { "Chimaera", 0 },
                { "Rockox", 0 },
                { "Moza", 0 },
                { "Ousha", 0 },
                { "Ghaf", 0 },
                { "Justitia", 0 }
            };
            
            TotalDeltaV = 0;
            FuelConsumedKg = 0;
            FuelRemainingKg = ProgramJustitia.INITIAL_FUEL_MASS_KG;
            OutOfFuel = false;
            TimeDays = 0;
            Crashed = false;
            Escaped = false;
        }
        
        // Quick access property for Justitia (final target)
        public double MinDistanceToJustitia => AsteroidMinDistances["Justitia"];
        
        /// <summary>
        /// Calculate fuel consumed using Tsiolkovsky rocket equation
        /// Formula: fuel_used = m0 * (1 - exp(-ΔV / (Isp * g0)))
        /// </summary>
        public void CalculateFuelConsumption(double totalDeltaV)
        {
            double exhaustVelocity = ProgramJustitia.SPECIFIC_IMPULSE_S * ProgramJustitia.G0;
            double massRatio = Math.Exp(-totalDeltaV / exhaustVelocity);
            double finalMass = ProgramJustitia.TOTAL_INITIAL_MASS_KG * massRatio;
            
            FuelConsumedKg = ProgramJustitia.TOTAL_INITIAL_MASS_KG - finalMass;
            FuelRemainingKg = ProgramJustitia.INITIAL_FUEL_MASS_KG - FuelConsumedKg;
            
            // Check if we've exceeded available fuel
            if (FuelConsumedKg > ProgramJustitia.INITIAL_FUEL_MASS_KG)
            {
                OutOfFuel = true;
                FuelRemainingKg = 0;
            }
        }
    }
    
    // =============================================================================
    // VECTOR 3D HELPER
    // =============================================================================
    
    public struct Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static Vector3D operator +(Vector3D a, Vector3D b) => new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3D operator -(Vector3D a, Vector3D b) => new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3D operator *(Vector3D a, double scalar) => new Vector3D(a.X * scalar, a.Y * scalar, a.Z * scalar);
        
        public double Magnitude() => Math.Sqrt(X * X + Y * Y + Z * Z);
        
        public Vector3D Normalized()
        {
            double mag = Magnitude();
            if (mag < 1e-10) return new Vector3D(0, 0, 0);
            return new Vector3D(X / mag, Y / mag, Z / mag);
        }
    }
    
    // =============================================================================
    // GENETIC ALGORITHM
    // =============================================================================
    
    public class JustitiaGeneticAlgorithm
    {
        private const double TimeStep = 21600.0;  // 6 hours (faster simulation)
        private const int MaxDays = 2920;  // 8 years
        private const double MutationRate = 0.12;
        private const double MutationStrength = 0.25;
        private const int EliteCount = 3;
        
        private readonly int populationSize;
        private readonly int generations;
        private readonly Random random;
        private List<JustitiaChromosome> population;
        private JustitiaChromosome? bestChromosome;
        private double bestFitness;
        private readonly Dictionary<string, AsteroidEphemeris> asteroidEphemerides;
        private List<List<JustitiaChromosome>> generationHistory;  // Store last 10 generations
        
        public JustitiaGeneticAlgorithm(int populationSize, int generations, Dictionary<string, AsteroidEphemeris> ephemerides)
        {
            this.populationSize = populationSize;
            this.generations = generations;
            this.random = new Random();
            this.population = new List<JustitiaChromosome>(populationSize);
            this.bestFitness = double.MaxValue;
            this.asteroidEphemerides = ephemerides;
            this.generationHistory = new List<List<JustitiaChromosome>>();
        }
        
        public JustitiaChromosome Run()
        {
            InitializePopulation();
            
            for (int gen = 0; gen < generations; gen++)
            {
                // Evaluate fitness
                EvaluatePopulation();
                
                // Track best
                var best = population.OrderBy(c => c.Fitness).First();
                if (best.Fitness < bestFitness)
                {
                    bestFitness = best.Fitness;
                    bestChromosome = best.Clone();
                }
                
                // Store generation history (last 10 generations, top 3 from each)
                if (gen >= generations - 10)
                {
                    var topChromosomes = population.OrderBy(c => c.Fitness).Take(3).Select(c => c.Clone()).ToList();
                    generationHistory.Add(topChromosomes);
                }
                
                // Log progress
                if (gen % 5 == 0 || gen == generations - 1)
                {
                    var result = SimulateTrajectory(best);
                    double fuelPercent = (result.FuelConsumedKg / ProgramJustitia.INITIAL_FUEL_MASS_KG) * 100;
                    
                    // Count successful planetary flybys
                    int planetFlybys = 0;
                    if (result.VenusFlybyDistance < 0.05 * ProgramJustitia.AU) planetFlybys++;
                    if (result.EarthFlybyDistance < 0.05 * ProgramJustitia.AU) planetFlybys++;
                    if (result.MarsFlybyDistance < 0.05 * ProgramJustitia.AU) planetFlybys++;
                    
                    // Count successful asteroid flybys
                    string[] flybyAsteroids = { "Westerwald", "Chimaera", "Rockox", "Moza", "Ousha", "Ghaf" };
                    int asteroidFlybys = flybyAsteroids.Count(a => result.AsteroidMinDistances[a] < 0.2 * ProgramJustitia.AU);
                    
                    Console.WriteLine($"Gen {gen,3} | Fit: {best.Fitness,7:F1} | " +
                                    $"Planets: {planetFlybys}/3 | " +
                                    $"Asteroids: {asteroidFlybys}/6 | " +
                                    $"Jus: {result.MinDistanceToJustitia / ProgramJustitia.AU,6:F4}AU | " +
                                    $"Fuel: {fuelPercent,4:F0}%");
                }
                
                // Create next generation
                CreateNextGeneration();
            }
            
            // Display final results
            DisplayResults();
            
            return bestChromosome ?? population[0];
        }
        
        private void InitializePopulation()
        {
            // Create seeded solutions with varying launch angles
            // C3 is FIXED, but direction can be optimized
            for (int i = 0; i < 10; i++)
            {
                var seed = new JustitiaChromosome
                {
                    LaunchAngleDeg = RandomDouble(10, 50),  // Typical outbound angles for asteroid belt
                    
                    Maneuver1Time = RandomDouble(100, 400),
                    Maneuver1DeltaVX = RandomDouble(-1000, 1000),
                    Maneuver1DeltaVY = RandomDouble(-1000, 1000),
                    
                    Maneuver2Time = RandomDouble(400, 800),
                    Maneuver2DeltaVX = RandomDouble(-1000, 1000),
                    Maneuver2DeltaVY = RandomDouble(-1000, 1000),
                    
                    Maneuver3Time = RandomDouble(800, 1200),
                    Maneuver3DeltaVX = RandomDouble(-800, 800),
                    Maneuver3DeltaVY = RandomDouble(-800, 800),
                    
                    Maneuver4Time = RandomDouble(1200, 1800),
                    Maneuver4DeltaVX = RandomDouble(-800, 800),
                    Maneuver4DeltaVY = RandomDouble(-800, 800),
                    
                    Maneuver5Time = RandomDouble(1800, 2300),
                    Maneuver5DeltaVX = RandomDouble(-600, 600),
                    Maneuver5DeltaVY = RandomDouble(-600, 600),
                    
                    Maneuver6Time = RandomDouble(2300, 2800),
                    Maneuver6DeltaVX = RandomDouble(-600, 600),
                    Maneuver6DeltaVY = RandomDouble(-600, 600)
                };
                population.Add(seed);
            }
            
            // Fill rest with random
            while (population.Count < populationSize)
            {
                population.Add(CreateRandomChromosome());
            }
            
            Console.WriteLine($"✓ Initialized population with {populationSize} chromosomes\n");
        }
        
        private JustitiaChromosome CreateRandomChromosome()
        {
            return new JustitiaChromosome
            {
                LaunchAngleDeg = RandomDouble(0, 360),  // Random launch direction (C3 fixed)
                
                Maneuver1Time = RandomDouble(50, 600),
                Maneuver1DeltaVX = RandomDouble(-2000, 2000),
                Maneuver1DeltaVY = RandomDouble(-2000, 2000),
                
                Maneuver2Time = RandomDouble(300, 1000),
                Maneuver2DeltaVX = RandomDouble(-2000, 2000),
                Maneuver2DeltaVY = RandomDouble(-2000, 2000),
                
                Maneuver3Time = RandomDouble(700, 1500),
                Maneuver3DeltaVX = RandomDouble(-1500, 1500),
                Maneuver3DeltaVY = RandomDouble(-1500, 1500),
                
                Maneuver4Time = RandomDouble(1200, 2000),
                Maneuver4DeltaVX = RandomDouble(-1500, 1500),
                Maneuver4DeltaVY = RandomDouble(-1500, 1500),
                
                Maneuver5Time = RandomDouble(1700, 2500),
                Maneuver5DeltaVX = RandomDouble(-1000, 1000),
                Maneuver5DeltaVY = RandomDouble(-1000, 1000),
                
                Maneuver6Time = RandomDouble(2300, 2900),
                Maneuver6DeltaVX = RandomDouble(-1000, 1000),
                Maneuver6DeltaVY = RandomDouble(-1000, 1000)
            };
        }
        
        private void EvaluatePopulation()
        {
            foreach (var chromosome in population)
            {
                chromosome.Fitness = CalculateFitness(chromosome);
            }
        }
        
        private double CalculateFitness(JustitiaChromosome chromosome)
        {
            var result = SimulateTrajectory(chromosome);
            
            // Severe penalties
            if (result.Crashed) return 50000.0;
            if (result.Escaped) return 50000.0;
            if (result.OutOfFuel) return 45000.0;
            
            // ==== MBR EXPLORER MISSION FITNESS ====
            // Mission constraints (in order of importance):
            // 1. HIT PLANETARY FLYBYS (Venus, Earth, Mars) - CRITICAL for gravity assists
            // 2. Fly by 6 asteroids (Westerwald, Chimaera, Rockox, Ghaf, Ousha, Moza)
            // 3. Arrive at Justitia for landing
            // 4. Minimize fuel consumption
            
            double totalFitness = 0;
            
            // ===== PLANETARY FLYBYS (MISSION CRITICAL) =====
            // Must hit these windows for gravity assists
            double planetaryPenalty = 0;
            int successfulPlanetFlybys = 0;
            
            // Venus flyby (July 2028 - ~135 days after launch)
            double venusDistAU = result.VenusFlybyDistance / ProgramJustitia.AU;
            double venusExpectedDay = (ProgramJustitia.VENUS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double venusTimingError = Math.Abs(result.VenusFlybyTimeDays - venusExpectedDay);
            
            if (double.IsInfinity(venusDistAU) || venusDistAU > 0.05)
            {
                planetaryPenalty += 1000.0;  // MISSED VENUS - huge problem
            }
            else
            {
                successfulPlanetFlybys++;
                planetaryPenalty += venusTimingError * 0.5;  // Timing penalty
                if (venusDistAU < 0.02) planetaryPenalty += 20.0; // Too close!
            }
            
            // Earth flyby (May 2029 - ~439 days)
            double earthDistAU = result.EarthFlybyDistance / ProgramJustitia.AU;
            double earthExpectedDay = (ProgramJustitia.EARTH_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double earthTimingError = Math.Abs(result.EarthFlybyTimeDays - earthExpectedDay);
            
            if (double.IsInfinity(earthDistAU) || earthDistAU > 0.05)
            {
                planetaryPenalty += 1000.0;  // MISSED EARTH - huge problem
            }
            else
            {
                successfulPlanetFlybys++;
                planetaryPenalty += earthTimingError * 0.5;
                if (earthDistAU < 0.02) planetaryPenalty += 20.0;
            }
            
            // Mars flyby (Sept 2031 - ~1292 days)
            double marsDistAU = result.MarsFlybyDistance / ProgramJustitia.AU;
            double marsExpectedDay = (ProgramJustitia.MARS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double marsTimingError = Math.Abs(result.MarsFlybyTimeDays - marsExpectedDay);
            
            if (double.IsInfinity(marsDistAU) || marsDistAU > 0.05)
            {
                planetaryPenalty += 1000.0;  // MISSED MARS - huge problem
            }
            else
            {
                successfulPlanetFlybys++;
                planetaryPenalty += marsTimingError * 0.5;
                if (marsDistAU < 0.02) planetaryPenalty += 20.0;
            }
            
            // Bonus for hitting all planetary windows
            double planetaryBonus = successfulPlanetFlybys == 3 ? 200.0 : 0;
            
            // FLYBY ASTEROIDS (Westerwald, Chimaera, Rockox, Moza, Ousha, Ghaf)
            // Target: Get within 0.01-0.1 AU (sweet spot for scientific observations)
            string[] flybyAsteroids = { "Westerwald", "Chimaera", "Rockox", "Moza", "Ousha", "Ghaf" };
            double flybyPenalty = 0;
            int successfulFlybys = 0;
            
            foreach (var asteroid in flybyAsteroids)
            {
                double distAU = result.AsteroidMinDistances[asteroid] / ProgramJustitia.AU;
                
                if (double.IsInfinity(distAU))
                {
                    flybyPenalty += 500.0;  // Huge penalty for missing an asteroid
                }
                else if (distAU < 0.01)
                {
                    // Too close - risky!
                    flybyPenalty += (0.01 - distAU) * 200.0;
                }
                else if (distAU > 0.2)
                {
                    // Too far - not much science
                    flybyPenalty += (distAU - 0.2) * 100.0;
                }
                else
                {
                    // Sweet spot! Reward this
                    successfulFlybys++;
                    flybyPenalty += (distAU - 0.05) * 10.0;  // Prefer ~0.05 AU
                }
            }
            
            // JUSTITIA - LANDING TARGET (must get VERY close!)
            double justitiaDistAU = result.MinDistanceToJustitia / ProgramJustitia.AU;
            double justitiaPenalty = 0;
            double justitiaBonus = 0;
            
            if (double.IsInfinity(justitiaDistAU))
            {
                justitiaPenalty = 1000.0;  // MISSION FAILURE
            }
            else if (justitiaDistAU > 0.01)
            {
                // Not close enough for landing!
                justitiaPenalty = justitiaDistAU * 200.0;
            }
            else
            {
                // Close enough for landing! Big bonus!
                justitiaBonus = 500.0 / (justitiaDistAU / 0.001 + 1.0);
            }
            
            // Fuel consumption penalty
            double fuelUsageFraction = result.FuelConsumedKg / ProgramJustitia.INITIAL_FUEL_MASS_KG;
            double fuelPenalty = fuelUsageFraction * 8.0;  // Important for multi-flyby mission
            
            // Fuel efficiency bonus
            double fuelBonus = 0;
            if (fuelUsageFraction < 0.6 && successfulFlybys >= 4)  
            {
                fuelBonus = (0.6 - fuelUsageFraction) * 5.0;
            }
            
            // Time penalty (8 years is OK, but prefer shorter)
            double timePenalty = (result.TimeDays / 2920.0) * 0.3;
            
            // Bonus for completing full grand tour
            double grandTourBonus = 0;
            if (successfulPlanetFlybys == 3 && successfulFlybys == 6 && justitiaDistAU < 0.01)
            {
                grandTourBonus = 300.0;  // COMPLETE MISSION SUCCESS!
            }
            else if (successfulPlanetFlybys == 3 && successfulFlybys >= 4)
            {
                grandTourBonus = 50.0 + (20.0 * successfulFlybys);
            }
            
            totalFitness = planetaryPenalty + flybyPenalty + justitiaPenalty + fuelPenalty + timePenalty 
                          - planetaryBonus - justitiaBonus - fuelBonus - grandTourBonus;
            
            return Math.Max(0, totalFitness);  // Never negative
        }
        
        private JustitiaTrajectoryResult SimulateTrajectory(JustitiaChromosome chromosome)
        {
            var result = new JustitiaTrajectoryResult();
            
            int maxSteps = (int)(MaxDays * 24 / (TimeStep / 3600.0));
            
            // Get Earth's starting position
            var currentTime = new AstroTime(ProgramJustitia.START_DATE);
            var currentDateTime = ProgramJustitia.START_DATE;
            
            var earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
            var earthPos = new Vector3D(earthPosVec.x * ProgramJustitia.AU, 
                                       earthPosVec.y * ProgramJustitia.AU, 
                                       earthPosVec.z * ProgramJustitia.AU);
            
            // Initialize spacecraft
            var craftPos = earthPos;
            double earthOrbitalVel = 29780.0;
            
            // Get launch velocity from C3 and optimized angle
            var (launchVx, launchVy) = chromosome.GetLaunchVelocity();
            var craftVel = new Vector3D(launchVx, 
                                       earthOrbitalVel + launchVy, 
                                       0);
            
            // Launch delta-v is fixed by C3 (don't count this in total - it's from launch vehicle)
            result.TotalDeltaV = 0;  // We'll add maneuver delta-vs only
            
            // Maneuver schedule
            int[] maneuverSteps = new int[6];
            maneuverSteps[0] = (int)(chromosome.Maneuver1Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[1] = (int)(chromosome.Maneuver2Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[2] = (int)(chromosome.Maneuver3Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[3] = (int)(chromosome.Maneuver4Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[4] = (int)(chromosome.Maneuver5Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[5] = (int)(chromosome.Maneuver6Time * 24 / (TimeStep / 3600.0));
            
            double[,] maneuverDvs = new double[6, 2];
            maneuverDvs[0, 0] = chromosome.Maneuver1DeltaVX; maneuverDvs[0, 1] = chromosome.Maneuver1DeltaVY;
            maneuverDvs[1, 0] = chromosome.Maneuver2DeltaVX; maneuverDvs[1, 1] = chromosome.Maneuver2DeltaVY;
            maneuverDvs[2, 0] = chromosome.Maneuver3DeltaVX; maneuverDvs[2, 1] = chromosome.Maneuver3DeltaVY;
            maneuverDvs[3, 0] = chromosome.Maneuver4DeltaVX; maneuverDvs[3, 1] = chromosome.Maneuver4DeltaVY;
            maneuverDvs[4, 0] = chromosome.Maneuver5DeltaVX; maneuverDvs[4, 1] = chromosome.Maneuver5DeltaVY;
            maneuverDvs[5, 0] = chromosome.Maneuver6DeltaVX; maneuverDvs[5, 1] = chromosome.Maneuver6DeltaVY;
            
            for (int step = 0; step < maxSteps; step++)
            {
                // Apply maneuvers
                for (int m = 0; m < 6; m++)
                {
                    if (step == maneuverSteps[m])
                    {
                        double dvMag = Math.Sqrt(maneuverDvs[m, 0] * maneuverDvs[m, 0] + 
                                                maneuverDvs[m, 1] * maneuverDvs[m, 1]);
                        result.TotalDeltaV += dvMag;
                        craftVel += new Vector3D(maneuverDvs[m, 0], maneuverDvs[m, 1], 0);
                    }
                }
                
                // Calculate gravitational forces from all major bodies
                var force = new Vector3D(0, 0, 0);
                
                // Sun (always calculated)
                var toSun = new Vector3D(0, 0, 0) - craftPos;
                double distToSun = toSun.Magnitude();
                if (distToSun > 1000)
                {
                    force += toSun.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Sun] / (distToSun * distToSun));
                }
                
                // Venus
                var venusPosVec = Astronomy.HelioVector(Body.Venus, currentTime);
                var venusPos = new Vector3D(venusPosVec.x * ProgramJustitia.AU, venusPosVec.y * ProgramJustitia.AU, venusPosVec.z * ProgramJustitia.AU);
                var toVenus = venusPos - craftPos;
                double distToVenus = toVenus.Magnitude();
                if (distToVenus < 1.5 * ProgramJustitia.AU && distToVenus > 1000)
                {
                    force += toVenus.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Venus] / (distToVenus * distToVenus));
                }
                
                // Earth
                earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
                earthPos = new Vector3D(earthPosVec.x * ProgramJustitia.AU, 
                                       earthPosVec.y * ProgramJustitia.AU, 
                                       earthPosVec.z * ProgramJustitia.AU);
                var toEarth = earthPos - craftPos;
                double distToEarth = toEarth.Magnitude();
                if (distToEarth < 1.5 * ProgramJustitia.AU && distToEarth > 1000)
                {
                    force += toEarth.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Earth] / (distToEarth * distToEarth));
                }
                
                // Mars
                var marsPosVec = Astronomy.HelioVector(Body.Mars, currentTime);
                var marsPos = new Vector3D(marsPosVec.x * ProgramJustitia.AU, marsPosVec.y * ProgramJustitia.AU, marsPosVec.z * ProgramJustitia.AU);
                var toMars = marsPos - craftPos;
                double distToMars = toMars.Magnitude();
                if (distToMars < 2.0 * ProgramJustitia.AU && distToMars > 1000)
                {
                    force += toMars.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Mars] / (distToMars * distToMars));
                }
                
                // Jupiter
                var jupiterPosVec = Astronomy.HelioVector(Body.Jupiter, currentTime);
                var jupiterPos = new Vector3D(jupiterPosVec.x * ProgramJustitia.AU, jupiterPosVec.y * ProgramJustitia.AU, jupiterPosVec.z * ProgramJustitia.AU);
                var toJupiter = jupiterPos - craftPos;
                double distToJupiter = toJupiter.Magnitude();
                if (distToJupiter < 7.0 * ProgramJustitia.AU && distToJupiter > 1000)
                {
                    force += toJupiter.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Jupiter] / (distToJupiter * distToJupiter));
                }
                
                // Saturn (for outer region assists)
                var saturnPosVec = Astronomy.HelioVector(Body.Saturn, currentTime);
                var saturnPos = new Vector3D(saturnPosVec.x * ProgramJustitia.AU, saturnPosVec.y * ProgramJustitia.AU, saturnPosVec.z * ProgramJustitia.AU);
                var toSaturn = saturnPos - craftPos;
                double distToSaturn = toSaturn.Magnitude();
                if (distToSaturn < 12.0 * ProgramJustitia.AU && distToSaturn > 1000)
                {
                    force += toSaturn.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Saturn] / (distToSaturn * distToSaturn));
                }
                
                // Update velocity and position
                craftVel += force * TimeStep;
                craftPos += craftVel * TimeStep;
                
                // Check distances to planets and asteroids every 10 steps
                if (step % 10 == 0)
                {
                    double currentTimeDays = step * TimeStep / 86400.0;
                    
                    // Track planetary flybys around expected dates (±30 day window)
                    double venusWindowDays = (ProgramJustitia.VENUS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
                    if (Math.Abs(currentTimeDays - venusWindowDays) < 30)
                    {
                        double venusFlybyDist = (craftPos - venusPos).Magnitude();
                        if (venusFlybyDist < result.VenusFlybyDistance)
                        {
                            result.VenusFlybyDistance = venusFlybyDist;
                            result.VenusFlybyTimeDays = currentTimeDays;
                        }
                    }
                    
                    double earthWindowDays = (ProgramJustitia.EARTH_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
                    if (Math.Abs(currentTimeDays - earthWindowDays) < 30)
                    {
                        double earthFlybyDist = (craftPos - earthPos).Magnitude();
                        if (earthFlybyDist < result.EarthFlybyDistance)
                        {
                            result.EarthFlybyDistance = earthFlybyDist;
                            result.EarthFlybyTimeDays = currentTimeDays;
                        }
                    }
                    
                    double marsWindowDays = (ProgramJustitia.MARS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
                    if (Math.Abs(currentTimeDays - marsWindowDays) < 30)
                    {
                        double marsFlybyDist = (craftPos - marsPos).Magnitude();
                        if (marsFlybyDist < result.MarsFlybyDistance)
                        {
                            result.MarsFlybyDistance = marsFlybyDist;
                            result.MarsFlybyTimeDays = currentTimeDays;
                        }
                    }
                    
                    // Track all asteroid encounters
                    foreach (var asteroidName in asteroidEphemerides.Keys)
                    {
                        var asteroidPos = asteroidEphemerides[asteroidName].GetPositionAtTime(currentDateTime);
                        double distance = (craftPos - asteroidPos).Magnitude();
                        
                        if (distance < result.AsteroidMinDistances[asteroidName])
                        {
                            result.AsteroidMinDistances[asteroidName] = distance;
                            result.AsteroidEncounterTimes[asteroidName] = currentTimeDays;
                        }
                    }
                    
                    // Update overall mission time
                    result.TimeDays = currentTimeDays;
                }
                
                // Check for crash or escape
                if (distToSun < 0.1 * ProgramJustitia.AU)
                {
                    result.Crashed = true;
                    break;
                }
                
                if (distToSun > 10 * ProgramJustitia.AU)
                {
                    result.Escaped = true;
                    break;
                }
                
                // Update time
                currentDateTime = currentDateTime.AddSeconds(TimeStep);
                currentTime = new AstroTime(currentDateTime);
            }
            
            // Calculate fuel consumption based on total delta-v
            result.CalculateFuelConsumption(result.TotalDeltaV);
            
            return result;
        }
        
        private void CreateNextGeneration()
        {
            var newPopulation = new List<JustitiaChromosome>(populationSize);
            
            // Keep elite
            var sorted = population.OrderBy(c => c.Fitness).ToList();
            for (int i = 0; i < EliteCount && i < sorted.Count; i++)
            {
                newPopulation.Add(sorted[i].Clone());
            }
            
            // Create rest through crossover and mutation
            while (newPopulation.Count < populationSize)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();
                
                var (child1, child2) = Crossover(parent1, parent2);
                
                child1 = Mutate(child1);
                child2 = Mutate(child2);
                
                newPopulation.Add(child1);
                if (newPopulation.Count < populationSize)
                    newPopulation.Add(child2);
            }
            
            population = newPopulation;
        }
        
        private JustitiaChromosome TournamentSelection(int tournamentSize = 3)
        {
            JustitiaChromosome? best = null;
            double bestFit = double.MaxValue;
            
            for (int i = 0; i < tournamentSize; i++)
            {
                int idx = random.Next(population.Count);
                if (population[idx].Fitness < bestFit)
                {
                    best = population[idx];
                    bestFit = population[idx].Fitness;
                }
            }
            
            return best!;
        }
        
        private (JustitiaChromosome, JustitiaChromosome) Crossover(JustitiaChromosome parent1, JustitiaChromosome parent2)
        {
            var child1 = new JustitiaChromosome();
            var child2 = new JustitiaChromosome();
            
            int crossPoint = random.Next(1, parent1.Genes.Length);
            
            for (int i = 0; i < parent1.Genes.Length; i++)
            {
                if (i < crossPoint)
                {
                    child1.Genes[i] = parent1.Genes[i];
                    child2.Genes[i] = parent2.Genes[i];
                }
                else
                {
                    child1.Genes[i] = parent2.Genes[i];
                    child2.Genes[i] = parent1.Genes[i];
                }
            }
            
            return (child1, child2);
        }
        
        private JustitiaChromosome Mutate(JustitiaChromosome chromosome)
        {
            var mutated = chromosome.Clone();
            
            for (int i = 0; i < mutated.Genes.Length; i++)
            {
                if (random.NextDouble() < MutationRate)
                {
                    double noise = RandomGaussian() * (Math.Abs(mutated.Genes[i]) * MutationStrength + 100);
                    mutated.Genes[i] += noise;
                    
                    // Enforce bounds
                    if (i == 0)
                    {
                        // Launch angle (0-360°)
                        mutated.Genes[i] = Clamp(mutated.Genes[i], 0, 360);
                    }
                    else if ((i - 1) % 3 == 0)
                    {
                        // Maneuver time
                        mutated.Genes[i] = Clamp(mutated.Genes[i], 50, 2900);
                    }
                    else
                    {
                        // Delta-V components
                        mutated.Genes[i] = Clamp(mutated.Genes[i], -2000, 2000);
                    }
                }
            }
            
            return mutated;
        }
        
        private void DisplayResults()
        {
            if (bestChromosome == null)
            {
                Console.WriteLine("⚠️  No best solution found!");
                return;
            }
            
            var result = SimulateTrajectory(bestChromosome);
            
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("🎯 MBR EXPLORER - MULTI-ASTEROID GRAND TOUR OPTIMIZATION COMPLETE!");
            Console.WriteLine(new string('=', 80));
            
            // Display planetary gravity assists
            Console.WriteLine("\n🪐 PLANETARY GRAVITY ASSISTS:");
            
            // Venus
            double venusDist = result.VenusFlybyDistance / ProgramJustitia.AU;
            string venusStatus = venusDist < 0.05 ? "✅ SUCCESS" : "❌ MISSED";
            double venusExpected = (ProgramJustitia.VENUS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double venusError = Math.Abs(result.VenusFlybyTimeDays - venusExpected);
            Console.WriteLine($"   Venus  | Target: Day {venusExpected,4:F0} | Actual: Day {result.VenusFlybyTimeDays,4:F0} | " +
                            $"Δt: {venusError,3:F0}d | Dist: {venusDist,7:F5} AU | {venusStatus}");
            
            // Earth
            double earthDist = result.EarthFlybyDistance / ProgramJustitia.AU;
            string earthStatus = earthDist < 0.05 ? "✅ SUCCESS" : "❌ MISSED";
            double earthExpected = (ProgramJustitia.EARTH_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double earthError = Math.Abs(result.EarthFlybyTimeDays - earthExpected);
            Console.WriteLine($"   Earth  | Target: Day {earthExpected,4:F0} | Actual: Day {result.EarthFlybyTimeDays,4:F0} | " +
                            $"Δt: {earthError,3:F0}d | Dist: {earthDist,7:F5} AU | {earthStatus}");
            
            // Mars
            double marsDist = result.MarsFlybyDistance / ProgramJustitia.AU;
            string marsStatus = marsDist < 0.05 ? "✅ SUCCESS" : "❌ MISSED";
            double marsExpected = (ProgramJustitia.MARS_FLYBY_DATE - ProgramJustitia.START_DATE).TotalDays;
            double marsError = Math.Abs(result.MarsFlybyTimeDays - marsExpected);
            Console.WriteLine($"   Mars   | Target: Day {marsExpected,4:F0} | Actual: Day {result.MarsFlybyTimeDays,4:F0} | " +
                            $"Δt: {marsError,3:F0}d | Dist: {marsDist,7:F5} AU | {marsStatus}");
            
            // Display asteroid encounters
            Console.WriteLine("\n🌑 ASTEROID FLYBY ENCOUNTERS:");
            string[] asteroidOrder = { "Westerwald", "Chimaera", "Rockox", "Moza", "Ousha", "Ghaf", "Justitia" };
            int flybyCount = 0;
            
            foreach (var asteroid in asteroidOrder)
            {
                double distAU = result.AsteroidMinDistances[asteroid] / ProgramJustitia.AU;
                double distKm = result.AsteroidMinDistances[asteroid] / 1000.0;
                double encounterDay = result.AsteroidEncounterTimes[asteroid];
                
                bool isTarget = asteroid == "Justitia";
                string status;
                
                if (double.IsInfinity(distAU))
                {
                    status = "❌ MISSED";
                }
                else if (isTarget)
                {
                    status = distAU < 0.001 ? "🎯 LANDING" : distAU < 0.01 ? "✅ CLOSE APPROACH" : "⚠️  DISTANT";
                }
                else
                {
                    if (distAU < 0.01) status = "⚠️  TOO CLOSE";
                    else if (distAU < 0.05) status = "✅ OPTIMAL FLYBY";
                    else if (distAU < 0.2) status = "✅ FLYBY";
                    else status = "⚠️  DISTANT";
                    
                    if (distAU < 0.2) flybyCount++;
                }
                
                Console.WriteLine($"  {(isTarget ? "🏁" : "  ")} {asteroid,-12} | {distAU,8:F5} AU ({distKm,10:F0} km) | Day {encounterDay,5:F0} | {status}");
            }
            
            Console.WriteLine($"\n  📊 Flyby Success Rate: {flybyCount}/6 asteroids");
            
            Console.WriteLine($"\n⛽ MISSION SUMMARY:");
            Console.WriteLine($"  • Total Delta-V: {result.TotalDeltaV:F0} m/s");
            Console.WriteLine($"  • Mission Time: {result.TimeDays:F0} days ({result.TimeDays / 365:F2} years)");
            Console.WriteLine($"  • Fitness Score: {bestFitness:F2}");
            
            Console.WriteLine($"\n⛽ Fuel Consumption:");
            Console.WriteLine($"  • Spacecraft Dry Mass: {ProgramJustitia.SPACECRAFT_DRY_MASS_KG:F0} kg");
            Console.WriteLine($"  • Initial Fuel: {ProgramJustitia.INITIAL_FUEL_MASS_KG:F0} kg");
            Console.WriteLine($"  • Fuel Consumed: {result.FuelConsumedKg:F1} kg ({result.FuelConsumedKg / ProgramJustitia.INITIAL_FUEL_MASS_KG * 100:F1}%)");
            Console.WriteLine($"  • Fuel Remaining: {result.FuelRemainingKg:F1} kg ({result.FuelRemainingKg / ProgramJustitia.INITIAL_FUEL_MASS_KG * 100:F1}%)");
            
            // Fuel status thresholds scaled for MBR Explorer (1260 kg total)
            string fuelStatus = result.FuelRemainingKg > 300 ? "✅ SAFE" : 
                               result.FuelRemainingKg > 100 ? "⚠️  LOW" : "🔴 CRITICAL";
            Console.WriteLine($"  • Mission Status: {fuelStatus}");
            
            Console.WriteLine($"\n🚀 Optimized Maneuver Schedule:");
            var (optLaunchVx, optLaunchVy) = bestChromosome.GetLaunchVelocity();
            Console.WriteLine($"  • Launch (Day 0): Angle={bestChromosome.LaunchAngleDeg:F1}° | C3={ProgramJustitia.C3_KM2_S2:F1}km²/s² | ΔV=[{optLaunchVx:F0},{optLaunchVy:F0}]m/s");
            Console.WriteLine($"  • Maneuver 1 (Day {bestChromosome.Maneuver1Time:F0}): ΔV = [{bestChromosome.Maneuver1DeltaVX:F0}, {bestChromosome.Maneuver1DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 2 (Day {bestChromosome.Maneuver2Time:F0}): ΔV = [{bestChromosome.Maneuver2DeltaVX:F0}, {bestChromosome.Maneuver2DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 3 (Day {bestChromosome.Maneuver3Time:F0}): ΔV = [{bestChromosome.Maneuver3DeltaVX:F0}, {bestChromosome.Maneuver3DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 4 (Day {bestChromosome.Maneuver4Time:F0}): ΔV = [{bestChromosome.Maneuver4DeltaVX:F0}, {bestChromosome.Maneuver4DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 5 (Day {bestChromosome.Maneuver5Time:F0}): ΔV = [{bestChromosome.Maneuver5DeltaVX:F0}, {bestChromosome.Maneuver5DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 6 (Day {bestChromosome.Maneuver6Time:F0}): ΔV = [{bestChromosome.Maneuver6DeltaVX:F0}, {bestChromosome.Maneuver6DeltaVY:F0}] m/s");
            Console.WriteLine(new string('=', 80));
        }
        
        public void SimulateAndExportTrajectory(JustitiaChromosome chromosome)
        {
            Console.WriteLine("📍 Simulating complete trajectory with full detail...");
            
            var positions = new List<(double x, double y, double z, DateTime time)>();
            
            // Store all asteroid positions
            var allAsteroidPositions = new Dictionary<string, List<(double x, double y, double z)>>();
            foreach (var asteroidName in asteroidEphemerides.Keys)
            {
                allAsteroidPositions[asteroidName] = new List<(double x, double y, double z)>();
            }
            
            // Planet positions
            var venusPositions = new List<(double x, double y, double z)>();
            var earthPositions = new List<(double x, double y, double z)>();
            var marsPositions = new List<(double x, double y, double z)>();
            var jupiterPositions = new List<(double x, double y, double z)>();
            var saturnPositions = new List<(double x, double y, double z)>();
            
            int maxSteps = (int)(MaxDays * 24 / (TimeStep / 3600.0));
            
            var currentTime = new AstroTime(ProgramJustitia.START_DATE);
            var currentDateTime = ProgramJustitia.START_DATE;
            
            var earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
            var earthPos = new Vector3D(earthPosVec.x * ProgramJustitia.AU, 
                                       earthPosVec.y * ProgramJustitia.AU, 
                                       earthPosVec.z * ProgramJustitia.AU);
            
            var craftPos = earthPos;
            double earthOrbitalVel = 29780.0;
            
            // Get launch velocity from C3 and optimized angle
            var (launchVx, launchVy) = chromosome.GetLaunchVelocity();
            var craftVel = new Vector3D(launchVx, 
                                       earthOrbitalVel + launchVy, 
                                       0);
            
            // Maneuver schedule
            int[] maneuverSteps = new int[6];
            maneuverSteps[0] = (int)(chromosome.Maneuver1Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[1] = (int)(chromosome.Maneuver2Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[2] = (int)(chromosome.Maneuver3Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[3] = (int)(chromosome.Maneuver4Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[4] = (int)(chromosome.Maneuver5Time * 24 / (TimeStep / 3600.0));
            maneuverSteps[5] = (int)(chromosome.Maneuver6Time * 24 / (TimeStep / 3600.0));
            
            double[,] maneuverDvs = new double[6, 2];
            maneuverDvs[0, 0] = chromosome.Maneuver1DeltaVX; maneuverDvs[0, 1] = chromosome.Maneuver1DeltaVY;
            maneuverDvs[1, 0] = chromosome.Maneuver2DeltaVX; maneuverDvs[1, 1] = chromosome.Maneuver2DeltaVY;
            maneuverDvs[2, 0] = chromosome.Maneuver3DeltaVX; maneuverDvs[2, 1] = chromosome.Maneuver3DeltaVY;
            maneuverDvs[3, 0] = chromosome.Maneuver4DeltaVX; maneuverDvs[3, 1] = chromosome.Maneuver4DeltaVY;
            maneuverDvs[4, 0] = chromosome.Maneuver5DeltaVX; maneuverDvs[4, 1] = chromosome.Maneuver5DeltaVY;
            maneuverDvs[5, 0] = chromosome.Maneuver6DeltaVX; maneuverDvs[5, 1] = chromosome.Maneuver6DeltaVY;
            
            Console.WriteLine($"Simulating {maxSteps} timesteps ({maxSteps * TimeStep / 86400:F0} days)...");
            
            for (int step = 0; step < maxSteps; step++)
            {
                // Apply maneuvers
                for (int m = 0; m < 6; m++)
                {
                    if (step == maneuverSteps[m])
                    {
                        craftVel += new Vector3D(maneuverDvs[m, 0], maneuverDvs[m, 1], 0);
                        Console.WriteLine($"⚡ Maneuver {m + 1} applied at day {step * TimeStep / 86400:F1}");
                    }
                }
                
                // Calculate gravitational forces from all major bodies
                var force = new Vector3D(0, 0, 0);
                
                // Sun (always calculated)
                var toSun = new Vector3D(0, 0, 0) - craftPos;
                double distToSun = toSun.Magnitude();
                if (distToSun > 1000)
                {
                    force += toSun.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Sun] / (distToSun * distToSun));
                }
                
                // Venus
                var venusPosVec = Astronomy.HelioVector(Body.Venus, currentTime);
                var venusPos = new Vector3D(venusPosVec.x * ProgramJustitia.AU, venusPosVec.y * ProgramJustitia.AU, venusPosVec.z * ProgramJustitia.AU);
                var toVenus = venusPos - craftPos;
                double distToVenus = toVenus.Magnitude();
                if (distToVenus < 1.5 * ProgramJustitia.AU && distToVenus > 1000)
                {
                    force += toVenus.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Venus] / (distToVenus * distToVenus));
                }
                
                // Earth
                earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
                earthPos = new Vector3D(earthPosVec.x * ProgramJustitia.AU, 
                                       earthPosVec.y * ProgramJustitia.AU, 
                                       earthPosVec.z * ProgramJustitia.AU);
                var toEarth = earthPos - craftPos;
                double distToEarth = toEarth.Magnitude();
                if (distToEarth < 1.5 * ProgramJustitia.AU && distToEarth > 1000)
                {
                    force += toEarth.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Earth] / (distToEarth * distToEarth));
                }
                
                // Mars
                var marsPosVec = Astronomy.HelioVector(Body.Mars, currentTime);
                var marsPos = new Vector3D(marsPosVec.x * ProgramJustitia.AU, marsPosVec.y * ProgramJustitia.AU, marsPosVec.z * ProgramJustitia.AU);
                var toMars = marsPos - craftPos;
                double distToMars = toMars.Magnitude();
                if (distToMars < 2.0 * ProgramJustitia.AU && distToMars > 1000)
                {
                    force += toMars.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Mars] / (distToMars * distToMars));
                }
                
                // Jupiter
                var jupiterPosVec = Astronomy.HelioVector(Body.Jupiter, currentTime);
                var jupiterPos = new Vector3D(jupiterPosVec.x * ProgramJustitia.AU, jupiterPosVec.y * ProgramJustitia.AU, jupiterPosVec.z * ProgramJustitia.AU);
                var toJupiter = jupiterPos - craftPos;
                double distToJupiter = toJupiter.Magnitude();
                if (distToJupiter < 7.0 * ProgramJustitia.AU && distToJupiter > 1000)
                {
                    force += toJupiter.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Jupiter] / (distToJupiter * distToJupiter));
                }
                
                // Saturn (for outer region assists)
                var saturnPosVec = Astronomy.HelioVector(Body.Saturn, currentTime);
                var saturnPos = new Vector3D(saturnPosVec.x * ProgramJustitia.AU, saturnPosVec.y * ProgramJustitia.AU, saturnPosVec.z * ProgramJustitia.AU);
                var toSaturn = saturnPos - craftPos;
                double distToSaturn = toSaturn.Magnitude();
                if (distToSaturn < 12.0 * ProgramJustitia.AU && distToSaturn > 1000)
                {
                    force += toSaturn.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Saturn] / (distToSaturn * distToSaturn));
                }
                
                craftVel += force * TimeStep;
                craftPos += craftVel * TimeStep;
                
                // Store positions every 4 steps (1 day)
                if (step % 4 == 0)
                {
                    positions.Add((craftPos.X / ProgramJustitia.AU, craftPos.Y / ProgramJustitia.AU, craftPos.Z / ProgramJustitia.AU, currentDateTime));
                    
                    // Store all asteroid positions
                    foreach (var asteroidName in asteroidEphemerides.Keys)
                    {
                        var asteroidPos = asteroidEphemerides[asteroidName].GetPositionAtTime(currentDateTime);
                        allAsteroidPositions[asteroidName].Add((asteroidPos.X / ProgramJustitia.AU, 
                                                                 asteroidPos.Y / ProgramJustitia.AU, 
                                                                 asteroidPos.Z / ProgramJustitia.AU));
                    }
                    
                    venusPosVec = Astronomy.HelioVector(Body.Venus, currentTime);
                    venusPositions.Add((venusPosVec.x, venusPosVec.y, venusPosVec.z));
                    
                    earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
                    earthPositions.Add((earthPosVec.x, earthPosVec.y, earthPosVec.z));
                    
                    marsPosVec = Astronomy.HelioVector(Body.Mars, currentTime);
                    marsPositions.Add((marsPosVec.x, marsPosVec.y, marsPosVec.z));
                    
                    jupiterPosVec = Astronomy.HelioVector(Body.Jupiter, currentTime);
                    jupiterPositions.Add((jupiterPosVec.x, jupiterPosVec.y, jupiterPosVec.z));
                    
                    saturnPosVec = Astronomy.HelioVector(Body.Saturn, currentTime);
                    saturnPositions.Add((saturnPosVec.x, saturnPosVec.y, saturnPosVec.z));
                }
                
                currentDateTime = currentDateTime.AddSeconds(TimeStep);
                currentTime = new AstroTime(currentDateTime);
                
                // Progress
                if (step % (maxSteps / 20) == 0)
                {
                    Console.WriteLine($"Progress: {100 * step / maxSteps:F0}%");
                }
            }
            
            // Export to CSV
            string filename = "trajectory_justitia_output.csv";
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("type,x_au,y_au,z_au,time");
                
                for (int i = 0; i < positions.Count; i++)
                {
                    var pos = positions[i];
                    writer.WriteLine($"spacecraft,{pos.x},{pos.y},{pos.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    
                    // Write all asteroid positions
                    foreach (var asteroidName in allAsteroidPositions.Keys)
                    {
                        var ast = allAsteroidPositions[asteroidName][i];
                        writer.WriteLine($"{asteroidName.ToLower()},{ast.x},{ast.y},{ast.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    }
                    
                    // Write planet positions
                    var v = venusPositions[i];
                    writer.WriteLine($"venus,{v.x},{v.y},{v.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    
                    var e = earthPositions[i];
                    writer.WriteLine($"earth,{e.x},{e.y},{e.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    
                    var m = marsPositions[i];
                    writer.WriteLine($"mars,{m.x},{m.y},{m.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    
                    var jup = jupiterPositions[i];
                    writer.WriteLine($"jupiter,{jup.x},{jup.y},{jup.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                    
                    var sat = saturnPositions[i];
                    writer.WriteLine($"saturn,{sat.x},{sat.y},{sat.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                }
            }
            
            Console.WriteLine($"✓ Trajectory exported to {filename}");
            Console.WriteLine($"  Total points: {positions.Count}");
            Console.WriteLine("\nRun 'python plot_justitia_trajectory.py' to visualize!\n");
        }
        
        public void SimulateAndExportMultipleTrajectories(JustitiaChromosome bestChromosome)
        {
            Console.WriteLine("📍 Exporting BEST trajectory + last 10 generations...");
            
            // Collect all chromosomes to simulate (best + top from last 10 gens)
            var chromosomesToSimulate = new List<(JustitiaChromosome chromosome, int genNumber, bool isBest)>();
            
            // Add best solution
            chromosomesToSimulate.Add((bestChromosome, generations - 1, true));
            
            // Add top chromosomes from last 10 generations
            for (int i = 0; i < generationHistory.Count; i++)
            {
                int genNumber = generations - 10 + i;
                foreach (var chrom in generationHistory[i].Take(2))  // Top 2 from each gen
                {
                    if (chrom != bestChromosome)  // Don't duplicate best
                    {
                        chromosomesToSimulate.Add((chrom, genNumber, false));
                    }
                }
            }
            
            Console.WriteLine($"Simulating {chromosomesToSimulate.Count} trajectories...");
            
            // Simulate all and export to single CSV
            string filename = "trajectory_justitia_multi.csv";
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("trajectory_id,is_best,generation,x_au,y_au,z_au,time");
                
                int trajId = 0;
                foreach (var (chrom, gen, isBest) in chromosomesToSimulate)
                {
                    Console.Write($"  Simulating trajectory {trajId + 1}/{chromosomesToSimulate.Count}...");
                    
                    // Quick simulation with fewer timesteps
                    int maxSteps = (int)(MaxDays * 24 / (TimeStep / 3600.0));
                    var currentTime = new AstroTime(ProgramJustitia.START_DATE);
                    var currentDateTime = ProgramJustitia.START_DATE;
                    
                    var earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
                    var craftPos = new Vector3D(earthPosVec.x * ProgramJustitia.AU, 
                                               earthPosVec.y * ProgramJustitia.AU, 
                                               earthPosVec.z * ProgramJustitia.AU);
                    
                    double earthOrbitalVel = 29780.0;
                    var (launchVx, launchVy) = chrom.GetLaunchVelocity();
                    var craftVel = new Vector3D(launchVx, earthOrbitalVel + launchVy, 0);
                    
                    // Maneuver schedule
                    int[] maneuverSteps = new int[6];
                    maneuverSteps[0] = (int)(chrom.Maneuver1Time * 24 / (TimeStep / 3600.0));
                    maneuverSteps[1] = (int)(chrom.Maneuver2Time * 24 / (TimeStep / 3600.0));
                    maneuverSteps[2] = (int)(chrom.Maneuver3Time * 24 / (TimeStep / 3600.0));
                    maneuverSteps[3] = (int)(chrom.Maneuver4Time * 24 / (TimeStep / 3600.0));
                    maneuverSteps[4] = (int)(chrom.Maneuver5Time * 24 / (TimeStep / 3600.0));
                    maneuverSteps[5] = (int)(chrom.Maneuver6Time * 24 / (TimeStep / 3600.0));
                    
                    double[,] maneuverDvs = new double[6, 2];
                    maneuverDvs[0, 0] = chrom.Maneuver1DeltaVX; maneuverDvs[0, 1] = chrom.Maneuver1DeltaVY;
                    maneuverDvs[1, 0] = chrom.Maneuver2DeltaVX; maneuverDvs[1, 1] = chrom.Maneuver2DeltaVY;
                    maneuverDvs[2, 0] = chrom.Maneuver3DeltaVX; maneuverDvs[2, 1] = chrom.Maneuver3DeltaVY;
                    maneuverDvs[3, 0] = chrom.Maneuver4DeltaVX; maneuverDvs[3, 1] = chrom.Maneuver4DeltaVY;
                    maneuverDvs[4, 0] = chrom.Maneuver5DeltaVX; maneuverDvs[4, 1] = chrom.Maneuver5DeltaVY;
                    maneuverDvs[5, 0] = chrom.Maneuver6DeltaVX; maneuverDvs[5, 1] = chrom.Maneuver6DeltaVY;
                    
                    // Simulate (store every 10 steps for speed)
                    for (int step = 0; step < maxSteps; step += 10)
                    {
                        // Apply maneuvers
                        for (int m = 0; m < 6; m++)
                        {
                            if (step == maneuverSteps[m])
                            {
                                craftVel += new Vector3D(maneuverDvs[m, 0], maneuverDvs[m, 1], 0);
                            }
                        }
                        
                        // Simplified gravity (Sun only for speed)
                        var toSun = new Vector3D(0, 0, 0) - craftPos;
                        double distToSun = toSun.Magnitude();
                        if (distToSun > 1000)
                        {
                            var force = toSun.Normalized() * (ProgramJustitia.G * ProgramJustitia.PlanetMasses[Body.Sun] / (distToSun * distToSun));
                            craftVel += force * TimeStep * 10;
                        }
                        
                        craftPos += craftVel * TimeStep * 10;
                        currentDateTime = currentDateTime.AddSeconds(TimeStep * 10);
                        currentTime = new AstroTime(currentDateTime);
                        
                        // Write every 40 steps (~10 days)
                        if (step % 40 == 0)
                        {
                            writer.WriteLine($"{trajId},{(isBest ? 1 : 0)},{gen}," +
                                           $"{craftPos.X / ProgramJustitia.AU}," +
                                           $"{craftPos.Y / ProgramJustitia.AU}," +
                                           $"{craftPos.Z / ProgramJustitia.AU}," +
                                           $"{currentDateTime:yyyy-MM-dd HH:mm:ss}");
                        }
                    }
                    
                    trajId++;
                    Console.WriteLine(" ✓");
                }
            }
            
            Console.WriteLine($"\n✓ Exported {chromosomesToSimulate.Count} trajectories to {filename}");
            Console.WriteLine($"  Best solution is trajectory_id = 0");
            Console.WriteLine($"  Last 10 generations included for comparison");
            Console.WriteLine("\nRun 'python plot_multi_trajectories.py' to visualize all!\n");
        }
        
        // Utility methods
        private double RandomDouble(double min, double max) => min + random.NextDouble() * (max - min);
        
        private double RandomGaussian()
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        }
        
        private double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}

