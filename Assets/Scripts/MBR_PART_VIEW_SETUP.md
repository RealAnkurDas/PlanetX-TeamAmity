# 🛸 MBR Part View 3D Setup Guide

Setup for exploring MBR Explorer spacecraft parts with camera focus and info display.

---

## 🎯 What You'll Get

- 70% screen: **3D rotating view of MBR**
- 30% bottom: **Part information panel**
- Dropdown: **Select different parts to focus on**
- Camera: **Orbits selected part**, customizable zoom per part

---

## 📋 Setup Steps

### Step 1: Setup POI Spheres

1. **Find "POI" GameObject** in Hierarchy
2. **Verify it has 7 sphere children** (one for each part)
3. **Name them** (optional, helps with debugging):
   - Sphere 1 → "Solar Panels Marker"
   - Sphere 2 → "Main Engine Marker"
   - Sphere 3 → "Antenna Marker"
   - Sphere 4 → "Sensors Marker"
   - Sphere 5 → "Fuel Tank Marker"
   - Sphere 6 → "Command Module Marker"
   - Sphere 7 → "Thruster Array Marker"

4. **Position each sphere** on the corresponding part of the spacecraft

5. **Make spheres invisible** (optional):
   - Select each sphere
   - Disable **Mesh Renderer** component
   - OR set material alpha to 0
   - Spheres still work as targets, just invisible

---

### Step 2: Setup Camera

1. **Find "Main Camera"** in Hierarchy

2. **Verify TouchCameraController** is attached

3. **Configure:**
   - **Target**: Will be set by script
   - **Min Zoom Distance**: `0.5`
   - **Max Zoom Distance**: `10`
   - **Rotation Speed**: `0.5`
   - **Enable Mouse Controls**: ✅ ON (for testing)

4. **Camera Position (initial):**
   ```
   Position: (0, 2, -5)
   Rotation: (0, 0, 0)
   ```

---

### Step 3: Add Part Selector

1. **Create GameObject:**
   - Right-click Hierarchy → Create Empty
   - Rename to **"MBR Part Selector"**

2. **Add Component:**
   - Add Component → **MBR Part Selector**

3. **Configure References:**
   - **Camera Controller**: Drag Main Camera (with TouchCameraController)
   - **MBR Root**: Drag "MBR FINAL - Copy"
   - **Menu Left Offset**: `20`
   - **Menu Top Offset**: `20`

4. **Configure Parts (in Inspector):**
   - Expand **"Part Settings"** array
   - You'll see 7 parts (Solar Panels, Main Engine, etc.)
   
   For each part:
   - **Part Name**: (already set, edit if needed)
   - **Part Object**: **Drag the corresponding sphere** from POI
   - **Zoom Distance**: Set viewing distance (e.g., `2.0` for good view)
   - **Field Of View**: Leave at `-1` (uses default) or set custom
   - **Part Description**: Click to expand, type info about this part

**Example for Solar Panels:**
```
Part Name: Solar Panels
Part Object: [Drag Sphere 1]
Zoom Distance: 2.5
Field Of View: -1
Part Description: "The MBR Explorer features dual solar panel arrays 
providing 5kW of power. These panels track the sun to maximize 
energy collection during the mission to Justitia."
```

---

### Step 4: Add Info Display

1. **Select "MBR Part Selector"** GameObject (same one)

2. **Add Component:**
   - Add Component → **MBR Part Info Display**

3. **Configure:**
   - **Part Selector**: Should auto-find (or drag MBR Part Selector)
   - **Show Panel**: ✅ ON
   - **Use On GUI**: ✅ ON
   - **Panel Background Color**: Black with alpha 0.7
   - **Text Color**: White
   - **Title Color**: Cyan

---

## 🎮 How It Works

### User Flow:

1. **Scene starts** → Camera shows full MBR (Overview)
2. **Bottom 30%** → Shows "Overview" description
3. **Top-left dropdown** → "▶ Overview"
4. **Click dropdown** → Opens menu showing:
   - Solar Panels
   - Main Engine
   - Antenna
   - Sensors
   - Fuel Tank
   - Command Module
   - Thruster Array
5. **Select "Main Engine"** → Camera rotates to face engine sphere
6. **Bottom panel** → Shows engine description
7. **Drag screen** → Camera orbits around engine
8. **Scroll/pinch** → Zoom in/out on engine
9. **Select "Overview"** → Returns to full spacecraft view

---

## 📱 Screen Layout

