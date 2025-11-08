# Planet Position API Documentation

A FastAPI server that provides heliocentric positions of solar system planets in metres.

## Installation

```bash
cd python
pip install -r requirements.txt
```

## Starting the Server

```bash
python api_server.py
```

Or using uvicorn directly:

```bash
uvicorn api_server:app --reload --host 0.0.0.0 --port 8000
```

The server will start on `http://localhost:8000`

## API Endpoints

### 1. Root Endpoint

**GET** `/`

Returns API information and available endpoints.

**Example:**
```bash
curl http://localhost:8000/
```

**Response:**
```json
{
  "message": "Planet Position API",
  "version": "1.0.0",
  "endpoints": {
    "health": "/health",
    "coordinates": "/get_coords"
  }
}
```

---

### 2. Health Check

**GET** `/health`

Check if the API is running and get service information.

**Example:**
```bash
curl http://localhost:8000/health
```

**Response:**
```json
{
  "status": "healthy",
  "service": "Planet Position API",
  "ephemeris": "JPL DE421",
  "ephemeris_range": "1900-07-28 to 2053-10-08"
}
```

---

### 3. Get All Planet Coordinates

**GET** `/get_coords`

Get heliocentric positions of all planets for a specific date and time.

**Query Parameters:**
- `date` (required): Date in `dd-mm-yyyy` format
- `time` (required): Time in 24-hour `HH:MM:SS` format

**Example:**
```bash
curl "http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00"
```

**Response:**
```json
{
  "status": "success",
  "date": "05-11-2025",
  "time": "14:30:00",
  "timestamp_utc": "2025-11-05T14:30:00Z",
  "units": "metres",
  "reference_frame": "heliocentric",
  "coordinate_system": "ICRF",
  "planets": {
    "mercury": {
      "x": 51700000000.0,
      "y": -18400000000.0,
      "z": -8500000000.0
    },
    "venus": {
      "x": 106600000000.0,
      "y": 3500000000.0,
      "z": -5200000000.0
    },
    "earth": {
      "x": -24400000000.0,
      "y": 132800000000.0,
      "z": 57500000000.0
    },
    "mars": {
      "x": 184700000000.0,
      "y": -85000000000.0,
      "z": -35100000000.0
    },
    "jupiter": {
      "x": 683300000000.0,
      "y": 351000000000.0,
      "z": 18500000000.0
    },
    "saturn": {
      "x": -1331000000000.0,
      "y": 849400000000.0,
      "z": 35100000000.0
    },
    "uranus": {
      "x": 2711000000000.0,
      "y": 1015000000000.0,
      "z": 51700000000.0
    },
    "neptune": {
      "x": 4467000000000.0,
      "y": -517100000000.0,
      "z": -68300000000.0
    }
  }
}
```

---

### 4. Get Single Planet Coordinates

**GET** `/get_coords/{planet_name}`

Get heliocentric position of a specific planet.

**Path Parameters:**
- `planet_name`: Planet name (mercury, venus, earth, mars, jupiter, saturn, uranus, neptune)

**Query Parameters:**
- `date` (required): Date in `dd-mm-yyyy` format
- `time` (required): Time in 24-hour `HH:MM:SS` format

**Example:**
```bash
curl "http://localhost:8000/get_coords/earth?date=05-11-2025&time=14:30:00"
```

**Response:**
```json
{
  "status": "success",
  "planet": "earth",
  "date": "05-11-2025",
  "time": "14:30:00",
  "timestamp_utc": "2025-11-05T14:30:00Z",
  "units": "metres",
  "reference_frame": "heliocentric",
  "coordinate_system": "ICRF",
  "position": {
    "x": -24400000000.0,
    "y": 132800000000.0,
    "z": 57500000000.0
  }
}
```

---

## Interactive API Documentation

FastAPI provides automatic interactive API documentation:

- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc

These interfaces allow you to:
- View all endpoints
- See parameter descriptions
- Test API calls directly in the browser
- View response schemas

---

## Error Responses

### 400 Bad Request
Returned when date/time format is invalid or values are out of range.

```json
{
  "detail": "Invalid date or time format: Day must be between 1 and 31"
}
```

### 404 Not Found
Returned when requesting an invalid planet name.

```json
{
  "detail": "Planet 'pluto' not found. Available planets: mercury, venus, earth, mars, jupiter, saturn, uranus, neptune"
}
```

### 500 Internal Server Error
Returned when an unexpected error occurs during calculation.

```json
{
  "detail": "Error calculating positions: [error message]"
}
```

---

## Technical Details

### Coordinate System
- **Reference Frame**: Heliocentric (Sun-centered)
- **Coordinate System**: ICRF (International Celestial Reference Frame)
- **Units**: Metres
- **Conversion**: 1 AU = 149,597,870,700 metres

### Date Range
- **Valid Range**: 1900-07-28 to 2053-10-08
- **Ephemeris**: JPL DE421

### Planets Included
1. Mercury
2. Venus
3. Earth
4. Mars
5. Jupiter
6. Saturn
7. Uranus
8. Neptune

**Note**: Pluto is not included as it's classified as a dwarf planet.

---

## Usage Examples

### Python
```python
import requests

# Get all planet positions
response = requests.get(
    "http://localhost:8000/get_coords",
    params={
        "date": "05-11-2025",
        "time": "14:30:00"
    }
)
data = response.json()
earth_x = data['planets']['earth']['x']
```

### JavaScript (Node.js/Browser)
```javascript
fetch('http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00')
  .then(response => response.json())
  .then(data => {
    console.log(data.planets.earth);
  });
```

### Unity C# (using UnityWebRequest)
```csharp
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

IEnumerator GetPlanetPositions()
{
    string url = "http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00";
    
    using (UnityWebRequest request = UnityWebRequest.Get(url))
    {
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            // Parse JSON and use positions
            Debug.Log(json);
        }
    }
}
```

---

## Performance Considerations

- The ephemeris file (~17 MB) is loaded once at server startup
- Subsequent requests are fast as they only perform calculations
- For production, consider:
  - Caching frequently requested date/times
  - Using async processing for bulk requests
  - Setting up proper CORS headers for web access
  - Adding rate limiting

---

## Adding CORS Support

For web applications, add CORS middleware:

```python
from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Adjust for production
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
```

---

## Deployment

### Local Development
```bash
python api_server.py
```

### Production with Gunicorn
```bash
pip install gunicorn
gunicorn api_server:app -w 4 -k uvicorn.workers.UvicornWorker --bind 0.0.0.0:8000
```

### Docker
```dockerfile
FROM python:3.9-slim

WORKDIR /app
COPY requirements.txt .
RUN pip install -r requirements.txt

COPY api_server.py .

CMD ["uvicorn", "api_server:app", "--host", "0.0.0.0", "--port", "8000"]
```

---

## Troubleshooting

### Server won't start
- Check if port 8000 is already in use
- Try a different port: `uvicorn api_server:app --port 8080`

### Ephemeris download fails
- Manually download from: https://naif.jpl.nasa.gov/pub/naif/generic_kernels/spk/planets/de421.bsp
- Place in Skyfield's data directory: `~/.skyfield-data/` (Linux/Mac) or `%USERPROFILE%\.skyfield-data\` (Windows)

### Date out of range errors
- Ensure dates are between 1900-07-28 and 2053-10-08
- For other date ranges, use a different ephemeris file

