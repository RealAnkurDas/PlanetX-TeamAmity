# All 7 Asteroids - Complete Setup Guide

Quick guide to add all 7 asteroids to your solar system.

---

## 🪐 **Your 7 Asteroids:**

1. **10253 Westerwald**
2. **623 Chimaera**
3. **13294 Rockox**
4. **59980 Moza**
5. **23871 Ousha**
6. **88055 Ghaf**
7. **269 Justitia**

---

## ✅ **Files Ready:**

All CSV files created in `asteroid_ephemeris/`:
- ✅ Westerwald_clean.csv (12,476 data points)
- ✅ Chimaera_clean.csv (12,476 data points)
- ✅ Rockox_clean.csv (12,476 data points)
- ✅ Moza_clean.csv (12,476 data points)
- ✅ Ousha_clean.csv (12,476 data points)
- ✅ Ghaf_clean.csv (12,476 data points)
- ✅ justitia_clean.csv (12,476 data points)

**Date range**: All cover 2025-Nov-05 to 2060-Jan-01

---

## 🚀 **Setup Steps (For Each Asteroid):**

### **Step 1: Create GameObject**
1. Right-click Hierarchy → **3D Object → Sphere**
2. Name it exactly: **`Westerwald`** (or Chimaera, Rockox, etc.)
3. Scale it appropriately (asteroids are small)
4. Add **Trail Renderer** component
5. Add **DelayedTrailRenderer** script
6. (Optional) Add text label as child with Billboard + ConstantSizeText

### **Step 2: Create Ephemeris Reader**
1. Right-click Hierarchy → **Create Empty**
2. Name it: **`WesterwaldEphemeris`** (or ChimaeraEphemeris, etc.)
3. Add Component → **`AsteroidEphemerisReader`**
4. In Inspector:
   - **Ephemeris File**: Drag `Westerwald_clean.csv`
   - **Asteroid Name**: Type `Westerwald` (exact match!)

### **Step 3: Add to Simulation**
1. Select **`SimulationManager`**
2. Find **"Asteroids"** → **"Asteroid Readers"**
3. Increase **Size** by 1 (was 1, now 2 for second asteroid, etc.)
4. Drag the ephemeris reader to the new element

**Repeat for all 7 asteroids!**

---

## ⚡ **Quick Batch Setup:**

### **For SimulationManager:**

After creating all ephemeris readers, set:
```
Asteroids → Asteroid Readers:
  Size: 7
  Element 0: WesterwaldEphemeris
  Element 1: ChimaeraEphemeris
  Element 2: RockoxEphemeris
  Element 3: MozaEphemeris
  Element 4: OushaEphemeris
  Element 5: GhafEphemeris
  Element 6: JustitiaEphemeris
```

---

## 📋 **Checklist Per Asteroid:**

### **Westerwald:**
- [ ] GameObject "Westerwald" created
- [ ] Sphere with Trail Renderer
- [ ] WesterwaldEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Westerwald_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Chimaera:**
- [ ] GameObject "Chimaera" created
- [ ] Sphere with Trail Renderer
- [ ] ChimaeraEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Chimaera_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Rockox:**
- [ ] GameObject "Rockox" created
- [ ] Sphere with Trail Renderer
- [ ] RockoxEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Rockox_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Moza:**
- [ ] GameObject "Moza" created
- [ ] Sphere with Trail Renderer
- [ ] MozaEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Moza_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Ousha:**
- [ ] GameObject "Ousha" created
- [ ] Sphere with Trail Renderer
- [ ] OushaEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Ousha_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Ghaf:**
- [ ] GameObject "Ghaf" created
- [ ] Sphere with Trail Renderer
- [ ] GhafEphemeris GameObject created
- [ ] AsteroidEphemerisReader added
- [ ] Ghaf_clean.csv assigned
- [ ] Added to SimulationManager array
- [ ] (Optional) Text label child

### **Justitia:**
- [x] Already done! ✓

---

## 🎨 **Recommended Asteroid Appearance:**

### **Scale (Visual Size):**
All asteroids are small (20-50 km diameter):
```
Transform → Scale: (0.001, 0.001, 0.001) or smaller
```
Adjust based on what looks good in your scene.

### **Colors (by type):**
- **Westerwald**: Gray/brown
- **Chimaera**: Reddish (C-type)
- **Rockox**: Gray (S-type)
- **Moza**: Brown
- **Ousha**: Red-brown
- **Ghaf**: Dark gray
- **Justitia**: Pink/red (unique!)

### **Trail Colors:**
Give each asteroid a distinct trail color for easy identification.

---

## 🎮 **After Setup:**

**Your dropdown will show:**
```
▶ Sun

When expanded:
▼ Sun
Mercury
Venus
Earth
Mars
Jupiter
Saturn
Uranus
Neptune
Westerwald    ← New!
Chimaera      ← New!
Rockox        ← New!
Moza          ← New!
Ousha         ← New!
Ghaf          ← New!
Justitia      ✓ Already there
```

Click any asteroid to zoom and focus on it!

---

## 📊 **Expected Console Output:**

When you press Play with all 7 asteroids set up:
```
AsteroidEphemeris (Westerwald): Loaded 12476 data points
  Date range: 2025-11-05 to 2060-01-01
AsteroidEphemeris (Chimaera): Loaded 12476 data points
  Date range: 2025-11-05 to 2060-01-01
... (all 7)
EphemerisSim: Updating 7 asteroid positions
```

---

## 💡 **Pro Tips:**

### **Tip 1: Organize in Hierarchy**
Create folders:
```
▼ Asteroids
  ├─ Westerwald
  ├─ Chimaera
  ├─ Rockox
  ├─ Moza
  ├─ Ousha
  ├─ Ghaf
  └─ Justitia
  
▼ Asteroid Ephemeris Readers
  ├─ WesterwaldEphemeris
  ├─ ChimaeraEphemeris
  ... (all 7)
```

### **Tip 2: Copy Justitia Settings**
1. Duplicate Justitia GameObject 6 times
2. Rename each duplicate
3. Change colors/materials
4. Much faster than creating from scratch!

### **Tip 3: Batch Ephemeris Readers**
1. Create one ephemeris reader
2. Duplicate it 6 times
3. Assign different CSV files
4. Update asteroid names
5. Add all to SimulationManager array

---

## ⚙️ **SimulationManager Final Settings:**

```
Asteroids:
  Asteroid Readers:
    Size: 7   ← Changed from 1
    Element 0: WesterwaldEphemeris
    Element 1: ChimaeraEphemeris
    Element 2: RockoxEphemeris
    Element 3: MozaEphemeris
    Element 4: OushaEphemeris
    Element 5: GhafEphemeris
    Element 6: JustitiaEphemeris
```

---

## 🎯 **Summary:**

**For each of the 7 asteroids:**
1. GameObject with name
2. Ephemeris reader GameObject
3. CSV file assigned
4. Added to SimulationManager array
5. Trail renderer + DelayedTrailRenderer

**Result**: All 7 asteroids moving accurately based on JPL data! 🌟

---

**Your complete solar system with all 7 asteroids is ready!** 🌍🪐✨