```
┌─────────────────────────────┐ ← 0%
│  ▶ Main Engine   [Dropdown] │
│                             │
│         🛸                  │ ← 70%
│      MBR Explorer           │   (3D Viewport)
│   (Camera focused on        │
│     Main Engine)            │
├─────────────────────────────┤ ← 70%
│ Main Engine                 │
│                             │
│ The main propulsion system  │ ← 30%
│ provides delta-v for the    │   (Info Panel)
│ mission to Justitia...      │
└─────────────────────────────┘ ← 100%
```

---

## 🎨 Customizing Part Descriptions

In Inspector → MBR Part Selector → Part Settings → Expand each part:

**Example Descriptions:**

**Solar Panels:**
```
The MBR Explorer features dual solar panel arrays providing 
5kW of power during the cruise phase. Panels automatically 
orient toward the Sun to maximize energy collection.
```

**Main Engine:**
```
The ion propulsion system delivers continuous low-thrust 
acceleration. Xenon fuel provides 10,000 seconds of specific 
impulse for efficient deep-space maneuvering.
```

**Antenna:**
```
High-gain antenna maintains communication with Earth from 
distances up to 800 million km. X-band frequency ensures 
reliable data transmission throughout the mission.
```

---

## 🔧 Adjusting Zoom Per Part

In Inspector → Part Settings → Each part has its own zoom:

**Examples:**
- **Solar Panels**: `3.0` (farther, see both panels)
- **Main Engine**: `1.5` (closer, see details)
- **Antenna**: `2.0` (medium distance)
- **Small parts**: `1.0` (very close)
- **Large sections**: `4.0` (wide view)

**Trial and error:** Select part, adjust zoom, test camera view!

---

## 🎯 Fine-Tuning

### Part Too Close When Selected:

- Increase **Zoom Distance** for that part (e.g., `2.0` → `3.0`)

### Part Too Far:

- Decrease **Zoom Distance** (e.g., `2.0` → `1.2`)

### Want Different Viewing Angle:

- Move the **sphere marker** in POI to different position
- Camera will target that sphere, changing the view angle

### Info Text Too Small:

- Info Display → Use larger font multiplier in code
- Or use TextMeshPro UI instead of OnGUI

---

## ✅ Testing Checklist

Setup:
- [ ] POI GameObject with 7 spheres
- [ ] Each sphere positioned on a spacecraft part
- [ ] MBR Part Selector added
- [ ] Camera Controller on Main Camera
- [ ] All 7 spheres assigned to part settings
- [ ] Zoom distances configured
- [ ] Part descriptions written
- [ ] Info Display component added

Functionality:
- [ ] Dropdown shows parts list
- [ ] Select part → Camera rotates to it
- [ ] Camera orbits around selected part
- [ ] Zoom/rotate works
- [ ] Info panel shows at bottom (30%)
- [ ] Description updates when part changes
- [ ] "Overview" returns to full view

---

## 🐛 Troubleshooting

### Dropdown doesn't show parts

**Solution:**
- Check MBR Part Selector component is enabled
- Press Play and check console for "Auto-assigned" messages

### Camera doesn't move when selecting part

**Solution:**
- Check TouchCameraController is on Main Camera
- Check spheres are assigned to Part Settings
- Check console for errors

### Info panel not visible

**Solution:**
- Check MBR Part Info Display → Show Panel: ON
- Check Use On GUI: ON
- Change panel background color alpha to 1.0 (fully opaque)

### Spheres visible in scene (don't want to see them)

**Solution:**
- Select each sphere → Disable Mesh Renderer

### Zoom distance wrong for a part

**Solution:**
- Adjust that part's Zoom Distance in Inspector
- Test by selecting it and seeing if camera is too close/far

---

## 🎨 UI Customization

### Change Panel Height:

In `MBRPartInfoDisplay.OnGUI()`, change:
```csharp
float panelHeight = Screen.height * 0.3f;  // 30%
```

To:
```csharp
float panelHeight = Screen.height * 0.4f;  // 40% (bigger panel)
```

### Change Colors:

Inspector → MBR Part Info Display:
- **Panel Background Color**: Adjust RGBA
- **Text Color**: Change to any color
- **Title Color**: Change highlight color

---

## 🚀 Quick Start

1. Follow Steps 1-4 above
2. Write descriptions for each part
3. Adjust zoom distances
4. Press Play
5. Select parts from dropdown
6. Camera smoothly rotates to each part!

---

**Your educational spacecraft viewer is ready!** 🛸📚

