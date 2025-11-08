"""
Example usage of the planet position calculator
Demonstrates various ways to use the get_planet_positions function
"""

from get_planet_positions import get_planet_positions
import json

def example_1_basic_usage():
    """
    Example 1: Basic usage - get positions for a specific date and time
    """
    print("\n" + "="*70)
    print("EXAMPLE 1: Basic Usage")
    print("="*70)
    
    # Get positions for November 5, 2025 at 14:30:00 UTC
    positions = get_planet_positions("05-11-2025", "14:30:00")
    
    # Access individual planet data
    earth_pos = positions['Earth']
    print(f"\nEarth's heliocentric position:")
    print(f"  X: {earth_pos['x']:.10f} AU")
    print(f"  Y: {earth_pos['y']:.10f} AU")
    print(f"  Z: {earth_pos['z']:.10f} AU")


def example_2_calculate_distance():
    """
    Example 2: Calculate distance between two planets
    """
    print("\n" + "="*70)
    print("EXAMPLE 2: Calculate Distance Between Planets")
    print("="*70)
    
    positions = get_planet_positions("01-01-2025", "00:00:00")
    
    # Get Earth and Mars positions
    earth = positions['Earth']
    mars = positions['Mars']
    
    # Calculate distance using Euclidean distance formula
    distance = ((earth['x'] - mars['x'])**2 + 
                (earth['y'] - mars['y'])**2 + 
                (earth['z'] - mars['z'])**2)**0.5
    
    print(f"\nDistance from Earth to Mars:")
    print(f"  {distance:.6f} AU")
    print(f"  {distance * 149597870.7:.2f} km")
    print(f"  {distance * 92955807.3:.2f} miles")


def example_3_export_to_json():
    """
    Example 3: Export positions to JSON format
    """
    print("\n" + "="*70)
    print("EXAMPLE 3: Export to JSON")
    print("="*70)
    
    positions = get_planet_positions("15-06-2025", "12:00:00")
    
    # Save to JSON file
    filename = "planet_positions.json"
    with open(filename, 'w') as f:
        json.dump(positions, f, indent=2)
    
    print(f"\nPositions exported to: {filename}")


def example_4_compare_positions():
    """
    Example 4: Compare positions at two different times
    """
    print("\n" + "="*70)
    print("EXAMPLE 4: Track Planet Movement Over Time")
    print("="*70)
    
    # Get positions at two different times (1 day apart)
    pos_day1 = get_planet_positions("01-01-2025", "00:00:00")
    pos_day2 = get_planet_positions("02-01-2025", "00:00:00")
    
    print("\nMercury's movement in 24 hours:")
    mercury1 = pos_day1['Mercury']
    mercury2 = pos_day2['Mercury']
    
    dx = mercury2['x'] - mercury1['x']
    dy = mercury2['y'] - mercury1['y']
    dz = mercury2['z'] - mercury1['z']
    
    distance_moved = (dx**2 + dy**2 + dz**2)**0.5
    
    print(f"  ΔX: {dx:.6f} AU")
    print(f"  ΔY: {dy:.6f} AU")
    print(f"  ΔZ: {dz:.6f} AU")
    print(f"  Total distance: {distance_moved:.6f} AU")
    print(f"  Total distance: {distance_moved * 149597870.7:.2f} km")


def example_5_distance_from_sun():
    """
    Example 5: Calculate each planet's distance from the Sun
    """
    print("\n" + "="*70)
    print("EXAMPLE 5: Distance from Sun")
    print("="*70)
    
    positions = get_planet_positions("21-03-2025", "12:00:00")
    
    print(f"\n{'Planet':<12} {'Distance (AU)':<18} {'Distance (km)':<20}")
    print("-" * 60)
    
    for planet_name, coords in positions.items():
        # Distance from Sun (origin) = magnitude of position vector
        distance_au = (coords['x']**2 + coords['y']**2 + coords['z']**2)**0.5
        distance_km = distance_au * 149597870.7
        
        print(f"{planet_name:<12} {distance_au:<18.6f} {distance_km:<20,.0f}")


def example_6_planetary_alignment():
    """
    Example 6: Check if planets are roughly aligned (simple check)
    """
    print("\n" + "="*70)
    print("EXAMPLE 6: Check Planetary Alignment")
    print("="*70)
    
    positions = get_planet_positions("01-07-2025", "00:00:00")
    
    # Calculate angles in XY plane (ecliptic plane approximation)
    import math
    
    print(f"\n{'Planet':<12} {'Angle (degrees)':<20}")
    print("-" * 40)
    
    for planet_name, coords in positions.items():
        angle = math.degrees(math.atan2(coords['y'], coords['x']))
        # Normalize to 0-360
        if angle < 0:
            angle += 360
        print(f"{planet_name:<12} {angle:<20.2f}")


def main():
    """
    Run all examples
    """
    print("\n" + "="*70)
    print("PLANET POSITION CALCULATOR - EXAMPLE USAGE")
    print("="*70)
    
    try:
        example_1_basic_usage()
        example_2_calculate_distance()
        example_3_export_to_json()
        example_4_compare_positions()
        example_5_distance_from_sun()
        example_6_planetary_alignment()
        
        print("\n" + "="*70)
        print("All examples completed successfully!")
        print("="*70 + "\n")
        
    except Exception as e:
        print(f"\nError running examples: {e}")


if __name__ == "__main__":
    main()

