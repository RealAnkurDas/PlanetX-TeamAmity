"""
Visualize the optimized trajectory from the Genetic Algorithm
Shows the spacecraft path from Earth to Jupiter in sped-up time
"""

import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from datetime import datetime, timedelta
import sys
import os

# Add parent directory to path
sys.path.append(os.path.join(os.path.dirname(__file__), '..', '..', 'python'))

try:
    from skyfield.api import load
    SKYFIELD_AVAILABLE = True
except ImportError:
    SKYFIELD_AVAILABLE = False
    print("Warning: Skyfield not available. Install with: pip install skyfield")
    print("Using simplified planetary positions instead.")

# === CONSTANTS ===
G = 6.67430e-11  # Gravitational constant
AU = 1.496e11    # Astronomical Unit in meters
START_DATE = datetime(2028, 3, 3, 0, 0, 0)

# === OPTIMIZED PARAMETERS FROM GA ===
LAUNCH_VX = 8080  # m/s
LAUNCH_VY = 5726  # m/s
MANEUVER_1_DAY = 414
MANEUVER_1_DVX = -392  # m/s
MANEUVER_1_DVY = -3000  # m/s
MANEUVER_2_DAY = 285
MANEUVER_2_DVX = 2714  # m/s
MANEUVER_2_DVY = -3000  # m/s
MISSION_DURATION = 1200  # days

# Planet masses (kg)
PLANET_MASSES = {
    'sun': 1.989e30,
    'earth': 5.972e24,
    'mars': 6.39e23,
    'jupiter': 1.898e27,
}

# === LOAD EPHEMERIS ===
if SKYFIELD_AVAILABLE:
    try:
        ephemeris_path = os.path.join(os.path.dirname(__file__), '..', '..', 'python', 'de421.bsp')
        if not os.path.exists(ephemeris_path):
            print("Downloading ephemeris data...")
            eph = load('de421.bsp')
        else:
            eph = load(ephemeris_path)
        
        sun = eph['sun']
        earth = eph['earth']
        mars = eph['mars barycenter']
        jupiter = eph['jupiter barycenter']
        
        ts = load.timescale()
        print("✓ Ephemeris loaded successfully")
    except Exception as e:
        print(f"Error loading ephemeris: {e}")
        SKYFIELD_AVAILABLE = False


def get_planet_position(planet_name, date):
    """Get planet position in meters (heliocentric)"""
    if SKYFIELD_AVAILABLE:
        t = ts.utc(date.year, date.month, date.day, date.hour, date.minute, date.second)
        
        if planet_name == 'earth':
            planet = earth
        elif planet_name == 'mars':
            planet = mars
        elif planet_name == 'jupiter':
            planet = jupiter
        else:
            return np.array([0.0, 0.0, 0.0])
        
        pos = planet.at(t).observe(sun).position.au
        return np.array([pos[0], pos[1], pos[2]]) * AU
    else:
        # Fallback: circular orbits (approximation)
        days_since_start = (date - START_DATE).total_seconds() / 86400
        
        if planet_name == 'earth':
            r = 1.0 * AU
            period = 365.25
        elif planet_name == 'mars':
            r = 1.52 * AU
            period = 687
        elif planet_name == 'jupiter':
            r = 5.2 * AU
            period = 4333
        else:
            return np.array([0.0, 0.0, 0.0])
        
        angle = 2 * np.pi * days_since_start / period
        return np.array([r * np.cos(angle), r * np.sin(angle), 0.0])


def compute_gravity(spacecraft_pos, date, planets=['sun', 'earth', 'mars', 'jupiter']):
    """Compute gravitational acceleration on spacecraft"""
    accel = np.zeros(3)
    
    for planet_name in planets:
        if planet_name == 'sun':
            planet_pos = np.array([0.0, 0.0, 0.0])
        else:
            planet_pos = get_planet_position(planet_name, date)
        
        r_vec = planet_pos - spacecraft_pos
        r_mag = np.linalg.norm(r_vec)
        
        if r_mag > 0:
            mass = PLANET_MASSES[planet_name]
            accel += G * mass * r_vec / (r_mag ** 3)
    
    return accel


