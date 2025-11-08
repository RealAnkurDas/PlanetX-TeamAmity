using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CosineKitty;

namespace TrajectoryOptimization
{
    /// <summary>
    /// Standalone Genetic Algorithm for Earth-to-Jupiter trajectory optimization
    /// Uses real ephemeris data - runs ~50-100x faster than Python!
    /// </summary>
    class Program
    {
        // Constants
        public const double G = 6.67430e-11;           // Gravitational constant
        public const double AU = 1.496e11;             // AU in meters
        
        public static readonly DateTime START_DATE = new DateTime(2028, 3, 3, 0, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime END_DATE = new DateTime(2035, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        
        // Planet masses (kg)
        public static readonly Dictionary<Body, double> PlanetMasses = new Dictionary<Body, double>
        {
            { Body.Sun, 1.989e30 },
            { Body.Earth, 5.972e24 },
            { Body.Mars, 6.39e23 },
            { Body.Jupiter, 1.898e27 },
        };
        
        static void Main(string[] args)
        {
            Console.WriteLine("=============================================================");
            Console.WriteLine("Trajectory Genetic Algorithm - Standalone C#");
            Console.WriteLine("Earth to Jupiter Mission Optimization");
            Console.WriteLine("=============================================================");
            Console.WriteLine($"Mission Timeline: {START_DATE:yyyy-MM-dd} → {END_DATE:yyyy-MM-dd}");
            Console.WriteLine("Using real ephemeris data from CosineKitty Astronomy library");
            Console.WriteLine("=============================================================\n");
            
            // Parse command line arguments
            int populationSize = 40;
            int generations = 75;
            
            if (args.Length >= 1 && int.TryParse(args[0], out int pop))
                populationSize = pop;
            if (args.Length >= 2 && int.TryParse(args[1], out int gen))
                generations = gen;
            
            Console.WriteLine($"Population Size: {populationSize}");
            Console.WriteLine($"Generations: {generations}");
            Console.WriteLine();
            
            // Run the GA
            var ga = new GeneticAlgorithm(populationSize, generations);
            
            Stopwatch sw = Stopwatch.StartNew();
            var bestChromosome = ga.Run();
            sw.Stop();
            
            Console.WriteLine($"\n⏱️  Total execution time: {sw.Elapsed.TotalSeconds:F2} seconds");
            Console.WriteLine("=============================================================\n");
            
            // Simulate and visualize the best trajectory
            Console.WriteLine("Simulating best trajectory for visualization...\n");
            ga.SimulateAndExportTrajectory(bestChromosome);
        }
    }
    
    // =============================================================================
    // CHROMOSOME STRUCTURE
    // =============================================================================
    
    public class Chromosome
    {
        public double[] Genes { get; set; }  // 8 genes total
        public double Fitness { get; set; }
        
        public Chromosome()
        {
            Genes = new double[8];
            Fitness = double.MaxValue;
        }
        
        public Chromosome Clone()
        {
            var clone = new Chromosome();
            Array.Copy(Genes, clone.Genes, Genes.Length);
            clone.Fitness = Fitness;
            return clone;
        }
        
        // Gene accessors for clarity
        public double LaunchVelocityX { get => Genes[0]; set => Genes[0] = value; }
        public double LaunchVelocityY { get => Genes[1]; set => Genes[1] = value; }
        public double Maneuver1Time { get => Genes[2]; set => Genes[2] = value; }
        public double Maneuver1DeltaVX { get => Genes[3]; set => Genes[3] = value; }
        public double Maneuver1DeltaVY { get => Genes[4]; set => Genes[4] = value; }
        public double Maneuver2Time { get => Genes[5]; set => Genes[5] = value; }
        public double Maneuver2DeltaVX { get => Genes[6]; set => Genes[6] = value; }
        public double Maneuver2DeltaVY { get => Genes[7]; set => Genes[7] = value; }
    }
    
    // =============================================================================
    // SIMULATION RESULT
    // =============================================================================
    
    public class TrajectoryResult
    {
        public double MinDistanceToJupiter { get; set; }
        public double TotalDeltaV { get; set; }
        public double TimeDays { get; set; }
        public bool Crashed { get; set; }
        public bool Escaped { get; set; }
        public Vector2D FinalPosition { get; set; }
        
        public TrajectoryResult()
        {
            MinDistanceToJupiter = double.PositiveInfinity;
            TotalDeltaV = 0;
            TimeDays = 0;
            Crashed = false;
            Escaped = false;
            FinalPosition = new Vector2D(0, 0);
        }
    }
    
    // =============================================================================
    // VECTOR2D HELPER
    // =============================================================================
    
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
        
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(Vector2D a, double scalar) => new Vector2D(a.X * scalar, a.Y * scalar);
        
        public double Magnitude() => Math.Sqrt(X * X + Y * Y);
        
        public Vector2D Normalized()
        {
            double mag = Magnitude();
            if (mag < 1e-10) return new Vector2D(0, 0);
            return new Vector2D(X / mag, Y / mag);
        }
    }
    
