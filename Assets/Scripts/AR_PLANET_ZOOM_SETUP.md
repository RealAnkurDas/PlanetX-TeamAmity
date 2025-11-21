# 🎯 AR Planet Zoom Setup

Complete setup for zooming to selected planets in AR mode with floating solar system.

---

## ✅ What You Get

1. **Solar system floats above floor** (not placed on it)
2. **Zoom buttons work on selected planet** (from dropdown)
3. **Select Sun → zoom in/out on Sun**
4. **Select Mars → zoom in/out on Mars**
5. **Default target is Sun**

---

## 🚀 Quick Setup

### Step 1: Add AR Planet Zoom Component

1. **In Hierarchy**, find or create "AR Manager" GameObject (same one with ARSolarSystemManager)

2. **Add Component:**
   - Select "AR Manager"
   - Add Component → **AR Planet Zoom**

3. **Configure (it should auto-find, but verify):**
   - **AR Manager**: Should auto-find ARSolarSystemManager
   - **Planet Selector**: Should auto-find PlanetSelector script
   - **Target Zoom Distance**: `2` (default viewing distance)
   - **Zoom Step**: `1.3` (30% change per click)
   - **Min Zoom Multiplier**: `0.5` (closest)
   - **Max Zoom Multiplier**: `5` (farthest)

---

### Step 2: Update AR Solar System Manager

1. **Select "AR Manager"** (or GameObject with ARSolarSystemManager)

2. **In Inspector**, find **AR Solar System Manager** component

3. **Set this new field:**
   - **Height Above Floor**: `1.5` (meters above floor to float)
   
   Adjust this value:
   - `0.5` = Low hover
   - `1.5` = Good height (default)
   - `3.0` = High hover

---

### Step 3: Update AR Scale UI Controller

1. **Find "AR Scale UI Controller"** in Hierarchy (the one with ARScaleUI script)

2. **In Inspector**, find **AR Scale UI** component

3. **Add Planet Zoom reference:**
   - **Planet Zoom**: Drag the "AR Manager" GameObject (or GameObject with ARPlanetZoom)
   
   ✅ This makes the +/- buttons zoom to selected planet!

---

## 🎮 How It Works

### Planet Selection + Zoom:

1. **Open dropdown** → Select "Mars"
2. **Tap "+"** → Zooms IN on Mars (gets closer)
3. **Tap "-"** → Zooms OUT from Mars (gets farther)
4. **Select "Jupiter"** → Now +/- zooms on Jupiter
5. **Default** → Sun is selected, so +/- zooms on Sun

### Zoom Multipliers:

| Zoom Level | Multiplier | What You See |
|------------|------------|--------------|
| Closest | 0.5x | Very close to planet |
| Normal | 1.0x | Default viewing distance |
| Far | 2.0x | Twice as far |
| Very Far | 5.0x | Maximum distance |

### How Floating Works:

- Solar system placed horizontally in front of camera
- Then lifted up by `heightAboveFloor` amount
- Result: Floats above floor instead of sitting on it

---

## 🎯 Test in Unity Editor

1. **Press Play**
2. **Open dropdown** → Select different planets
3. **Click +/- buttons** → Console shows "Zoomed IN on selected planet"
4. **Top-left text** → Shows "AR Zoom: 1.00x"

---

## 📱 Test on Device

1. **Build to device**
2. **Point at floor** → AR detects surface
3. **Solar system appears floating above floor** 🌟
4. **Open dropdown** → Select planet
5. **Tap +** → Solar system moves so selected planet gets closer
6. **Tap -** → Selected planet gets farther
7. **Walk around** → See from different angles!

---

## 🔧 Configuration

### Adjust Float Height:

**AR Solar System Manager → Height Above Floor**
- Higher value = More floating
- Lower value = Closer to floor

### Adjust Zoom Sensitivity:

**AR Planet Zoom → Zoom Step**
- `1.3` = 30% change (default)
- `1.5` = 50% change (more dramatic)
- `1.2` = 20% change (subtle)

### Adjust Zoom Range:

**AR Planet Zoom**
- **Min Zoom Multiplier**: How close you can get (`0.5` = half distance)
- **Max Zoom Multiplier**: How far you can go (`5` = 5x distance)

---

## 🐛 Troubleshooting

### Problem: Solar system still on floor

**Solution:**
1. Check AR Solar System Manager has **Height Above Floor** set (1.5)
2. Check XR Origin Scale (should be ~1000)
3. Higher scale needs higher height: `heightAboveFloor * xrOriginScale`

### Problem: Zoom buttons don't work on selected planet

**Solution:**
1. Check ARScaleUI has **Planet Zoom** reference assigned
2. Check console for "ARScaleUI: Found AR Planet Zoom"
3. Check ARPlanetZoom has **Planet Selector** reference

### Problem: Can't find planet when selecting from dropdown

**Solution:**
1. Check planet names in dropdown match GameObject names exactly
2. Check console for "Could not find planet 'X'"
3. Check ARPlanetZoom's `GetPlanetZoomDistance()` has your planet name

### Problem: Zoom is too slow/fast

**Solution:**
- Adjust **AR Planet Zoom → Zoom Step**:
  - Slower: `1.2` (20% change)
  - Faster: `1.5` (50% change)

### Problem: Zoom is too jerky/instant

**Solution:**
- Adjust **AR Planet Zoom → Smooth Speed**:
  - Smoother: `3` (slower transition)
  - Faster: `10` (quicker transition)
  - Default: `5`

---

## 📊 Example Usage

### Scenario: Exploring the Solar System

1. **Launch AR** → Solar system floats in room
2. **Select Sun** → Default
3. **Tap +** → Zoom close to Sun, see solar flares
4. **Select Earth** → Switches to Earth
5. **Tap -** → Zoom out to see Moon orbit
6. **Walk around** → See Earth from all angles
7. **Select Jupiter** → Switch to gas giant
8. **Tap +** → Get close to see bands

---

## ✅ Checklist

- [ ] AR Planet Zoom component added to AR Manager
- [ ] ARSolarSystemManager has Height Above Floor set
- [ ] ARScaleUI has Planet Zoom reference assigned
- [ ] Planet Selector working (dropdown shows planets)
- [ ] Zoom buttons connected (+, -)
- [ ] Tested in editor (console shows messages)
- [ ] Built to device and tested
- [ ] Solar system floats above floor ✓
- [ ] Zoom works on selected planet ✓

---

## 🎨 Visual Example

```
Device View:
┌─────────────────────────────┐
│  ▶ Sun    [Dropdown]        │  ← Select planet
│  AR Zoom: 1.00x             │  ← Current zoom
│                             │
│                        [+]  │  ← Zoom In
│                        [-]  │  ← Zoom Out
│                             │
│        🌟 Solar System      │  ← Floating above floor
│       (Selected: Sun)       │
│      ___________            │  ← Floor plane (detected)
│                             │
└─────────────────────────────┘
```

---

## 🚀 Advanced: Auto-Zoom on Select

Want solar system to automatically zoom when selecting from dropdown?

**Option 1: Call from PlanetSelector**

Modify `PlanetSelector.cs` to call ARPlanetZoom when planet is selected.

**Option 2: Listen for Selection**

Create script that watches for `currentAnchor` changes in PlanetSelector.

Let me know if you want this feature!

---

**All set!** Your AR solar system should now float above the floor and zoom to selected planets. 🌌✨

