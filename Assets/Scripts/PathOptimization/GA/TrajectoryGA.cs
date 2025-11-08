using System;
using System.Collections.Generic;
using System.Linq;
using CosineKitty;
using UnityEngine;

/// <summary>
/// Genetic Algorithm for optimizing Earth-to-Jupiter spacecraft trajectory
/// Uses real ephemeris data from Astronomy library
/// Much faster than Python implementation
/// </summary>
public class TrajectoryGA : MonoBehaviour
{
    // Constants
    private const double G = 6.67430e-11;           // Gravitational constant
    private const double AU = 1.496e11;             // AU in meters
    
    [Header("Mission Timeline")]
    [SerializeField] private string startDateString = "2028-03-03";
    [SerializeField] private string endDateString = "2035-12-31";
    
    [Header("GA Parameters")]
    [SerializeField] private int populationSize = 40;
    [SerializeField] private int generations = 75;
    [SerializeField] private float mutationRate = 0.1f;
    [SerializeField] private float mutationStrength = 0.2f;
    [SerializeField] private int eliteCount = 2;
    
    [Header("Simulation Parameters")]
    [SerializeField] private double timeStep = 7200.0;  // 2 hours in seconds
    [SerializeField] private int maxDays = 1200;
    
    [Header("Status")]
    [SerializeField] private bool isRunning = false;
    [SerializeField] private int currentGeneration = 0;
    [SerializeField] private float bestFitness = float.MaxValue;
    [SerializeField] private string statusText = "Ready";
    
    private DateTime startDate;
    private DateTime endDate;
    private List<Chromosome> population;
    private Chromosome bestChromosome;
    private List<float> fitnessHistory;
    private System.Random random;
    
    // Planet masses (kg)
    private static readonly Dictionary<Body, double> PlanetMasses = new Dictionary<Body, double>
    {
        { Body.Sun, 1.989e30 },
        { Body.Earth, 5.972e24 },
        { Body.Mars, 6.39e23 },
        { Body.Jupiter, 1.898e27 },
    };
    
    // =============================================================================
    // CHROMOSOME STRUCTURE
    // =============================================================================
    
    [System.Serializable]
    public class Chromosome
    {
        public double[] genes;  // 8 genes total
        public float fitness;
        
        public Chromosome()
        {
            genes = new double[8];
            fitness = float.MaxValue;
        }
        
        public Chromosome Clone()
        {
            Chromosome clone = new Chromosome();
            Array.Copy(genes, clone.genes, genes.Length);
            clone.fitness = fitness;
            return clone;
        }
        
        // Gene accessors for clarity
        public double LaunchVelocityX { get => genes[0]; set => genes[0] = value; }
        public double LaunchVelocityY { get => genes[1]; set => genes[1] = value; }
        public double Maneuver1Time { get => genes[2]; set => genes[2] = value; }
        public double Maneuver1DeltaVX { get => genes[3]; set => genes[3] = value; }
        public double Maneuver1DeltaVY { get => genes[4]; set => genes[4] = value; }
        public double Maneuver2Time { get => genes[5]; set => genes[5] = value; }
        public double Maneuver2DeltaVX { get => genes[6]; set => genes[6] = value; }
        public double Maneuver2DeltaVY { get => genes[7]; set => genes[7] = value; }
    }
    
    // =============================================================================
    // SIMULATION RESULT
    // =============================================================================
    
    public class TrajectoryResult
    {
        public double minDistanceToJupiter;
        public double totalDeltaV;
        public double timeDays;
        public bool crashed;
        public bool escaped;
        public Vector2d finalPosition;
        
        public TrajectoryResult()
        {
            minDistanceToJupiter = double.PositiveInfinity;
            totalDeltaV = 0;
            timeDays = 0;
            crashed = false;
            escaped = false;
        }
    }
    
    // =============================================================================
    // HELPER STRUCTURES
    // =============================================================================
    
