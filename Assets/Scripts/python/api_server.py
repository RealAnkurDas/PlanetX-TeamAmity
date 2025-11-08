"""
FastAPI server for planet position calculations
Provides endpoints to get heliocentric positions of planets in metres
"""

from fastapi import FastAPI, HTTPException, Query
from fastapi.responses import JSONResponse
from skyfield.api import load
from typing import Dict
import uvicorn

# Initialize FastAPI app
app = FastAPI(
    title="Planet Position API",
    description="Get heliocentric positions of solar system planets",
    version="1.0.0"
)

# Conversion constant: 1 AU to metres
AU_TO_METRES = 149597870700  # metres per AU

# Load ephemeris and timescale at startup (cache for performance)
ts = load.timescale()
eph = load('de421.bsp')

# Planet mapping
PLANETS = {
    'mercury': 'mercury barycenter',
    'venus': 'venus barycenter',
    'earth': 'earth barycenter',
    'mars': 'mars barycenter',
    'jupiter': 'jupiter barycenter',
    'saturn': 'saturn barycenter',
    'uranus': 'uranus barycenter',
    'neptune': 'neptune barycenter'
}


@app.get("/", tags=["Root"])
async def root():
    """
    Root endpoint with API information
    """
    return {
        "message": "Planet Position API",
        "version": "1.0.0",
        "endpoints": {
            "health": "/health",
            "coordinates": "/get_coords"
        }
    }


@app.get("/health", tags=["Health"])
async def health_check():
    """
    Health check endpoint
    Returns the status of the API
    """
    return {
        "status": "healthy",
        "service": "Planet Position API",
        "ephemeris": "JPL DE421",
        "ephemeris_range": "1900-07-28 to 2053-10-08"
    }


@app.get("/get_coords", tags=["Coordinates"])
async def get_coordinates(
    date: str = Query(
        ..., 
        description="Date in dd-mm-yyyy format (e.g., 05-11-2025)",
        example="05-11-2025"
    ),
    time: str = Query(
        ..., 
        description="Time in 24-hour HH:MM:SS format (e.g., 14:30:00)",
        example="14:30:00"
    )
):
    """
    Get heliocentric positions of all planets for a given date and time
    
    Parameters:
    - date: Date in dd-mm-yyyy format
    - time: Time in 24-hour HH:MM:SS format
    
    Returns:
    - Dictionary with planet names and their x, y, z positions in metres
    """
    try:
        # Parse date
        day, month, year = map(int, date.split('-'))
        
        # Parse time
        hour, minute, second = map(int, time.split(':'))
        
        # Validate ranges
        if not (1 <= day <= 31):
            raise ValueError("Day must be between 1 and 31")
        if not (1 <= month <= 12):
            raise ValueError("Month must be between 1 and 12")
        if not (1900 <= year <= 2053):
            raise ValueError("Year must be between 1900 and 2053 for DE421 ephemeris")
        if not (0 <= hour <= 23):
            raise ValueError("Hour must be between 0 and 23")
        if not (0 <= minute <= 59):
            raise ValueError("Minute must be between 0 and 59")
        if not (0 <= second <= 59):
            raise ValueError("Second must be between 0 and 59")
            
    except ValueError as e:
        raise HTTPException(
            status_code=400,
            detail=f"Invalid date or time format: {str(e)}"
        )
    
    try:
        # Create time object
        t = ts.utc(year, month, day, hour, minute, second)
        
        # Get the Sun
        sun = eph['sun']
        
        # Calculate positions
        positions = {}
        
        for planet_name, planet_key in PLANETS.items():
            # Get planet
            planet = eph[planet_key]
            
            # Calculate heliocentric position
            heliocentric = sun.at(t).observe(planet)
            
            # Get x, y, z in AU
            x_au, y_au, z_au = heliocentric.position.au
            
            # Convert to metres
            positions[planet_name] = {
                'x': float(x_au * AU_TO_METRES),
                'y': float(y_au * AU_TO_METRES),
                'z': float(z_au * AU_TO_METRES)
            }
        
        # Prepare response
        response = {
            "status": "success",
            "date": date,
            "time": time,
            "timestamp_utc": f"{year}-{month:02d}-{day:02d}T{hour:02d}:{minute:02d}:{second:02d}Z",
            "units": "metres",
            "reference_frame": "heliocentric",
            "coordinate_system": "ICRF",
            "planets": positions
        }
        
        return JSONResponse(content=response)
        
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Error calculating positions: {str(e)}"
        )


@app.get("/get_coords/{planet_name}", tags=["Coordinates"])
async def get_planet_coordinates(
    planet_name: str,
    date: str = Query(
        ..., 
        description="Date in dd-mm-yyyy format",
        example="05-11-2025"
    ),
    time: str = Query(
        ..., 
        description="Time in 24-hour HH:MM:SS format",
        example="14:30:00"
    )
):
    """
    Get heliocentric position of a specific planet
    
    Parameters:
    - planet_name: Name of the planet (mercury, venus, earth, mars, jupiter, saturn, uranus, neptune)
    - date: Date in dd-mm-yyyy format
    - time: Time in 24-hour HH:MM:SS format
    
    Returns:
    - Dictionary with planet position in metres
    """
    planet_name_lower = planet_name.lower()
    
    if planet_name_lower not in PLANETS:
        raise HTTPException(
            status_code=404,
            detail=f"Planet '{planet_name}' not found. Available planets: {', '.join(PLANETS.keys())}"
        )
    
    try:
        # Parse date and time
        day, month, year = map(int, date.split('-'))
        hour, minute, second = map(int, time.split(':'))
        
        # Create time object
        t = ts.utc(year, month, day, hour, minute, second)
        
        # Get the Sun and planet
        sun = eph['sun']
        planet = eph[PLANETS[planet_name_lower]]
        
        # Calculate heliocentric position
        heliocentric = sun.at(t).observe(planet)
        
        # Get x, y, z in AU and convert to metres
        x_au, y_au, z_au = heliocentric.position.au
        
        response = {
            "status": "success",
            "planet": planet_name_lower,
            "date": date,
            "time": time,
            "timestamp_utc": f"{year}-{month:02d}-{day:02d}T{hour:02d}:{minute:02d}:{second:02d}Z",
            "units": "metres",
            "reference_frame": "heliocentric",
            "coordinate_system": "ICRF",
            "position": {
                'x': float(x_au * AU_TO_METRES),
                'y': float(y_au * AU_TO_METRES),
                'z': float(z_au * AU_TO_METRES)
            }
        }
        
        return JSONResponse(content=response)
        
    except ValueError as e:
        raise HTTPException(
            status_code=400,
            detail=f"Invalid date or time format: {str(e)}"
        )
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Error calculating position: {str(e)}"
        )


if __name__ == "__main__":
    # Run the server
    print("\n" + "="*70)
    print("Starting Planet Position API Server")
    print("="*70)
    print("\nEndpoints:")
    print("  - Health Check: http://localhost:8000/health")
    print("  - Get All Coords: http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00")
    print("  - Get Single Planet: http://localhost:8000/get_coords/earth?date=05-11-2025&time=14:30:00")
    print("  - API Docs: http://localhost:8000/docs")
    print("="*70 + "\n")
    
    uvicorn.run(app, host="0.0.0.0", port=8000)

