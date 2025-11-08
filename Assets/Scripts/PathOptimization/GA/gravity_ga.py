import numpy as np
import matplotlib.pyplot as plt
from copy import deepcopy
import time
from datetime import datetime, timedelta
import sys
import os

# Add parent directory to path to access de421.bsp
sys.path.append(os.path.join(os.path.dirname(__file__), '..', '..', 'python'))

try:
    from skyfield.api import load
    SKYFIELD_AVAILABLE = True
except ImportError:
    SKYFIELD_AVAILABLE = False
    print("Warning: Skyfield not available. Install with: pip install skyfield")

# --- Simulation parameters ---
G = 6.67430e-11  # gravitational constant
AU = 1.496e11    # Astronomical Unit in meters

# --- Mission Timeline ---
START_DATE = datetime(2028, 3, 3, 0, 0, 0)  # March 3, 2028
END_DATE = datetime(2035, 12, 31, 23, 59, 59)  # December 31, 2035

# --- Planet masses (kg) ---
PLANET_MASSES = {
    'sun': 1.989e30,
    'mercury': 3.285e23,
    'venus': 4.867e24,
    'earth': 5.972e24,
    'mars': 6.39e23,
    'jupiter': 1.898e27,
    'saturn': 5.683e26,
}

# --- Planet colors for visualization ---
PLANET_COLORS = {
    'sun': 'yellow',
    'mercury': 'gray',
    'venus': 'orange',
    'earth': 'blue',
    'mars': 'red',
    'jupiter': 'brown',
    'saturn': 'gold',
}

# --- Load Skyfield ephemeris ---
if SKYFIELD_AVAILABLE:
    try:
        ts = load.timescale()
        eph_path = os.path.join(os.path.dirname(__file__), '..', '..', 'python', 'de421.bsp')
        eph = load(eph_path)
        print(f"✓ Loaded ephemeris: {eph_path}")
    except Exception as e:
        print(f"Error loading ephemeris: {e}")
        SKYFIELD_AVAILABLE = False

def get_planet_position(planet_name, date_time):
    """
    Get heliocentric position of a planet at a specific datetime.
    Returns 2D position (x, y) in meters, or None if unavailable.
    """
    if not SKYFIELD_AVAILABLE:
        # Fallback to simplified circular orbits
        return get_simplified_position(planet_name, date_time)
    
    try:
        # Convert datetime to Skyfield time
        t = ts.utc(date_time.year, date_time.month, date_time.day,
                   date_time.hour, date_time.minute, date_time.second)
        
        # Planet mapping
        planet_map = {
            'mercury': 'mercury barycenter',
            'venus': 'venus barycenter',
            'earth': 'earth barycenter',
            'mars': 'mars barycenter',
            'jupiter': 'jupiter barycenter',
            'saturn': 'saturn barycenter',
        }
        
        if planet_name == 'sun':
            return np.array([0.0, 0.0])
        
        if planet_name not in planet_map:
            return None
        
        # Get heliocentric position
        sun = eph['sun']
        planet = eph[planet_map[planet_name]]
        heliocentric = sun.at(t).observe(planet)
        x_au, y_au, z_au = heliocentric.position.au
        
        # Convert to meters (use XY plane for 2D simulation)
        return np.array([x_au * AU, y_au * AU])
        
    except Exception as e:
        print(f"Error getting position for {planet_name}: {e}")
        return get_simplified_position(planet_name, date_time)

def get_simplified_position(planet_name, date_time):
    """
    Fallback simplified circular orbits if ephemeris unavailable.
    Planets orbit at constant angular velocity.
    """
    # Orbital radii and periods (approximate)
    orbits = {
        'sun': (0.0, 0.0),
        'mercury': (0.39 * AU, 88),
        'venus': (0.72 * AU, 225),
        'earth': (1.0 * AU, 365.25),
        'mars': (1.52 * AU, 687),
        'jupiter': (5.2 * AU, 4333),
        'saturn': (9.54 * AU, 10759),
    }
    
    if planet_name not in orbits:
        return None
    
    radius, period_days = orbits[planet_name]
    
    if radius == 0.0:
        return np.array([0.0, 0.0])
    
    # Calculate angle based on days since start
    days_elapsed = (date_time - START_DATE).total_seconds() / 86400.0
    angle = 2 * np.pi * (days_elapsed / period_days)
    
    x = radius * np.cos(angle)
    y = radius * np.sin(angle)
    
    return np.array([x, y])