    public struct Vector2d
    {
        public double x;
        public double y;
        
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x + b.x, a.y + b.y);
        }
        
        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x - b.x, a.y - b.y);
        }
        
        public static Vector2d operator *(Vector2d a, double scalar)
        {
            return new Vector2d(a.x * scalar, a.y * scalar);
        }
        
        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }
        
        public Vector2d Normalized()
        {
            double mag = Magnitude();
            if (mag < 1e-10) return new Vector2d(0, 0);
            return new Vector2d(x / mag, y / mag);
        }
    }
    
    // =============================================================================
    // UNITY LIFECYCLE
    // =============================================================================
    
    void Start()
    {
        random = new System.Random();
        fitnessHistory = new List<float>();
        
        // Parse dates
        startDate = DateTime.Parse(startDateString).ToUniversalTime();
        endDate = DateTime.Parse(endDateString).ToUniversalTime();
        
        Debug.Log("=============================================================");
        Debug.Log("Trajectory Genetic Algorithm - Native C#");
        Debug.Log("=============================================================");
        Debug.Log($"Mission Timeline: {startDate:yyyy-MM-dd} → {endDate:yyyy-MM-dd}");
        Debug.Log($"Population: {populationSize}, Generations: {generations}");
        Debug.Log("=============================================================");
    }
    
    // =============================================================================
    // PUBLIC CONTROLS
    // =============================================================================
    
    public void StartOptimization()
    {
        if (isRunning)
        {
            Debug.LogWarning("GA already running!");
            return;
        }
        
        StartCoroutine(RunGeneticAlgorithm());
    }
    
    public void StopOptimization()
    {
        isRunning = false;
        statusText = "Stopped";
    }
    
    // =============================================================================
    // GENETIC ALGORITHM MAIN LOOP
    // =============================================================================
    
    private System.Collections.IEnumerator RunGeneticAlgorithm()
    {
        isRunning = true;
        statusText = "Initializing...";
        
        // Initialize population
        InitializePopulation();
        
        bestFitness = float.MaxValue;
        bestChromosome = null;
        
        statusText = "Evolving...";
        
        for (int gen = 0; gen < generations; gen++)
        {
            currentGeneration = gen;
            
            // Evaluate fitness for all chromosomes
            EvaluatePopulation();
            
            // Track best
            var best = population.OrderBy(c => c.fitness).First();
            if (best.fitness < bestFitness)
            {
                bestFitness = best.fitness;
                bestChromosome = best.Clone();
            }
            
            fitnessHistory.Add(bestFitness);
            
            // Log progress every 5 generations
            if (gen % 5 == 0 || gen == generations - 1)
            {
                var result = SimulateTrajectory(best);
                Debug.Log($"Gen {gen:D3} | Fitness: {best.fitness:F2} | " +
                         $"Dist: {result.minDistanceToJupiter / AU:F3} AU | " +
                         $"ΔV: {result.totalDeltaV:F0} m/s | " +
                         $"Time: {result.timeDays:F0}d");
            }
            
            // Create next generation
            CreateNextGeneration();
            
            // Yield every generation to prevent freezing
            if (gen % 5 == 0)
            {
                yield return null;
            }
            
            if (!isRunning) break;
        }
        
        // Final results
        DisplayResults();
        
        isRunning = false;
        statusText = "Complete";
    }
    
    // =============================================================================
    // POPULATION INITIALIZATION
    // =============================================================================
    
    private void InitializePopulation()
    {
        population = new List<Chromosome>(populationSize);
        
        // Create 5 seeded "smart" solutions
        for (int i = 0; i < 5; i++)
        {
            Chromosome seed = new Chromosome();
            seed.LaunchVelocityX = RandomDouble(0, 5000);
            seed.LaunchVelocityY = RandomDouble(8000, 12000);
            seed.Maneuver1Time = RandomDouble(100, 400);
            seed.Maneuver1DeltaVX = RandomDouble(-1000, 1000);
            seed.Maneuver1DeltaVY = RandomDouble(-1000, 1000);
            seed.Maneuver2Time = RandomDouble(400, 800);
            seed.Maneuver2DeltaVX = RandomDouble(-1000, 1000);
            seed.Maneuver2DeltaVY = RandomDouble(-1000, 1000);
            population.Add(seed);
        }
        
        // Fill rest with random
        while (population.Count < populationSize)
        {
            population.Add(CreateRandomChromosome());
        }
        
        Debug.Log($"Initialized population with {populationSize} chromosomes");
    }
    
    private Chromosome CreateRandomChromosome()
    {
        Chromosome chrom = new Chromosome();
        chrom.LaunchVelocityX = RandomDouble(-10000, 10000);
        chrom.LaunchVelocityY = RandomDouble(5000, 15000);
        chrom.Maneuver1Time = RandomDouble(50, 600);
        chrom.Maneuver1DeltaVX = RandomDouble(-3000, 3000);
        chrom.Maneuver1DeltaVY = RandomDouble(-3000, 3000);
        chrom.Maneuver2Time = RandomDouble(200, 1000);
        chrom.Maneuver2DeltaVX = RandomDouble(-3000, 3000);
        chrom.Maneuver2DeltaVY = RandomDouble(-3000, 3000);
        return chrom;
    }
    
    // =============================================================================
    // FITNESS EVALUATION
    // =============================================================================
    
    private void EvaluatePopulation()
    {
        foreach (var chromosome in population)
        {
            chromosome.fitness = CalculateFitness(chromosome);
        }
    }
    
    private float CalculateFitness(Chromosome chromosome)
    {
        TrajectoryResult result = SimulateTrajectory(chromosome);
        
        // Severe penalties
        if (result.crashed) return 10000f;
        if (result.escaped) return 10000f;
        if (double.IsInfinity(result.minDistanceToJupiter)) return 10000f;
        
        // Calculate fitness components
        float distancePenalty = (float)(result.minDistanceToJupiter / AU) * 2.0f;
        float fuelPenalty = (float)(result.totalDeltaV / 10000.0) * 0.3f;
        float timePenalty = (float)(result.timeDays / 365.0) * 0.1f;
        
        // Bonus for getting close
        float arrivalBonus = 0f;
        if (result.minDistanceToJupiter < 0.5 * AU)
        {
            arrivalBonus = (float)(50.0 / (result.minDistanceToJupiter / AU + 0.1));
        }
        
        return distancePenalty + fuelPenalty + timePenalty - arrivalBonus;
    }
    
    // =============================================================================
    // TRAJECTORY SIMULATION
    // =============================================================================
    
    private TrajectoryResult SimulateTrajectory(Chromosome chromosome)
    {
        TrajectoryResult result = new TrajectoryResult();
        
        int maxSteps = (int)(maxDays * 24 / (timeStep / 3600.0));
        
        // Get Earth's starting position
        AstroTime currentTime = new AstroTime(startDate);
        DateTime currentDateTime = startDate;
        
        AstroVector earthPosVec = Astronomy.HelioVector(Body.Earth, currentTime);
        Vector2d earthPos = new Vector2d(earthPosVec.x * AU, earthPosVec.y * AU);
        
        // Initialize spacecraft
        Vector2d craftPos = earthPos;
        double earthOrbitalVel = 29780.0;
        Vector2d craftVel = new Vector2d(chromosome.LaunchVelocityX, 
                                         earthOrbitalVel + chromosome.LaunchVelocityY);
        
        result.totalDeltaV = Math.Sqrt(chromosome.LaunchVelocityX * chromosome.LaunchVelocityX + 
                                       chromosome.LaunchVelocityY * chromosome.LaunchVelocityY);
        
        // Maneuver schedule
        int maneuver1Step = (int)(chromosome.Maneuver1Time * 24 / (timeStep / 3600.0));
        int maneuver2Step = (int)(chromosome.Maneuver2Time * 24 / (timeStep / 3600.0));
        
        bool leftEarth = false;
        
        // Simulation loop
        for (int step = 0; step < maxSteps; step++)
        {
            // Apply maneuvers
            if (step == maneuver1Step)
            {
                Vector2d dv1 = new Vector2d(chromosome.Maneuver1DeltaVX, chromosome.Maneuver1DeltaVY);
                craftVel = craftVel + dv1;
                result.totalDeltaV += dv1.Magnitude();
            }
            else if (step == maneuver2Step)
            {
                Vector2d dv2 = new Vector2d(chromosome.Maneuver2DeltaVX, chromosome.Maneuver2DeltaVY);
                craftVel = craftVel + dv2;
                result.totalDeltaV += dv2.Magnitude();
            }
            
            // Check if left Earth
            double distFromEarth = (craftPos - earthPos).Magnitude();
            if (distFromEarth > 5e7) leftEarth = true;
            
            // Advance time
            currentDateTime = currentDateTime.AddSeconds(timeStep);
            if (currentDateTime > endDate) break;
            
            currentTime = new AstroTime(currentDateTime);
            
            // Calculate gravitational acceleration
            Vector2d acc = new Vector2d(0, 0);
            Vector2d jupiterPos = new Vector2d(0, 0);
            
            foreach (var kvp in PlanetMasses)
            {
                Body planet = kvp.Key;
                double mass = kvp.Value;
                
                Vector2d planetPos;
                if (planet == Body.Sun)
                {
                    planetPos = new Vector2d(0, 0);
                }
                else
                {
                    AstroVector pv = Astronomy.HelioVector(planet, currentTime);
                    planetPos = new Vector2d(pv.x * AU, pv.y * AU);
                }
                
                if (planet == Body.Jupiter)
                {
                    jupiterPos = planetPos;
                }
                
                Vector2d rVec = planetPos - craftPos;
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
                        result.crashed = true;
                        break;
                    }
                }
                
                if (r < 1e3) continue;
                
                double a = G * mass / (r * r);
                acc = acc + (rVec.Normalized() * a);
            }
            
            if (result.crashed) break;
            
            // Update velocity and position
            craftVel = craftVel + (acc * timeStep);
            craftPos = craftPos + (craftVel * timeStep);
            
            // Check escape
            double distFromSun = craftPos.Magnitude();
            if (distFromSun > 10 * AU)
            {
                result.escaped = true;
                break;
            }
            
            // Track distance to Jupiter
            if (step % 10 == 0)
            {
                double distToJupiter = (craftPos - jupiterPos).Magnitude();
                result.minDistanceToJupiter = Math.Min(result.minDistanceToJupiter, distToJupiter);
                
                // Early termination
                if (step > 200 && distFromSun > 7 * AU)
                {
                    if (distToJupiter > 3 * AU && distToJupiter > result.minDistanceToJupiter * 2.0)
                    {
                        break;
                    }
                }
            }
        }
        
        result.timeDays = (currentDateTime - startDate).TotalDays;
        result.finalPosition = craftPos;
        
        return result;
    }
    
    // =============================================================================
    // GA OPERATORS
    // =============================================================================
    
    private void CreateNextGeneration()
    {
        List<Chromosome> newPopulation = new List<Chromosome>(populationSize);
        
        // Elitism - keep best
        var elite = population.OrderBy(c => c.fitness).Take(eliteCount).ToList();
        foreach (var e in elite)
        {
            newPopulation.Add(e.Clone());
        }
        
        // Generate rest through selection, crossover, mutation
        while (newPopulation.Count < populationSize)
        {
            Chromosome parent1 = TournamentSelection();
            Chromosome parent2 = TournamentSelection();
            
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
        Chromosome best = null;
        float bestFit = float.MaxValue;
        
        for (int i = 0; i < tournamentSize; i++)
        {
            int idx = random.Next(population.Count);
            if (population[idx].fitness < bestFit)
            {
                best = population[idx];
                bestFit = population[idx].fitness;
            }
        }
        
        return best;
    }
    
    private (Chromosome, Chromosome) Crossover(Chromosome parent1, Chromosome parent2)
    {
        Chromosome child1 = new Chromosome();
        Chromosome child2 = new Chromosome();
        
        int crossPoint = random.Next(1, parent1.genes.Length);
        
        for (int i = 0; i < parent1.genes.Length; i++)
        {
            if (i < crossPoint)
            {
                child1.genes[i] = parent1.genes[i];
                child2.genes[i] = parent2.genes[i];
            }
            else
            {
                child1.genes[i] = parent2.genes[i];
                child2.genes[i] = parent1.genes[i];
            }
        }
        
        return (child1, child2);
    }
    
    private Chromosome Mutate(Chromosome chromosome)
    {
        Chromosome mutated = chromosome.Clone();
        
        for (int i = 0; i < mutated.genes.Length; i++)
        {
            if (RandomDouble(0, 1) < mutationRate)
            {
                double noise = RandomGaussian() * (Math.Abs(mutated.genes[i]) * mutationStrength + 100);
                mutated.genes[i] += noise;
                
                // Enforce bounds
                if (i == 0) mutated.genes[i] = Clamp(mutated.genes[i], -10000, 10000);
                else if (i == 1) mutated.genes[i] = Clamp(mutated.genes[i], 5000, 15000);
                else if (i == 2) mutated.genes[i] = Clamp(mutated.genes[i], 50, 600);
                else if (i == 5) mutated.genes[i] = Clamp(mutated.genes[i], 200, 1000);
                else mutated.genes[i] = Clamp(mutated.genes[i], -3000, 3000);
            }
        }
        
        return mutated;
    }
    
    // =============================================================================
    // RESULTS
    // =============================================================================
    
    private void DisplayResults()
    {
        if (bestChromosome == null)
        {
            Debug.LogWarning("No best solution found!");
            return;
        }
        
        TrajectoryResult result = SimulateTrajectory(bestChromosome);
        
        Debug.Log("\n" + new string('=', 60));
        Debug.Log("OPTIMIZATION COMPLETE!");
        Debug.Log(new string('=', 60));
        Debug.Log("\nBest Solution Found:");
        Debug.Log($"  Minimum Distance to Jupiter: {result.minDistanceToJupiter / AU:F4} AU ({result.minDistanceToJupiter / 1e6:F0} km)");
        Debug.Log($"  Total Delta-V: {result.totalDeltaV:F0} m/s");
        Debug.Log($"  Mission Time: {result.timeDays:F0} days ({result.timeDays / 365:F2} years)");
        Debug.Log($"  Fitness Score: {bestFitness:F2}");
        Debug.Log($"\n  Launch Velocity: [{bestChromosome.LaunchVelocityX:F0}, {bestChromosome.LaunchVelocityY:F0}] m/s");
        Debug.Log($"  Maneuver 1 (Day {bestChromosome.Maneuver1Time:F0}): ΔV = [{bestChromosome.Maneuver1DeltaVX:F0}, {bestChromosome.Maneuver1DeltaVY:F0}] m/s");
        Debug.Log($"  Maneuver 2 (Day {bestChromosome.Maneuver2Time:F0}): ΔV = [{bestChromosome.Maneuver2DeltaVX:F0}, {bestChromosome.Maneuver2DeltaVY:F0}] m/s");
        Debug.Log(new string('=', 60) + "\n");
    }
    
    // =============================================================================
    // UTILITY FUNCTIONS
    // =============================================================================
    
    private double RandomDouble(double min, double max)
    {
        return min + random.NextDouble() * (max - min);
    }
    
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
    
    // =============================================================================
    // PUBLIC API
    // =============================================================================
    
    public Chromosome GetBestChromosome()
    {
        return bestChromosome;
    }
    
    public float GetBestFitness()
    {
        return bestFitness;
    }
    
    public List<float> GetFitnessHistory()
    {
        return fitnessHistory;
    }
}

