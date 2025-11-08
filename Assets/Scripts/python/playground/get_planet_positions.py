"""
Script to get heliocentric positions of solar system planets
Uses Skyfield library with JPL ephemeris files
Heliocentric Reference Frame: positions relative to the Sun
"""

from skyfield.api import load
from datetime import datetime

def get_planet_positions(date_str, time_str):
    """
    Get heliocentric positions of all planets (excluding Pluto)
    
    Parameters:
    -----------
    date_str : str
        Date in dd-mm-yyyy format (e.g., "05-11-2025")
    time_str : str
        Time in 24-hour format HH:MM:SS (e.g., "14:30:00")
    
    Returns:
    --------
    dict : Dictionary containing planet names and their heliocentric (x, y, z) positions in AU
    """
    
    # Parse the input date and time
    try:
        day, month, year = map(int, date_str.split('-'))
        hour, minute, second = map(int, time_str.split(':'))
    except ValueError:
        raise ValueError("Invalid date or time format. Use dd-mm-yyyy for date and HH:MM:SS for time.")
    
    # Load timescale and ephemeris
    ts = load.timescale()
    
    # Load ephemeris file (de421.bsp covers 1900-2053)
    # This will download the file on first run and cache it
    eph = load('de421.bsp')
    
    # Create time object
    t = ts.utc(year, month, day, hour, minute, second)
    
    # Get the Sun (center of heliocentric reference frame)
    sun = eph['sun']
    
    # List of planets (excluding Pluto)
    # Using barycenters for consistency, as they're directly available
    planets = {
        'Mercury': 'mercury barycenter',
        'Venus': 'venus barycenter',
        'Earth': 'earth barycenter',
        'Mars': 'mars barycenter',
        'Jupiter': 'jupiter barycenter',
        'Saturn': 'saturn barycenter',
        'Uranus': 'uranus barycenter',
        'Neptune': 'neptune barycenter'
    }
    
    # Calculate heliocentric positions
    positions = {}
    
    print(f"\nHeliocentric Positions for {date_str} at {time_str} UTC")
    print("=" * 70)
    print(f"{'Planet':<12} {'X (AU)':<20} {'Y (AU)':<20} {'Z (AU)':<20}")
    print("-" * 70)
    
    for planet_name, planet_key in planets.items():
        # Get planet position
        planet = eph[planet_key]
        
        # Calculate heliocentric position (from Sun to planet)
        # This gives us the position vector in the heliocentric frame
        heliocentric = sun.at(t).observe(planet)
        
        # Get x, y, z coordinates in AU (Astronomical Units)
        x, y, z = heliocentric.position.au
        
        positions[planet_name] = {
            'x': x,
            'y': y,
            'z': z
        }
        
        print(f"{planet_name:<12} {x:<20.10f} {y:<20.10f} {z:<20.10f}")
    
    print("=" * 70)
    print("\nNote: Coordinates are in Astronomical Units (AU)")
    print("Reference Frame: Heliocentric (Sun-centered)")
    print("Coordinate System: ICRF (International Celestial Reference Frame)")
    print("\nEphemeris: JPL DE421 (valid from 1900 to 2053)")
    
    return positions


def main():
    """
    Main function to run the script
    """
    print("\n" + "="*70)
    print("SOLAR SYSTEM PLANET POSITION CALCULATOR")
    print("Heliocentric Reference Frame")
    print("="*70)
    
    # Get user input
    date_input = input("\nEnter date (dd-mm-yyyy): ")
    time_input = input("Enter time in 24-hour format (HH:MM:SS): ")
    
    try:
        # Get positions
        positions = get_planet_positions(date_input, time_input)
        
        # Optional: Save to file
        save_option = input("\nWould you like to save the results to a file? (y/n): ")
        if save_option.lower() == 'y':
            filename = f"planet_positions_{date_input.replace('-', '')}_{time_input.replace(':', '')}.txt"
            with open(filename, 'w') as f:
                f.write(f"Heliocentric Positions for {date_input} at {time_input} UTC\n")
                f.write("="*70 + "\n")
                f.write(f"{'Planet':<12} {'X (AU)':<20} {'Y (AU)':<20} {'Z (AU)':<20}\n")
                f.write("-"*70 + "\n")
                for planet_name, coords in positions.items():
                    f.write(f"{planet_name:<12} {coords['x']:<20.10f} {coords['y']:<20.10f} {coords['z']:<20.10f}\n")
                f.write("="*70 + "\n")
                f.write("\nNote: Coordinates are in Astronomical Units (AU)\n")
                f.write("Reference Frame: Heliocentric (Sun-centered)\n")
                f.write("Coordinate System: ICRF (International Celestial Reference Frame)\n")
            print(f"\nResults saved to: {filename}")
    
    except Exception as e:
        print(f"\nError: {e}")
        return
    
    print("\nDone!")


if __name__ == "__main__":
    main()

