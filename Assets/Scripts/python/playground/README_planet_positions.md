# Solar System Planet Position Calculator

This script calculates the heliocentric positions of all solar system planets (excluding Pluto) for a given date and time using the Skyfield library and JPL ephemeris files.

## Features

- **Heliocentric Reference Frame**: All positions are calculated relative to the Sun
- **High Accuracy**: Uses JPL DE421 ephemeris (valid from 1900 to 2053)
- **3D Coordinates**: Returns x, y, z positions in Astronomical Units (AU)
- **ICRF Frame**: Uses the International Celestial Reference Frame
- **User-Friendly**: Interactive command-line interface
- **Export Option**: Save results to a text file

## Installation

1. Install Python 3.7 or higher
2. Install required dependencies:

```bash
pip install -r requirements.txt
```

Or install Skyfield directly:

```bash
pip install skyfield
```

## Usage

### Interactive Mode

Run the script and follow the prompts:

```bash
python get_planet_positions.py
```

You'll be asked to enter:
- Date in `dd-mm-yyyy` format (e.g., `05-11-2025`)
- Time in 24-hour `HH:MM:SS` format (e.g., `14:30:00`)

### Programmatic Use

You can also import and use the function in your own code:

```python
from get_planet_positions import get_planet_positions

# Get positions for November 5, 2025 at 14:30:00 UTC
positions = get_planet_positions("05-11-2025", "14:30:00")

# Access individual planet coordinates
mercury_x = positions['Mercury']['x']
mercury_y = positions['Mercury']['y']
mercury_z = positions['Mercury']['z']
```

## Output

The script outputs a table with heliocentric positions:

```
Heliocentric Positions for 05-11-2025 at 14:30:00 UTC
======================================================================
Planet       X (AU)               Y (AU)               Z (AU)              
----------------------------------------------------------------------
Mercury      0.1234567890         -0.2345678901        0.0123456789
Venus        0.7123456789         0.0234567890         -0.0345678901
...
```

## Understanding the Coordinates

- **X, Y, Z**: Cartesian coordinates in the heliocentric reference frame
- **Units**: Astronomical Units (1 AU ≈ 149.6 million km)
- **Reference Frame**: ICRF (International Celestial Reference Frame)
  - X-axis: Points toward the vernal equinox (J2000)
  - Y-axis: Points 90° east along the celestial equator
  - Z-axis: Points toward the North Celestial Pole
- **Origin**: Center of the Sun

## Planets Included

1. Mercury
2. Venus
3. Earth
4. Mars
5. Jupiter
6. Saturn
7. Uranus
8. Neptune

**Note**: Pluto is excluded as requested (it was reclassified as a dwarf planet in 2006).

## Ephemeris File

The script uses the JPL DE421 ephemeris file (`de421.bsp`):
- **Coverage**: 1900-07-28 to 2053-10-08
- **Accuracy**: High precision planetary ephemeris
- **Size**: ~17 MB
- **Download**: Automatically downloaded on first run and cached locally

If you need a different date range, you can modify the script to use:
- `de422.bsp`: 1900-2050 (similar to DE421)
- `de430t.bsp`: 1550-2650 (larger file, ~128 MB)
- `de440s.bsp`: 1849-2150 (modern, recommended for recent dates)

## Technical Details

### Why Barycenters?

The script uses planet barycenters instead of planet centers. For most planets (except Earth-Moon), the barycenter is essentially the same as the planet center because the planet is much more massive than its moons. For Earth, the Earth-Moon barycenter is still within the Earth.

### Coordinate System

The positions are given in the **ICRF (International Celestial Reference Frame)**:
- This is a high-precision replacement for the J2000 reference system
- The axes are fixed relative to distant quasars
- It's the standard reference frame used in modern astronomy

### Heliocentric vs Barycentric

- **Heliocentric**: Positions relative to the Sun's center (used in this script)
- **Barycentric**: Positions relative to the Solar System's center of mass

For most purposes, the difference is negligible (the Sun contains 99.86% of the Solar System's mass).

## Example Use Cases

1. **3D Solar System Visualization**: Use coordinates to plot planet positions
2. **Orbital Mechanics**: Calculate orbital parameters and trajectories
3. **Astronomical Calculations**: Determine planet configurations and aspects
4. **Space Mission Planning**: Calculate transfer orbits and trajectories
5. **Educational Projects**: Demonstrate planetary motion and Kepler's laws

## Troubleshooting

### Date Out of Range Error
If you get an error about dates being out of range, make sure your date is between 1900-07-28 and 2053-10-08. For dates outside this range, modify the script to use a different ephemeris file.

### Download Issues
If the ephemeris file fails to download, you can manually download it from:
https://naif.jpl.nasa.gov/pub/naif/generic_kernels/spk/planets/

Place the file in Skyfield's data directory (usually `~/.skyfield-data/` or `%USERPROFILE%\.skyfield-data\` on Windows).

## References

- [Skyfield Documentation](https://rhodesmill.org/skyfield/)
- [JPL Ephemeris Files](https://ssd.jpl.nasa.gov/planets/eph_export.html)
- [ICRF Definition](https://www.iers.org/IERS/EN/DataProducts/ICRF/ICRF.html)

## License

This script is provided as-is for educational and research purposes.