# =============================================================================
# TRAJECTORY SIMULATION
# =============================================================================

def simulate_trajectory(chromosome, visualize=False):
    """
    Simulate spacecraft trajectory with real planet ephemeris positions.
    
    Chromosome structure:
    [0]: launch_velocity_x (m/s)
    [1]: launch_velocity_y (m/s)
    [2]: maneuver1_time (days)
    [3]: maneuver1_delta_vx (m/s)
    [4]: maneuver1_delta_vy (m/s)
    [5]: maneuver2_time (days)
    [6]: maneuver2_delta_vx (m/s)
    [7]: maneuver2_delta_vy (m/s)
    """
    # Use larger time step for faster simulation
    dt = 7200.0 if not visualize else 3600.0  # 2 hours (1 hour for visualization)
    max_days = 1200  # Increased to allow reaching Jupiter (typically 2-3 years)
    max_steps = int(max_days * 24 / (dt / 3600.0))
    
    # Initial conditions - start at Earth's position on START_DATE
    current_datetime = START_DATE
    earth_pos = get_planet_position('earth', current_datetime)
    
    if earth_pos is None:
        raise ValueError("Could not get Earth position!")
    
    craft_pos = earth_pos.copy()
    
    # Earth's orbital velocity (approximate, tangent to orbit)
    earth_orbital_vel = 29780.0  # m/s
    
    # Launch velocity relative to Earth
    craft_vel = np.array([chromosome[0], earth_orbital_vel + chromosome[1]])
    
    # Maneuver schedule
    maneuvers = []
    for i in range(2):  # 2 maneuvers
        maneuver_step = int(chromosome[2 + i*3] * 24 / (dt / 3600.0))  # Convert days to steps
        delta_v = np.array([chromosome[3 + i*3], chromosome[4 + i*3]])
        maneuvers.append({"step": maneuver_step, "delta_v": delta_v})
    
    # Tracking variables
    min_distance_to_jupiter = float('inf')
    total_delta_v = np.linalg.norm(craft_vel - np.array([0, earth_orbital_vel]))
    trajectory = []
    crashed = False
    escaped = False
    left_earth = False  # Track if spacecraft has left Earth's vicinity
    
    # Planet list to track (skip Saturn for speed)
    planet_names = ['sun', 'earth', 'mars', 'jupiter']
    
    # Simulation loop
    for step in range(max_steps):
        # Apply maneuvers
        for maneuver in maneuvers:
            if step == maneuver["step"]:
                craft_vel += maneuver["delta_v"]
                total_delta_v += np.linalg.norm(maneuver["delta_v"])
        
        # Check if left Earth's vicinity
        dist_from_earth = np.linalg.norm(craft_pos - earth_pos)
        if dist_from_earth > 5e7:  # 50,000 km
            left_earth = True
        
        # Get current planet positions from ephemeris
        current_datetime += timedelta(seconds=dt)
        
        # Check if we've exceeded mission end date
        if current_datetime > END_DATE:
            break
        
        # Compute gravitational acceleration from all planets
        acc = np.zeros(2)
        jupiter_pos = None
        
        for planet_name in planet_names:
            planet_pos = get_planet_position(planet_name, current_datetime)
            
            if planet_pos is None:
                continue
            
            # Track Jupiter position for distance calculation
            if planet_name == 'jupiter':
                jupiter_pos = planet_pos
            
            r_vec = planet_pos - craft_pos
            r = np.linalg.norm(r_vec)
            
            # Crash detection (use realistic planet radii + safety margin)
            if planet_name == 'jupiter':
                crash_radius = 7.5e7  # Jupiter radius ~71,000 km
            elif planet_name == 'sun':
                crash_radius = 7e8    # Sun radius ~696,000 km  
            else:
                crash_radius = 7e6    # Other planets
            
            # Only check crashes after leaving Earth
            if r < crash_radius:
                if planet_name == 'earth' and not left_earth:
                    pass  # Still near Earth at launch
                else:
                    crashed = True
                    break
            
            if r < 1e3:  # Avoid division by zero
                continue
            
            # Get planet mass
            planet_mass = PLANET_MASSES.get(planet_name, 0)
            
            # Calculate gravitational acceleration
            a = G * planet_mass / r**2
            acc += a * (r_vec / r)
        
        if crashed:
            break
        
        # Update velocity and position
        craft_vel += acc * dt
        craft_pos += craft_vel * dt
        
        # Check if escaped solar system
        distance_from_sun = np.linalg.norm(craft_pos)
        if distance_from_sun > 10 * AU:
            escaped = True
            break
        
        # Track distance to Jupiter (only every 10 steps for speed)
        if step % 10 == 0 and jupiter_pos is not None:
            dist_to_jupiter = np.linalg.norm(craft_pos - jupiter_pos)
            min_distance_to_jupiter = min(min_distance_to_jupiter, dist_to_jupiter)
            
            # Early termination only if way past Jupiter and clearly moving away
            if step > 200:
                dist_from_sun = np.linalg.norm(craft_pos)
                if dist_from_sun > 7 * AU:  # Well past Jupiter's orbit
                    if dist_to_jupiter > 3 * AU and dist_to_jupiter > min_distance_to_jupiter * 2.0:
                        break  # Clearly moving away from Jupiter
        
        if visualize and step % 200 == 0:  # Reduced frequency
            trajectory.append(craft_pos.copy())
    
    # Calculate final metrics
    final_time_days = step * dt / (24 * 3600)
    
    return {
        'min_distance': min_distance_to_jupiter,
        'total_delta_v': total_delta_v,
        'time_days': final_time_days,
        'crashed': crashed,
        'escaped': escaped,
        'trajectory': trajectory,
        'final_pos': craft_pos
    }

