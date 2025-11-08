# Unity + Python API Setup Guide

This guide explains how to set up your Unity scene to fetch planet positions from the Python API.

## ✅ What's Been Set Up

### Python API Server
- **Location**: `Assets/Scripts/python/`
- **Main File**: `api_server.py`
- **Endpoints**:
  - Health: `http://localhost:8000/health`
  - Get Coordinates: `http://localhost:8000/get_coords?date=05-11-2025&time=20:00:00`

### Unity Scripts
1. **PlanetPositionInitializer.cs** - Fetches positions from API and applies them to planets
2. **SolarSystem.cs (Modified)** - Waits for positions to load before calculating velocities

## 🚀 Quick Setup Steps

### 1. Start the Python API Server

Open a terminal/command prompt:

```bash
cd "C:\Users\ankur\Planet X - AR\Assets\Scripts\python"
python api_server.py
```

You should see:
```
Starting Planet Position API Server
======================================================================
Endpoints:
  - Health Check: http://localhost:8000/health
  - Get All Coords: http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00
  ...
```

**Test it's working:**
Open browser to: http://localhost:8000/health

### 2. Unity Scene Setup

#### A. Create Position Manager GameObject
1. In Unity Hierarchy, right-click → Create Empty
2. Name it: `PlanetPositionManager`
3. Add Component → `PlanetPositionInitializer`
4. Configure in Inspector:
   - **API Base URL**: `http://localhost:8000`
   - **Date**: `05-11-2025` (dd-mm-yyyy format)
   - **Time**: `20:00:00` (24-hour HH:MM:SS format)
   - **Scale To Unity Units**: `1E-09` (divides by 10^9)

#### B. Configure Your Planets
Make sure each planet GameObject:
1. Has the **"Celestial"** tag
2. Has a name containing the planet name:
   - ✅ "Mercury", "Venus", "Earth", "Mars", etc.
   - ✅ "Earth_Planet", "mars_obj", etc. (case-insensitive, can have prefixes/suffixes)
3. Has a Rigidbody component (for orbital physics)

#### C. Configure the Sun
1. Name the Sun GameObject: **"Sun"** (exact name)
2. It will be automatically positioned at (0, 0, 0) as the heliocentric center

#### D. Configure Orbit Script
1. Find the GameObject with the `Orbit` script
2. In Inspector, you'll see:
   - **Is Elliptical Orbit**: Choose circular or elliptical
   - **Wait For Positions Delay**: Default 1.5 seconds (adjust if API is slow)

### 3. Test the Setup

#### Test API Connection:
1. Make sure Python server is running
2. Press Play in Unity
3. Check Unity Console for messages:
   ```
   Fetching planet positions from API...
   Set Earth position to (x, y, z) (scaled from API)
   ...
   Successfully loaded positions for 8 planets at 05-11-2025 20:00:00
   Orbit: Waiting 1.5 seconds for planet positions to load from API...
   ```

#### Expected Behavior:
1. Scene starts
2. PlanetPositionInitializer fetches positions from API (runs in Awake)
3. Planets move to their API positions
4. After 1.5 seconds, Orbit script calculates and applies initial velocities
5. Planets begin orbiting with physics

## 📊 Understanding the Scale

### Distance Conversion:
- **API Returns**: Positions in metres
- **Unity Uses**: 1 unit = 1 billion metres (10^9 m)
- **Conversion**: Divide API values by 10^9

### Examples:
| Distance | Metres | Unity Units |
|----------|--------|-------------|
| Earth orbit | 1.496 × 10^11 m | 149.6 units |
| Mars orbit | 2.279 × 10^11 m | 227.9 units |
| Jupiter orbit | 7.785 × 10^11 m | 778.5 units |

## 🔧 Coordinate System

### API (ICRF - Astronomical):
- X: Points toward vernal equinox
- Y: 90° east along celestial equator
- Z: Points to North Celestial Pole (up)

### Unity Conversion:
```csharp
Unity.X = API.X
Unity.Y = API.Z  // Z becomes Y (Unity's up axis)
Unity.Z = API.Y  // Y becomes Z (Unity's forward axis)
```

**Note**: If your scene has a different orientation, you may need to adjust the axis mapping in `ConvertPosition()` method.

## 🐛 Troubleshooting

### Issue: "Failed to fetch planet positions"
**Solution**: 
- Make sure Python API server is running
- Test URL in browser: http://localhost:8000/health
- Check firewall isn't blocking port 8000

### Issue: "Failed to parse API response"
**Solution**:
- Check Console for the actual response
- Verify date/time format in PlanetPositionInitializer
- Test API directly in browser

### Issue: Planets not moving to correct positions
**Solution**:
- Check planet GameObject names contain planet names
- Verify "Celestial" tag is applied
- Check Console for "Set [Planet] position to..." messages

### Issue: Planets spawn but velocities are wrong
**Solution**:
- Increase `waitForPositionsDelay` in Orbit script (try 2-3 seconds)
- Check that positionsLoaded becomes true in Inspector

### Issue: Coordinate system looks wrong
**Solution**:
- Adjust axis mapping in `ConvertPosition()` method
- Try different combinations:
  ```csharp
  // Option 1 (current):
  new Vector3(x, z, y);
  
  // Option 2:
  new Vector3(x, y, z);
  
  // Option 3:
  new Vector3(-x, z, -y);
  ```

## 🎮 Changing Date/Time at Runtime

### Method 1: Inspector (Before Play)
1. Select `PlanetPositionManager` GameObject
2. Change Date/Time fields in Inspector
3. Press Play

### Method 2: Script (Optional Enhancement)
You could add a method to reload positions:

```csharp
public void UpdateDateTime(string newDate, string newTime)
{
    date = newDate;
    time = newTime;
    StartCoroutine(InitializePlanetPositions());
}
```

## 📝 Summary Checklist

Before pressing Play:

- [ ] Python API server is running (`python api_server.py`)
- [ ] Test API in browser: http://localhost:8000/health
- [ ] PlanetPositionManager GameObject exists with script attached
- [ ] All planets have "Celestial" tag
- [ ] Planet names contain their planet name (mercury, venus, earth, etc.)
- [ ] Sun is named "Sun"
- [ ] Date and Time are configured in Inspector
- [ ] Orbit script has appropriate delay (1.5s default)

## 🎯 Next Steps

Once working:
1. Experiment with different dates/times
2. Adjust scale if planets are too close/far
3. Fine-tune orbital velocity calculations
4. Add visualization for orbital paths
5. Create UI to change date/time dynamically

## 📚 Files Reference

- **Python API**: `Assets/Scripts/python/api_server.py`
- **API Docs**: `Assets/Scripts/python/API_README.md`
- **Unity Initializer**: `Assets/Scripts/PlanetPositionInitializer.cs`
- **Orbit Script**: `Assets/Scripts/SolarSystem.cs`
- **API Tests**: `Assets/Scripts/python/test_api.py`

## 🌟 API Date Range

The JPL DE421 ephemeris covers:
- **Start**: 1900-07-28
- **End**: 2053-10-08

Dates outside this range will return errors.

