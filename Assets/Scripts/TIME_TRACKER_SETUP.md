# Simulation Time Tracker - Setup Guide

This script tracks simulation time based on Earth's orbital position around the Sun.

## ✅ Features

- **Scale-Independent**: Works with any G value or distance scaling
- **Physically Accurate**: Based on Earth's actual orbital progress
- **Auto-Detection**: Automatically finds Earth and Sun GameObjects
- **Real-Time Display**: Shows current simulation date/time on screen
- **Complete API**: Access time data from other scripts

---

## 🚀 Quick Setup

### 1. Add to Your Scene

1. Create an empty GameObject in your scene
2. Name it: `TimeTracker` or `SimulationManager`
3. Add the `SimulationTimeTracker` component

### 2. Configure in Inspector

The script should auto-detect Earth and Sun, but you can verify:

#### **References Section:**
- **Earth Transform**: Should auto-find (looks for GameObject with "earth" in name)
- **Sun Transform**: Should auto-find (looks for "Sun Sphere" or "Sun")

If auto-detection fails, drag your Earth and Sun GameObjects here manually.

#### **Starting Date/Time Section:**
- **Start Date**: `05-11-2025` (dd-mm-yyyy format)
- **Start Time**: `20:00:00` (HH:MM:SS format)
- These should match your `PlanetPositionInitializer` settings!

#### **Settings Section:**
- **Orbital Plane Normal**: `(0, 1, 0)` = Y-axis (for horizontal orbits)
  - If your planets orbit in a different plane, adjust this
  - Z-axis orbits: use `(0, 0, 1)`
- **Initialization Delay**: `2.5` seconds (wait for positions to load)

---

## 📊 What You'll See

### On-Screen Display (Top-Left)

When you press Play, you'll see:

```
Simulation Time
Date: 05-11-2025
Time: 20:00:00
Days Elapsed: 0.0
Years Elapsed: 0.00
Orbits: 0
Orbital Progress: 0.0%
```

As Earth orbits:
- **Date** updates based on Earth's position
- **Days Elapsed** shows simulation days passed
- **Years Elapsed** shows years (365.25 days = 1 year)
- **Orbits** shows complete orbits around Sun
- **Orbital Progress** shows % of current orbit (0-100%)

### Console Messages

You'll see debug messages:
```
Time Tracker: Found Earth - Earth
Time Tracker: Found Sun - Sun Sphere
Time Tracker: Starting from 05-11-2025 20:00:00
Time Tracker: Initialized! Tracking Earth's orbit around Sun.
```

---

## 🔧 How It Works

### The Math

1. **Tracks Earth's Direction** from Sun (normalized vector)
2. **Calculates Angular Movement** each frame
3. **Converts Angle to Time**:
   - 360° = 365.25 days (1 Earth year)
   - 180° = ~182.6 days (half year)
   - 1° = ~1.01 days

4. **Updates Date**: Adds elapsed days to starting date

### Why This Works

- **Independent of Scale**: Only tracks angles, not distances
- **Independent of G Value**: Doesn't matter how fast planets move
- **Self-Correcting**: If Earth takes correct time to orbit, date will be accurate

---

## 💻 Using in Your Own Scripts

### Access Time Data

```csharp
public class MyScript : MonoBehaviour
{
    private SimulationTimeTracker timeTracker;
    
    void Start()
    {
        timeTracker = FindFirstObjectByType<SimulationTimeTracker>();
    }
    
    void Update()
    {
        if (timeTracker != null)
        {
            // Get current simulation date
            string date = timeTracker.GetCurrentDate();
            
            // Get days elapsed
            float days = timeTracker.GetDaysElapsed();
            
            // Get orbital progress (0 to 1)
            float progress = timeTracker.GetOrbitalProgress();
            
            // Get DateTime object for complex calculations
            System.DateTime current = timeTracker.GetDateTime();
            
            // Check how many complete orbits
            int orbits = timeTracker.GetCompleteOrbits();
        }
    }
}
```

