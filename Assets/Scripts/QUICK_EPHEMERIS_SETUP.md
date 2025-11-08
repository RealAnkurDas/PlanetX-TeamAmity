# Quick Setup: Ephemeris-Based Simulation

## 🎯 **How It Works:**

### **Planets:**
- Positions calculated from **Astronomy Engine** ephemeris
- Updated **every frame** based on simulation time
- **Always accurate** - no drift
- Don't use Unity physics

### **Spacecraft/Asteroids** (future):
- Tag as **"SmallBody"**
- Use **GravitySimulator** - feels gravity from all planets
- Full N-body physics simulation

---

## ⚡ **Quick Setup (3 Steps):**

### **1. Create SimulationManager**
```
Hierarchy → Create Empty → "SimulationManager"
Add Component → EphemerisBasedSimulation

Settings:
  ☑ Use Current DateTime
  Time Step Multiplier: 86400  (1 day per second)
  Scale To Unity Units: 1E-09
```

### **2. Add Time Display**
```
Same GameObject or new one
Add Component → SimulationTimeTrackerNew

Settings:
  Simulation: [drag SimulationManager]
  ☑ Show Display
```

### **3. Disable Old Scripts**
```
Find and UNCHECK (don't delete):
  - Orbit (SolarSystem.cs)
  - PlanetPositionInitializer
  - SimulationTimeTracker  
  - TimeSpeedController
```

**Press Play!** ✅

---

## 🎮 **Speed Control:**

### **Changing Speed:**

**Method 1: Inspector (while playing)**
- Select SimulationManager
- Change "Time Step Multiplier" value
- Changes take effect immediately

**Method 2: Speed Button (optional)**
- Create GameObject → Add TimeStepSpeedController
- Assign simulation and button
- Click button to cycle through presets

### **Speed Presets:**

| Multiplier | Meaning | Earth Orbit Time |
|------------|---------|------------------|
| 1 | Real-time (1 sec = 1 sec) | 365 days |
| 86400 | 1 day/sec | 6 minutes |
| 604800 | 1 week/sec | 52 seconds |
| 2592000 | 1 month/sec | 12 seconds |
| 31536000 | 1 year/sec | 1 second |

---

## 📊 **What You'll See:**

### **Bottom Center:**
```
Date: 05-11-2025
Time: 20:00:00
```

Advances based on timestep!

### **Planets:**
- Move smoothly along ephemeris-accurate paths
- No jitter or drift
- Always in correct positions

---

## 🛸 **Adding Spacecraft (Later):**

### **Simple 3-Step:**
1. Create spacecraft GameObject
2. Tag as **"SmallBody"** (new tag!)
3. Add Rigidbody with initial velocity

**That's it!** Spacecraft automatically:
- Feels gravity from Sun + all 8 planets
- Uses accurate N-body simulation
- Follows realistic trajectories

---

## 🔧 **Common Adjustments:**

### **Too Fast?**
Reduce Time Step Multiplier:
- 86400 → 43200 (12 hours per second)
- 86400 → 3600 (1 hour per second)

### **Too Slow?**
Increase Time Step Multiplier:
- 86400 → 172800 (2 days per second)
- 86400 → 2592000 (30 days per second)

### **Jittery Motion?**
- Reduce timestep
- Check Unity's Fixed Timestep (Edit → Project Settings → Time)
- Recommended: 0.02 (50 fps)

---

## ✅ **Quick Checklist:**

- [ ] astronomy.cs in project
- [ ] EphemerisBasedSimulation.cs created
- [ ] SimulationManager GameObject created
- [ ] Component added and configured
- [ ] Old scripts disabled
- [ ] Planets have "Celestial" tag
- [ ] Press Play - planets move! ✓
- [ ] Date/time advances ✓

---

## 🎯 **Key Differences:**

| Feature | Old (Gravity) | New (Ephemeris) |
|---------|--------------|-----------------|
| **Planet motion** | Physics simulation | Ephemeris lookup |
| **Accuracy** | Drifts over time | Always perfect |
| **Speed control** | Scale G | Scale timestep |
| **Spacecraft** | Same as planets | Separate GravitySimulator |
| **Performance** | Heavy physics | Light calculations |

---

## 💡 **Pro Tips:**

1. **Start Simple**: Use default 86400 timestep (1 day/sec)
2. **Test Accuracy**: Compare planet positions to real data
3. **Add Spacecraft**: Tag as "SmallBody" when ready
4. **Keep Old Scripts**: Don't delete, just disable (easy to revert)

---

**Your simulation is now astronomically accurate and properly time-based!** 🌍🪐✨