# =============================================================================
# FITNESS FUNCTION
# =============================================================================

def fitness_function(chromosome):
    """Calculate fitness for a trajectory."""
    result = simulate_trajectory(chromosome)
    
    # Severe penalties for bad outcomes
    if result['crashed']:
        return 10000.0
    if result['escaped']:
        return 10000.0
    
    # Distance penalty (primary objective)
    distance_penalty = result['min_distance'] / AU
    
    # Cap infinite distances
    if not np.isfinite(distance_penalty):
        return 10000.0
    
    # Fuel penalty
    fuel_penalty = result['total_delta_v'] / 10000.0
    
    # Time penalty (prefer faster missions)
    time_penalty = result['time_days'] / 365.0
    
    # Bonus for getting close to Jupiter
    arrival_bonus = 0.0
    if result['min_distance'] < 0.5 * AU:  # Within reasonable capture distance
        arrival_bonus = 50.0 / (result['min_distance'] / AU + 0.1)
    
    # Combined fitness (minimize this)
    fitness = (
        distance_penalty * 2.0 +
        fuel_penalty * 0.3 +
        time_penalty * 0.1 -
        arrival_bonus
    )
    
    return fitness

# =============================================================================
# GENETIC ALGORITHM OPERATORS
# =============================================================================

def create_random_chromosome():
    """Create a random chromosome within bounds."""
    chromosome = [
        np.random.uniform(-10000, 10000),   # launch_vx
        np.random.uniform(5000, 15000),     # launch_vy boost
        np.random.uniform(50, 600),         # maneuver1 time (days) - extended range
        np.random.uniform(-3000, 3000),     # maneuver1 dvx
        np.random.uniform(-3000, 3000),     # maneuver1 dvy
        np.random.uniform(200, 1000),       # maneuver2 time (days) - extended range
        np.random.uniform(-3000, 3000),     # maneuver2 dvx
        np.random.uniform(-3000, 3000),     # maneuver2 dvy
    ]
    return chromosome

def tournament_selection(population, fitnesses, tournament_size=3):
    """Select parent using tournament selection."""
    indices = np.random.choice(len(population), tournament_size, replace=False)
    best_idx = indices[np.argmin([fitnesses[i] for i in indices])]
    return population[best_idx]

def crossover(parent1, parent2):
    """Single-point crossover."""
    point = np.random.randint(1, len(parent1))
    child1 = parent1[:point] + parent2[point:]
    child2 = parent2[:point] + parent1[point:]
    return child1, child2

def mutate(chromosome, mutation_rate=0.1, mutation_strength=0.2):
    """Mutate chromosome with Gaussian noise."""
    mutated = chromosome.copy()
    for i in range(len(mutated)):
        if np.random.random() < mutation_rate:
            # Add Gaussian noise proportional to the value
            noise = np.random.normal(0, abs(mutated[i]) * mutation_strength + 100)
            mutated[i] += noise
            
            # Enforce bounds
            if i == 0:  # launch_vx
                mutated[i] = np.clip(mutated[i], -10000, 10000)
            elif i == 1:  # launch_vy boost
                mutated[i] = np.clip(mutated[i], 5000, 15000)
            elif i == 2:  # maneuver1 time
                mutated[i] = np.clip(mutated[i], 50, 600)
            elif i == 5:  # maneuver2 time
                mutated[i] = np.clip(mutated[i], 200, 1000)
            else:  # delta_v values
                mutated[i] = np.clip(mutated[i], -3000, 3000)
    
    return mutated

