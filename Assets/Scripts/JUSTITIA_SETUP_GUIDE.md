# Justitia Asteroid Setup Guide

Quick guide to add Justitia with JPL ephemeris data.

---

## 🚀 **Setup Steps (3 Steps):**

### **Step 1: Create Justitia Ephemeris Reader**

1. Create Empty GameObject → Name: **`JustitiaEphemeris`**
2. **Add Component** → `AsteroidEphemerisReader`
3. In Inspector:
   - **Ephemeris File**: Drag `justitia.txt` from `asteroid_ephemeris` folder
   - **Asteroid Name**: Type `Justitia` (must match GameObject name exactly)

---

### **Step 2: Connect to Simulation**

1. Select **`SimulationManager`** (GameObject with EphemerisBasedSimulation)
2. In Inspector, find **"Asteroids"** section
3. **Asteroid Readers**:
   - Change **Size** from `0` to `1`
   - **Element 0**: Drag `JustitiaEphemeris` GameObject here

---

### **Step 3: Verify Justitia GameObject Exists**

1. Make sure you have a GameObject named exactly **`Justitia`** in scene
2. It should be a sphere or 3D model
3. It will be positioned automatically from the .txt file

---

## ✅ **Inspector Layout:**

### **JustitiaEphemeris GameObject:**
```
AsteroidEphemerisReader (Script)
├─ Ephemeris File
│  └─ [justitia.txt]
├─ Asteroid Info
│  └─ Asteroid Name: Justitia
└─ Status (shows when playing)
   ├─ Data Points Loaded: 4383
   ├─ Date Range Start: 2025-11-05
   └─ Date Range End: 2037-12-05
```

### **SimulationManager:**
```
EphemerisBasedSimulation (Script)
...
├─ Asteroids
│  └─ Asteroid Readers
│     ├─ Size: 1
│     └─ Element 0: JustitiaEphemeris
...
```

---

## 🎮 **Testing:**

1. **Press Play**
2. Check Console for:
   ```
   AsteroidEphemeris (Justitia): Parsing ephemeris file...
   AsteroidEphemeris (Justitia): Loaded 4383 data points
     Date range: 2025-11-05 to 2037-12-05
   ```
3. Justitia should appear and move with time!
4. Use dropdown to select Justitia and zoom to it

---

## 🐛 **Troubleshooting:**

### Issue: "No ephemeris file assigned!"
**Solution:**
- Make sure justitia.txt is in project
- Drag it to Ephemeris File slot in Inspector

### Issue: "No data points found in file!"
**Solution:**
- Check file contains $$SOE and $$EOE markers
- Verify file isn't corrupted
- Check Console for parsing errors

### Issue: Justitia doesn't move
**Solution:**
- Check "Data Points Loaded" > 0 in Inspector
- Verify GameObject is named exactly "Justitia" (case-sensitive)
- Check SimulationManager has JustitiaEphemeris in Asteroid Readers array

### Issue: Justitia position seems wrong
**Solution:**
- JPL data is in kilometers (heliocentric)
- Should be automatically converted
- Check Scale To Unity Units = 1E-09 in SimulationManager

---

## 📊 **Adding More Asteroids:**

For each additional asteroid:

1. Get JPL Horizons vector table (.txt file)
2. Create new Empty GameObject → Add AsteroidEphemerisReader
3. Assign the .txt file
4. Set asteroid name
5. Add to SimulationManager's Asteroid Readers array (increase Size)
6. Create GameObject with matching name

---

## ✅ **Quick Checklist:**

- [ ] justitia.txt file in Assets
- [ ] JustitiaEphemeris GameObject created
- [ ] AsteroidEphemerisReader component added
- [ ] justitia.txt assigned to Ephemeris File
- [ ] Asteroid Name set to "Justitia"
- [ ] SimulationManager → Asteroid Readers Size = 1
- [ ] JustitiaEphemeris dragged to Element 0
- [ ] Justitia GameObject exists in scene
- [ ] Press Play - Justitia moves! ✓

---

**Justitia will now move based on JPL ephemeris data, just like the planets!** 🪐✨