### Available Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetDaysElapsed()` | float | Days since start |
| `GetYearsElapsed()` | float | Years since start |
| `GetCurrentDate()` | string | Date (dd-MM-yyyy) |
| `GetCurrentDateTime()` | string | Date & Time (dd-MM-yyyy HH:mm:ss) |
| `GetOrbitalProgress()` | float | Current orbit % (0.0 to 1.0) |
| `GetTotalAngleRotated()` | float | Total degrees rotated |
| `GetCompleteOrbits()` | int | Number of complete orbits |
| `GetDateTime()` | DateTime | Full DateTime object |
| `ResetTracking()` | void | Reset time to start |

---

## 🎨 Customizing the Display

### Hide On-Screen Display

If you want to hide the GUI and use your own UI:

Comment out the `OnGUI()` method in the script (lines ~210-230), or create your own UI:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplayUI : MonoBehaviour
{
    [SerializeField] private Text dateText;
    [SerializeField] private Text timeText;
    private SimulationTimeTracker timeTracker;
    
    void Start()
    {
        timeTracker = FindFirstObjectByType<SimulationTimeTracker>();
    }
    
    void Update()
    {
        if (timeTracker != null)
        {
            dateText.text = $"Date: {timeTracker.GetCurrentDate()}";
            timeText.text = $"Days: {timeTracker.GetDaysElapsed():F1}";
        }
    }
}
```

---

## 🐛 Troubleshooting

### Issue: "Could not find Earth!"
**Solution:**
- Make sure your Earth GameObject name contains "earth" (case-insensitive)
- Or manually assign Earth in Inspector

### Issue: "Could not find Sun!"
**Solution:**
- Make sure Sun is named "Sun Sphere" or "Sun"
- Or manually assign Sun in Inspector

### Issue: Time not updating
**Solution:**
- Check that Earth is actually moving (has velocity)
- Verify "Initialization Delay" gave enough time for positions to load
- Check Console for initialization messages

### Issue: Time moving too fast/slow
**Solution:**
- This is normal! Time speed depends on your G value
- If G = 10 (default), simulation time moves faster than real time
- The tracker accurately reflects Earth's orbital position

### Issue: Wrong orbital plane
**Solution:**
- Adjust "Orbital Plane Normal" in Inspector
- Try different axes: (0,1,0) for Y-up, (0,0,1) for Z-up

---

## 📈 Verification

To verify accuracy, you can check other planets:

| Planet | Orbital Period | Expected Days |
|--------|---------------|---------------|
| Mercury | 88 days | ~0.24 years |
| Venus | 225 days | ~0.62 years |
| Earth | 365.25 days | 1.00 year |
| Mars | 687 days | ~1.88 years |
| Jupiter | 4,333 days | ~11.86 years |

If your simulation is accurate, these ratios should match!

---

## 🎯 Tips

1. **Match PlanetPositionInitializer**: Use the same start date/time
2. **Wait for Initialization**: The 2.5s delay ensures positions are set
3. **Adjust G for Speed**: Higher G = faster orbits = faster simulation time
4. **Record Data**: Use the API methods to log time data for analysis

---

## 📝 Summary Checklist

- [ ] TimeTracker GameObject exists in scene
- [ ] SimulationTimeTracker component attached
- [ ] Earth and Sun auto-detected (check Console)
- [ ] Start date matches PlanetPositionInitializer
- [ ] Initialization delay set appropriately (2.5s default)
- [ ] Orbital plane normal set correctly (Y-up by default)
- [ ] On-screen display shows time updating

---

## 🌟 Next Steps

Once working:
1. Create custom UI for time display
2. Add time controls (pause, speed up, reset)
3. Log time data for analysis
4. Compare with real ephemeris data for accuracy
5. Add events triggered at specific dates

Enjoy tracking your solar system simulation! 🚀