# =============================================================================
# GENETIC ALGORITHM MAIN LOOP
# =============================================================================

def run_ga(population_size=40, generations=75, visualize_best=True):
    """Run the genetic algorithm."""
    print("=" * 60)
    print("GENETIC ALGORITHM: Earth to Jupiter Trajectory Optimization")
    print("=" * 60)
    print(f"Mission Timeline: {START_DATE.strftime('%B %d, %Y')} → {END_DATE.strftime('%B %d, %Y')}")
    print(f"Ephemeris: {'Real (Skyfield)' if SKYFIELD_AVAILABLE else 'Simplified'}")
    print("=" * 60 + "\n")
    
    # Initialize population with some seeded solutions (Hohmann-like transfers)
    population = []
    
    # Add some seeded "reasonable" solutions (Hohmann-like transfers)
    for i in range(5):
        seed = [
            np.random.uniform(0, 5000),      # launch_vx
            np.random.uniform(8000, 12000),  # launch_vy boost
            np.random.uniform(100, 400),     # maneuver1 time (spread out more)
            np.random.uniform(-1000, 1000),  # maneuver1 dvx
            np.random.uniform(-1000, 1000),  # maneuver1 dvy
            np.random.uniform(400, 800),     # maneuver2 time (later in mission)
            np.random.uniform(-1000, 1000),  # maneuver2 dvx
            np.random.uniform(-1000, 1000),  # maneuver2 dvy
        ]
        population.append(seed)
    
    # Fill rest with random
    while len(population) < population_size:
        population.append(create_random_chromosome())
    
    best_fitness_history = []
    avg_fitness_history = []
    best_chromosome_ever = population[0]  # Initialize with first chromosome
    best_fitness_ever = float('inf')
    
    for gen in range(generations):
        # Evaluate fitness
        fitnesses = [fitness_function(chrom) for chrom in population]
        
        # Track best
        best_idx = np.argmin(fitnesses)
        best_fitness = fitnesses[best_idx]
        avg_fitness = np.mean(fitnesses)
        
        if best_fitness < best_fitness_ever:
            best_fitness_ever = best_fitness
            best_chromosome_ever = population[best_idx].copy()
        
        best_fitness_history.append(best_fitness)
        avg_fitness_history.append(avg_fitness)
        
        # Print progress every 5 generations (faster updates)
        if gen % 5 == 0 or gen == generations - 1:
            result = simulate_trajectory(population[best_idx])
            print(f"Gen {gen:3d} | Fitness: {best_fitness:.2f} | "
                  f"Dist: {result['min_distance']/AU:.3f} AU | "
                  f"ΔV: {result['total_delta_v']:.0f} m/s | "
                  f"Time: {result['time_days']:.0f}d")
        
        # Create next generation
        new_population = []
        
        # Elitism: keep best 2
        elite_indices = np.argsort(fitnesses)[:2]
        for idx in elite_indices:
            new_population.append(population[idx])
        
        # Generate rest through crossover and mutation
        while len(new_population) < population_size:
            parent1 = tournament_selection(population, fitnesses)
            parent2 = tournament_selection(population, fitnesses)
            child1, child2 = crossover(parent1, parent2)
            child1 = mutate(child1)
            child2 = mutate(child2)
            new_population.extend([child1, child2])
        
        population = new_population[:population_size]
    
    # Final results
    print("\n" + "=" * 60)
    print("OPTIMIZATION COMPLETE!")
    print("=" * 60)
    
    result = simulate_trajectory(best_chromosome_ever)
    print(f"\nBest Solution Found:")
    print(f"  Minimum Distance to Jupiter: {result['min_distance']/AU:.4f} AU ({result['min_distance']/1e6:.0f} km)")
    print(f"  Total Delta-V: {result['total_delta_v']:.0f} m/s")
    print(f"  Mission Time: {result['time_days']:.0f} days ({result['time_days']/365:.2f} years)")
    print(f"  Fitness Score: {best_fitness_ever:.2f}")
    print(f"\n  Launch Velocity: [{best_chromosome_ever[0]:.0f}, {best_chromosome_ever[1]:.0f}] m/s")
    print(f"  Maneuver 1 (Day {best_chromosome_ever[2]:.0f}): ΔV = [{best_chromosome_ever[3]:.0f}, {best_chromosome_ever[4]:.0f}] m/s")
    print(f"  Maneuver 2 (Day {best_chromosome_ever[5]:.0f}): ΔV = [{best_chromosome_ever[6]:.0f}, {best_chromosome_ever[7]:.0f}] m/s")
    
    # Visualize best trajectory
    if visualize_best:
        visualize_trajectory(best_chromosome_ever, best_fitness_history, avg_fitness_history)
    
    return best_chromosome_ever, best_fitness_history