    // =============================================================================
    // GENETIC ALGORITHM
    // =============================================================================
    
    public class GeneticAlgorithm
    {
        private const double TimeStep = 7200.0;  // 2 hours in seconds
        private const int MaxDays = 1200;
        private const double MutationRate = 0.1;
        private const double MutationStrength = 0.2;
        private const int EliteCount = 2;
        
        private readonly int populationSize;
        private readonly int generations;
        private readonly Random random;
        private List<Chromosome> population;
        private Chromosome? bestChromosome;
        private double bestFitness;
        
        public GeneticAlgorithm(int populationSize, int generations)
        {
            this.populationSize = populationSize;
            this.generations = generations;
            this.random = new Random();
            this.population = new List<Chromosome>(populationSize);
            this.bestFitness = double.MaxValue;
        }
        
        public Chromosome Run()
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
                
                // Log progress
                if (gen % 5 == 0 || gen == generations - 1)
                {
                    var result = SimulateTrajectory(best);
                    Console.WriteLine($"Gen {gen,3} | Fitness: {best.Fitness,6:F2} | " +
                                    $"Dist: {result.MinDistanceToJupiter / Program.AU,5:F3} AU | " +
                                    $"ΔV: {result.TotalDeltaV,5:F0} m/s | " +
                                    $"Time: {result.TimeDays,4:F0}d");
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
            // Create 5 seeded "smart" solutions
            for (int i = 0; i < 5; i++)
            {
                var seed = new Chromosome
                {
                    LaunchVelocityX = RandomDouble(0, 5000),
                    LaunchVelocityY = RandomDouble(8000, 12000),
                    Maneuver1Time = RandomDouble(100, 400),
                    Maneuver1DeltaVX = RandomDouble(-1000, 1000),
                    Maneuver1DeltaVY = RandomDouble(-1000, 1000),
                    Maneuver2Time = RandomDouble(400, 800),
                    Maneuver2DeltaVX = RandomDouble(-1000, 1000),
                    Maneuver2DeltaVY = RandomDouble(-1000, 1000)
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
        
        private Chromosome CreateRandomChromosome()
        {
            return new Chromosome
            {
                LaunchVelocityX = RandomDouble(-10000, 10000),
                LaunchVelocityY = RandomDouble(5000, 15000),
                Maneuver1Time = RandomDouble(50, 600),
                Maneuver1DeltaVX = RandomDouble(-3000, 3000),
                Maneuver1DeltaVY = RandomDouble(-3000, 3000),
                Maneuver2Time = RandomDouble(200, 1000),
                Maneuver2DeltaVX = RandomDouble(-3000, 3000),
                Maneuver2DeltaVY = RandomDouble(-3000, 3000)
            };
        }
        
        private void EvaluatePopulation()
        {
            foreach (var chromosome in population)
            {
                chromosome.Fitness = CalculateFitness(chromosome);
            }
        }
        
        private double CalculateFitness(Chromosome chromosome)
        {
            var result = SimulateTrajectory(chromosome);
            
            // Severe penalties
            if (result.Crashed) return 10000.0;
            if (result.Escaped) return 10000.0;
            if (double.IsInfinity(result.MinDistanceToJupiter)) return 10000.0;
            
            // Calculate fitness components
            double distancePenalty = (result.MinDistanceToJupiter / Program.AU) * 2.0;
            double fuelPenalty = (result.TotalDeltaV / 10000.0) * 0.3;
            double timePenalty = (result.TimeDays / 365.0) * 0.1;
            
            // Bonus for getting close
            double arrivalBonus = 0;
            if (result.MinDistanceToJupiter < 0.5 * Program.AU)
            {
                arrivalBonus = 50.0 / (result.MinDistanceToJupiter / Program.AU + 0.1);
            }
            
            return distancePenalty + fuelPenalty + timePenalty - arrivalBonus;
        }
        
        private TrajectoryResult SimulateTrajectory(Chromosome chromosome)
        {
            var result = new TrajectoryResult();
            
            int maxSteps = (int)(MaxDays * 24 / (TimeStep / 3600.0));
            
            // Get Earth's starting position
            var currentTime = new AstroTime(Program.START_DATE);
            var currentDateTime = Program.START_DATE;
            
            var earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
            var earthPos = new Vector2D(earthPosVec.x * Program.AU, earthPosVec.y * Program.AU);
            
            // Initialize spacecraft
            var craftPos = earthPos;
            double earthOrbitalVel = 29780.0;
            var craftVel = new Vector2D(chromosome.LaunchVelocityX, 
                                       earthOrbitalVel + chromosome.LaunchVelocityY);
            
            result.TotalDeltaV = Math.Sqrt(chromosome.LaunchVelocityX * chromosome.LaunchVelocityX + 
                                          chromosome.LaunchVelocityY * chromosome.LaunchVelocityY);
            
            // Maneuver schedule
            int maneuver1Step = (int)(chromosome.Maneuver1Time * 24 / (TimeStep / 3600.0));
            int maneuver2Step = (int)(chromosome.Maneuver2Time * 24 / (TimeStep / 3600.0));
            
            bool leftEarth = false;
            
            // Simulation loop
            for (int step = 0; step < maxSteps; step++)
            {
                // Apply maneuvers
                if (step == maneuver1Step)
                {
                    var dv1 = new Vector2D(chromosome.Maneuver1DeltaVX, chromosome.Maneuver1DeltaVY);
                    craftVel = craftVel + dv1;
                    result.TotalDeltaV += dv1.Magnitude();
                }
                else if (step == maneuver2Step)
                {
                    var dv2 = new Vector2D(chromosome.Maneuver2DeltaVX, chromosome.Maneuver2DeltaVY);
                    craftVel = craftVel + dv2;
                    result.TotalDeltaV += dv2.Magnitude();
                }
                
                // Check if left Earth
                double distFromEarth = (craftPos - earthPos).Magnitude();
                if (distFromEarth > 5e7) leftEarth = true;
                
                // Advance time
                currentDateTime = currentDateTime.AddSeconds(TimeStep);
                if (currentDateTime > Program.END_DATE) break;
                
                currentTime = new AstroTime(currentDateTime);
                
                // Calculate gravitational acceleration
                var acc = new Vector2D(0, 0);
                var jupiterPos = new Vector2D(0, 0);
                
                foreach (var kvp in Program.PlanetMasses)
                {
                    Body planet = kvp.Key;
                    double mass = kvp.Value;
                    
                    Vector2D planetPos;
                    if (planet == Body.Sun)
                    {
                        planetPos = new Vector2D(0, 0);
                    }
                    else
                    {
                        var pv = Astronomy.HelioVector(planet, currentTime);
                        planetPos = new Vector2D(pv.x * Program.AU, pv.y * Program.AU);
                    }
                    
                    if (planet == Body.Jupiter)
                    {
                        jupiterPos = planetPos;
                    }
                    
                    var rVec = planetPos - craftPos;
                    double r = rVec.Magnitude();
                    
                    // Crash detection
                    double crashRadius = planet == Body.Jupiter ? 7.5e7 :
                                        planet == Body.Sun ? 7e8 :
                                        7e6;
                    
                    if (r < crashRadius)
                    {
                        if (planet == Body.Earth && !leftEarth)
                        {
                            // Still at Earth launch
                        }
                        else
                        {
                            result.Crashed = true;
                            break;
                        }
                    }
                    
                    if (r < 1e3) continue;
                    
                    double a = Program.G * mass / (r * r);
                    acc = acc + (rVec.Normalized() * a);
                }
                
                if (result.Crashed) break;
                
                // Update velocity and position
                craftVel = craftVel + (acc * TimeStep);
                craftPos = craftPos + (craftVel * TimeStep);
                
                // Check escape
                double distFromSun = craftPos.Magnitude();
                if (distFromSun > 10 * Program.AU)
                {
                    result.Escaped = true;
                    break;
                }
                
                // Track distance to Jupiter
                if (step % 10 == 0)
                {
                    double distToJupiter = (craftPos - jupiterPos).Magnitude();
                    result.MinDistanceToJupiter = Math.Min(result.MinDistanceToJupiter, distToJupiter);
                    
                    // Early termination
                    if (step > 200 && distFromSun > 7 * Program.AU)
                    {
                        if (distToJupiter > 3 * Program.AU && distToJupiter > result.MinDistanceToJupiter * 2.0)
                        {
                            break;
                        }
                    }
                }
            }
            
            result.TimeDays = (currentDateTime - Program.START_DATE).TotalDays;
            result.FinalPosition = craftPos;
            
            return result;
        }
        
        private void CreateNextGeneration()
        {
            var newPopulation = new List<Chromosome>(populationSize);
            
            // Elitism - keep best
            var elite = population.OrderBy(c => c.Fitness).Take(EliteCount).ToList();
            foreach (var e in elite)
            {
                newPopulation.Add(e.Clone());
            }
            
            // Generate rest through selection, crossover, mutation
            while (newPopulation.Count < populationSize)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();
                
                var (child1, child2) = Crossover(parent1, parent2);
                
                child1 = Mutate(child1);
                child2 = Mutate(child2);
                
                newPopulation.Add(child1);
                if (newPopulation.Count < populationSize)
                {
                    newPopulation.Add(child2);
                }
            }
            
            population = newPopulation;
        }
        
        private Chromosome TournamentSelection(int tournamentSize = 3)
        {
            Chromosome? best = null;
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
        
        private (Chromosome, Chromosome) Crossover(Chromosome parent1, Chromosome parent2)
        {
            var child1 = new Chromosome();
            var child2 = new Chromosome();
            
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
        
        private Chromosome Mutate(Chromosome chromosome)
        {
            var mutated = chromosome.Clone();
            
            for (int i = 0; i < mutated.Genes.Length; i++)
            {
                if (random.NextDouble() < MutationRate)
                {
                    double noise = RandomGaussian() * (Math.Abs(mutated.Genes[i]) * MutationStrength + 100);
                    mutated.Genes[i] += noise;
                    
                    // Enforce bounds
                    if (i == 0) mutated.Genes[i] = Clamp(mutated.Genes[i], -10000, 10000);
                    else if (i == 1) mutated.Genes[i] = Clamp(mutated.Genes[i], 5000, 15000);
                    else if (i == 2) mutated.Genes[i] = Clamp(mutated.Genes[i], 50, 600);
                    else if (i == 5) mutated.Genes[i] = Clamp(mutated.Genes[i], 200, 1000);
                    else mutated.Genes[i] = Clamp(mutated.Genes[i], -3000, 3000);
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
            
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("🎯 OPTIMIZATION COMPLETE!");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("\n📊 Best Solution Found:");
            Console.WriteLine($"  • Minimum Distance to Jupiter: {result.MinDistanceToJupiter / Program.AU:F4} AU ({result.MinDistanceToJupiter / 1e6:F0} km)");
            Console.WriteLine($"  • Total Delta-V: {result.TotalDeltaV:F0} m/s");
            Console.WriteLine($"  • Mission Time: {result.TimeDays:F0} days ({result.TimeDays / 365:F2} years)");
            Console.WriteLine($"  • Fitness Score: {bestFitness:F2}");
            Console.WriteLine($"\n🚀 Mission Parameters:");
            Console.WriteLine($"  • Launch Velocity: [{bestChromosome.LaunchVelocityX:F0}, {bestChromosome.LaunchVelocityY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 1 (Day {bestChromosome.Maneuver1Time:F0}): ΔV = [{bestChromosome.Maneuver1DeltaVX:F0}, {bestChromosome.Maneuver1DeltaVY:F0}] m/s");
            Console.WriteLine($"  • Maneuver 2 (Day {bestChromosome.Maneuver2Time:F0}): ΔV = [{bestChromosome.Maneuver2DeltaVX:F0}, {bestChromosome.Maneuver2DeltaVY:F0}] m/s");
            Console.WriteLine(new string('=', 60));
        }
        
        public void SimulateAndExportTrajectory(Chromosome chromosome)
        {
            Console.WriteLine("📍 Simulating complete trajectory with full detail...");
            
            var positions = new List<(double x, double y, double z, DateTime time)>();
            var planetPositions = new Dictionary<string, List<(double x, double y, double z)>>();
            
            planetPositions["earth"] = new List<(double x, double y, double z)>();
            planetPositions["mars"] = new List<(double x, double y, double z)>();
            planetPositions["jupiter"] = new List<(double x, double y, double z)>();
            
            int maxSteps = (int)(MaxDays * 24 / (TimeStep / 3600.0));
            
            // Get Earth's starting position
            var currentTime = new AstroTime(Program.START_DATE);
            var currentDateTime = Program.START_DATE;
            
            var earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
            var earthPos = new Vector2D(earthPosVec.x * Program.AU, earthPosVec.y * Program.AU);
            
            // Initialize spacecraft
            var craftPos = earthPos;
            double earthOrbitalVel = 29780.0;
            var craftVel = new Vector2D(chromosome.LaunchVelocityX, 
                                       earthOrbitalVel + chromosome.LaunchVelocityY);
            
            // Maneuver schedule
            int maneuver1Step = (int)(chromosome.Maneuver1Time * 24 / (TimeStep / 3600.0));
            int maneuver2Step = (int)(chromosome.Maneuver2Time * 24 / (TimeStep / 3600.0));
            
            Console.WriteLine($"Simulating {maxSteps} timesteps ({maxSteps * TimeStep / 86400:F0} days)...");
            
            for (int step = 0; step < maxSteps; step++)
            {
                // Apply maneuvers
                if (step == maneuver1Step)
                {
                    craftVel += new Vector2D(chromosome.Maneuver1DeltaVX, chromosome.Maneuver1DeltaVY);
                }
                if (step == maneuver2Step)
                {
                    craftVel += new Vector2D(chromosome.Maneuver2DeltaVX, chromosome.Maneuver2DeltaVY);
                }
                
                // Calculate gravitational forces
                var force = new Vector2D(0, 0);
                
                // Sun
                var sunPos = new Vector2D(0, 0);
                var toSun = sunPos - craftPos;
                double distToSun = toSun.Magnitude();
                if (distToSun > 1000)
                {
                    force += toSun.Normalized() * (Program.G * Program.PlanetMasses[Body.Sun] / (distToSun * distToSun));
                }
                
                // Earth
                earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
                earthPos = new Vector2D(earthPosVec.x * Program.AU, earthPosVec.y * Program.AU);
                var toEarth = earthPos - craftPos;
                double distToEarth = toEarth.Magnitude();
                if (distToEarth > 1000)
                {
                    force += toEarth.Normalized() * (Program.G * Program.PlanetMasses[Body.Earth] / (distToEarth * distToEarth));
                }
                
                // Mars
                var marsPosVec = Astronomy.HelioVector(Body.Mars, currentTime);
                var marsPos = new Vector2D(marsPosVec.x * Program.AU, marsPosVec.y * Program.AU);
                var toMars = marsPos - craftPos;
                double distToMars = toMars.Magnitude();
                if (distToMars > 1000)
                {
                    force += toMars.Normalized() * (Program.G * Program.PlanetMasses[Body.Mars] / (distToMars * distToMars));
                }
                
                // Jupiter
                var jupiterPosVec = Astronomy.HelioVector(Body.Jupiter, currentTime);
                var jupiterPos = new Vector2D(jupiterPosVec.x * Program.AU, jupiterPosVec.y * Program.AU);
                var toJupiter = jupiterPos - craftPos;
                double distToJupiter = toJupiter.Magnitude();
                if (distToJupiter > 1000)
                {
                    force += toJupiter.Normalized() * (Program.G * Program.PlanetMasses[Body.Jupiter] / (distToJupiter * distToJupiter));
                }
                
                // Update velocity and position
                craftVel += force * TimeStep;
                craftPos += craftVel * TimeStep;
                
                // Store positions every 24 hours (not every timestep to save space)
                if (step % 12 == 0)
                {
                    positions.Add((craftPos.X / Program.AU, craftPos.Y / Program.AU, 0, currentDateTime));
                    planetPositions["earth"].Add((earthPosVec.x, earthPosVec.y, earthPosVec.z));
                    planetPositions["mars"].Add((marsPosVec.x, marsPosVec.y, marsPosVec.z));
                    planetPositions["jupiter"].Add((jupiterPosVec.x, jupiterPosVec.y, jupiterPosVec.z));
                }
                
                // Update time
                currentDateTime = currentDateTime.AddSeconds(TimeStep);
                currentTime = new AstroTime(currentDateTime);
            }
            
            // Export to CSV
            string filename = "trajectory_output.csv";
            using (var writer = new System.IO.StreamWriter(filename))
            {
                writer.WriteLine("type,x_au,y_au,z_au,time");
                
                foreach (var pos in positions)
                {
                    writer.WriteLine($"spacecraft,{pos.x},{pos.y},{pos.z},{pos.time:yyyy-MM-dd HH:mm:ss}");
                }
                
                for (int i = 0; i < planetPositions["earth"].Count; i++)
                {
                    var e = planetPositions["earth"][i];
                    var m = planetPositions["mars"][i];
                    var j = planetPositions["jupiter"][i];
                    var t = positions[i].time;
                    
                    writer.WriteLine($"earth,{e.x},{e.y},{e.z},{t:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"mars,{m.x},{m.y},{m.z},{t:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"jupiter,{j.x},{j.y},{j.z},{t:yyyy-MM-dd HH:mm:ss}");
                }
            }
            
            Console.WriteLine($"✓ Trajectory exported to {filename}");
            Console.WriteLine($"  Total points: {positions.Count}");
            Console.WriteLine("\nTo visualize, you can plot this CSV file with any plotting tool.");
            Console.WriteLine("Columns: type (spacecraft/earth/mars/jupiter), x_au, y_au, z_au, time\n");
        }
        
        // Utility methods
        private double RandomDouble(double min, double max) => min + random.NextDouble() * (max - min);
        
        private double RandomGaussian()
        {
            // Box-Muller transform
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