def simulate_trajectory(dt=3600):  # 1 hour timestep
    """Simulate the spacecraft trajectory with optimized parameters"""
    print("\n" + "="*60)
    print("SIMULATING OPTIMIZED TRAJECTORY")
    print("="*60)
    print(f"Launch Velocity: [{LAUNCH_VX}, {LAUNCH_VY}] m/s")
    print(f"Maneuver 1 (Day {MANEUVER_1_DAY}): ΔV = [{MANEUVER_1_DVX}, {MANEUVER_1_DVY}] m/s")
    print(f"Maneuver 2 (Day {MANEUVER_2_DAY}): ΔV = [{MANEUVER_2_DVX}, {MANEUVER_2_DVY}] m/s")
    print(f"Mission Duration: {MISSION_DURATION} days")
    print("="*60 + "\n")
    
    # Initialize spacecraft at Earth's position
    current_date = START_DATE
    earth_pos = get_planet_position('earth', current_date)
    
    position = earth_pos.copy()
    velocity = np.array([LAUNCH_VX, LAUNCH_VY, 0.0])
    
    # Storage for trajectory
    positions = [position.copy()]
    dates = [current_date]
    
    # Simulate for mission duration
    total_steps = int(MISSION_DURATION * 86400 / dt)
    
    for step in range(total_steps):
        # Current mission time in days
        mission_day = step * dt / 86400
        
        # Apply maneuvers
        if abs(mission_day - MANEUVER_1_DAY) < 0.01:
            velocity += np.array([MANEUVER_1_DVX, MANEUVER_1_DVY, 0.0])
            print(f"⚡ Maneuver 1 applied at day {mission_day:.1f}")
        
        if abs(mission_day - MANEUVER_2_DAY) < 0.01:
            velocity += np.array([MANEUVER_2_DVX, MANEUVER_2_DVY, 0.0])
            print(f"⚡ Maneuver 2 applied at day {mission_day:.1f}")
        
        # Compute acceleration from gravity
        accel = compute_gravity(position, current_date)
        
        # Update velocity and position (simple Euler integration)
        velocity += accel * dt
        position += velocity * dt
        
        # Store trajectory every few steps (to reduce memory)
        if step % 24 == 0:  # Store every 24 hours
            positions.append(position.copy())
            dates.append(current_date)
        
        current_date += timedelta(seconds=dt)
        
        # Progress indicator
        if step % (total_steps // 20) == 0:
            print(f"Progress: {100*step/total_steps:.0f}%")
    
    # Calculate final distance to Jupiter
    final_jupiter_pos = get_planet_position('jupiter', dates[-1])
    final_distance = np.linalg.norm(positions[-1] - final_jupiter_pos)
    
    print(f"\n✓ Simulation complete!")
    print(f"Final distance to Jupiter: {final_distance/1000:.0f} km ({final_distance/AU:.4f} AU)")
    
    return np.array(positions), dates


def create_animation(positions, dates, speed_factor=100):
    """Create animated visualization of the trajectory"""
    print("\nCreating animation...")
    
    fig, ax = plt.subplots(figsize=(12, 12))
    ax.set_aspect('equal')
    ax.set_facecolor('black')
    fig.patch.set_facecolor('black')
    
    # Set plot limits (zoomed out to see full trajectory)
    max_dist_au = 8.0  # Show up to 8 AU
    ax.set_xlim(-max_dist_au, max_dist_au)
    ax.set_ylim(-max_dist_au, max_dist_au)
    
    # Convert to AU for plotting
    positions_au = positions / AU
    
    # Plot full trajectory as a faint line
    ax.plot(positions_au[:, 0], positions_au[:, 1], 'cyan', alpha=0.3, linewidth=0.5)
    
    # Sun
    ax.plot(0, 0, 'o', color='yellow', markersize=20, label='Sun')
    
    # Initialize moving objects
    spacecraft_dot, = ax.plot([], [], 'wo', markersize=8, label='Spacecraft')
    spacecraft_trail, = ax.plot([], [], 'c-', linewidth=1.5, alpha=0.7)
    
    earth_orbit = plt.Circle((0, 0), 1.0, fill=False, color='blue', alpha=0.3, linestyle='--')
    mars_orbit = plt.Circle((0, 0), 1.52, fill=False, color='red', alpha=0.3, linestyle='--')
    jupiter_orbit = plt.Circle((0, 0), 5.2, fill=False, color='brown', alpha=0.3, linestyle='--')
    ax.add_patch(earth_orbit)
    ax.add_patch(mars_orbit)
    ax.add_patch(jupiter_orbit)
    
    earth_dot, = ax.plot([], [], 'o', color='blue', markersize=10, label='Earth')
    mars_dot, = ax.plot([], [], 'o', color='red', markersize=7, label='Mars')
    jupiter_dot, = ax.plot([], [], 'o', color='brown', markersize=15, label='Jupiter')
    
    # Text for mission info
    info_text = ax.text(0.02, 0.98, '', transform=ax.transAxes, 
                        color='white', fontsize=10, verticalalignment='top',
                        bbox=dict(boxstyle='round', facecolor='black', alpha=0.7))
    
    ax.set_xlabel('X (AU)', color='white', fontsize=12)
    ax.set_ylabel('Y (AU)', color='white', fontsize=12)
    ax.set_title('Earth to Jupiter Trajectory - Optimized by Genetic Algorithm', 
                 color='white', fontsize=14, fontweight='bold')
    ax.legend(loc='upper right', facecolor='black', edgecolor='white', labelcolor='white')
    ax.tick_params(colors='white')
    ax.grid(True, alpha=0.2)
    
    trail_length = 50  # Show last 50 points in trail
    
    def init():
        spacecraft_dot.set_data([], [])
        spacecraft_trail.set_data([], [])
        earth_dot.set_data([], [])
        mars_dot.set_data([], [])
        jupiter_dot.set_data([], [])
        info_text.set_text('')
        return spacecraft_dot, spacecraft_trail, earth_dot, mars_dot, jupiter_dot, info_text
    
    def animate(frame):
        idx = frame * speed_factor
        if idx >= len(positions):
            idx = len(positions) - 1
        
        # Update spacecraft
        spacecraft_dot.set_data([positions_au[idx, 0]], [positions_au[idx, 1]])
        
        # Update trail
        start_idx = max(0, idx - trail_length)
        spacecraft_trail.set_data(positions_au[start_idx:idx, 0], 
                                  positions_au[start_idx:idx, 1])
        
        # Update planets
        current_date = dates[min(idx, len(dates)-1)]
        
        earth_pos = get_planet_position('earth', current_date) / AU
        mars_pos = get_planet_position('mars', current_date) / AU
        jupiter_pos = get_planet_position('jupiter', current_date) / AU
        
        earth_dot.set_data([earth_pos[0]], [earth_pos[1]])
        mars_dot.set_data([mars_pos[0]], [mars_pos[1]])
        jupiter_dot.set_data([jupiter_pos[0]], [jupiter_pos[1]])
        
        # Update info text
        mission_day = (current_date - START_DATE).days
        distance_to_jupiter = np.linalg.norm(positions[idx] - jupiter_pos * AU) / 1000
        
        info_text.set_text(f'Date: {current_date.strftime("%Y-%m-%d")}\n'
                          f'Mission Day: {mission_day}\n'
                          f'Distance to Jupiter: {distance_to_jupiter:.0f} km')
        
        return spacecraft_dot, spacecraft_trail, earth_dot, mars_dot, jupiter_dot, info_text
    
    # Create animation
    num_frames = len(positions) // speed_factor
    anim = animation.FuncAnimation(fig, animate, init_func=init, 
                                   frames=num_frames, interval=50, 
                                   blit=True, repeat=True)
    
    print("✓ Animation created!")
    print("\nShowing animation... (Close window when done)")
    plt.show()
    
    return anim


def main():
    print("\n" + "="*60)
    print("TRAJECTORY VISUALIZATION - GA OPTIMIZED SOLUTION")
    print("="*60)
    
    # Simulate the trajectory
    positions, dates = simulate_trajectory(dt=3600)  # 1-hour timesteps
    
    # Create animated visualization
    anim = create_animation(positions, dates, speed_factor=50)  # Speed up 50x
    
    print("\n✓ Visualization complete!")


if __name__ == '__main__':
    main()