# =============================================================================
# VISUALIZATION
# =============================================================================

def visualize_trajectory(chromosome, best_history, avg_history):
    """Visualize the best trajectory and fitness evolution."""
    
    # Simulate with full trajectory tracking
    result = simulate_trajectory(chromosome, visualize=True)
    trajectory = np.array(result['trajectory'])
    
    # Create figure with subplots
    fig = plt.figure(figsize=(16, 8))
    
    # Subplot 1: Trajectory
    ax1 = fig.add_subplot(121)
    ax1.set_aspect('equal')
    ax1.set_xlim(-6 * AU, 6 * AU)
    ax1.set_ylim(-6 * AU, 6 * AU)
    
    # Plot planets at their positions throughout the trajectory
    # Sample a few key dates
    sample_dates = [START_DATE + timedelta(days=d) for d in [0, 200, 400, 600, 800, 1000]]
    
    for planet_name in ['sun', 'earth', 'mars', 'jupiter']:
        planet_positions = []
        for date in sample_dates:
            pos = get_planet_position(planet_name, date)
            if pos is not None:
                planet_positions.append(pos)
        
        if planet_positions:
            planet_positions = np.array(planet_positions)
            color = PLANET_COLORS.get(planet_name, 'white')
            size = 15 if planet_name == 'sun' else 8 if planet_name == 'jupiter' else 5
            
            # Plot start position
            ax1.plot(planet_positions[0, 0], planet_positions[0, 1], "o", 
                    color=color, markersize=size, label=planet_name.capitalize())
            
            # Plot orbit path
            if len(planet_positions) > 1:
                ax1.plot(planet_positions[:, 0], planet_positions[:, 1], 
                        "--", color=color, alpha=0.3, linewidth=0.5)
    
    # Plot trajectory
    if len(trajectory) > 0:
        ax1.plot(trajectory[:, 0], trajectory[:, 1], "cyan", 
                linewidth=1.5, alpha=0.7, label="Spacecraft")
        ax1.plot(trajectory[0, 0], trajectory[0, 1], "g*", 
                markersize=15, label="Launch")
        ax1.plot(trajectory[-1, 0], trajectory[-1, 1], "r*", 
                markersize=15, label="Final")
    
    ax1.set_facecolor("black")
    ax1.set_title("Best Trajectory: Earth to Jupiter (Real Ephemeris)", color="white", fontsize=14)
    ax1.set_xlabel("Distance (m)", color="white")
    ax1.set_ylabel("Distance (m)", color="white")
    ax1.tick_params(colors="white")
    ax1.legend(loc="upper right", facecolor="black", edgecolor="white", 
              labelcolor="white", fontsize=8)
    
    # Subplot 2: Fitness Evolution
    ax2 = fig.add_subplot(122)
    generations = range(len(best_history))
    ax2.plot(generations, best_history, 'g-', linewidth=2, label='Best Fitness')
    ax2.plot(generations, avg_history, 'b--', linewidth=1, label='Avg Fitness')
    ax2.set_xlabel("Generation", fontsize=12)
    ax2.set_ylabel("Fitness (lower is better)", fontsize=12)
    ax2.set_title("Fitness Evolution", fontsize=14)
    ax2.legend()
    ax2.grid(True, alpha=0.3)
    
    fig.patch.set_facecolor("white")
    plt.tight_layout()
    plt.show()

# =============================================================================
# MAIN
# =============================================================================

if __name__ == "__main__":
    # Run the genetic algorithm
    best_solution, history = run_ga(
        population_size=40,   # Balance between speed and solution quality
        generations=75,       # Enough to find good Jupiter trajectories
        visualize_best=True
    )
