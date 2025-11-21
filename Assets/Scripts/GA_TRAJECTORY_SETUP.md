# 🚀 GA Trajectory Setup Guide

## ✅ **What Was Created**

- **`GATrajectoryFollower.cs`** - Reads GA-optimized CSV trajectory and positions MBR Explorer based on simulation date
- **`Assets/StreamingAssets/Trajectories/`** - Folder containing CSV files (works on Android!)
- **`trajectory_justitia_output.csv`** - Copied to StreamingAssets for device compatibility

---

## 📱 **Android Build Fix Applied**

The script now uses **StreamingAssets** folder instead of reading directly from Scripts folder, which fixes the issue where trajectories worked in Editor but not on Android devices.

**What changed:**
- ✅ CSV moved to `StreamingAssets/Trajectories/`
- ✅ Uses `UnityWebRequest` on Android (handles APK compression)
- ✅ Uses `File.ReadAllText` in Editor (faster for testing)
- ✅ Works identically on both platforms

---

## 🎯 **Unity Setup Steps**

### **Step 1: Select MBR Explorer GameObject**

1. In **Hierarchy**, find your duplicated scene
2. Locate **"MBR Explorer"** or **"MBR explorer"** GameObject
3. Click to select it

---

### **Step 2: Remove Old Components (if not done yet)**

In **Inspector**, remove these components:

- ❌ **MBRExplorerPositioner** (old waypoint system)
- ❌ **SpacecraftInitialVelocity** (physics-based)
- ❌ **Rigidbody** (physics not needed)
- ❌ **HardcodedTrajectory** (already deleted)

Keep these:
- ✅ **Transform**
- ✅ **Mesh Renderer** / **Material**
- ✅ **DelayedTrailRenderer** (trail display)
- ✅ **Tag: SmallBody** (for camera selection)

---

### **Step 3: Add GATrajectoryFollower Script**

1. With **MBR Explorer** selected in Inspector
2. Click **Add Component**
3. Search for **"GATrajectoryFollower"**
4. Click to add it

---

### **Step 4: Configure GATrajectoryFollower**

In Inspector → **GATrajectoryFollower** component:

#### **Trajectory Data:**
- **CSV File Name**: `trajectory_justitia_output.csv`
  - (Already set by default!)
  - File is loaded from: `StreamingAssets/Trajectories/` folder
  - This works on both Editor AND Android builds ✅

#### **Simulation Reference:**
- **Simulation Manager**: 
  - Drag **"SimulationManager"** GameObject from Hierarchy → into this slot
  - Or leave empty and it will auto-find it

#### **Scale Settings:**
- **Unity Scale**: `1000000000` (1 billion, already set)
  - This matches your solar system scale

#### **Debug:**
- **Show Debug Info**: ✅ Enabled (see console logs)
- **Draw Gizmos**: ✅ Enabled (see trajectory in Scene view)

---

## 🎮 **How It Works**

1. **On Start:**
   - Reads CSV file
   - Parses all "spacecraft" entries
   - Converts AU coordinates to Unity units
   - Stores trajectory points with date-time stamps

2. **Every Frame:**
   - Gets current date from `EphemerisBasedSimulation`
   - Finds trajectory points before/after current date
   - Interpolates position between those points
   - Updates `transform.position`

3. **Console Output:**
   ```
   ✅ GATrajectoryFollower: Loaded 1260 trajectory points
      Start: 2028-03-02 at (-140.65, 18.68, 43.80)
      End: 2035-05-01 at (24.54, 25.40, 163.98)
   
   🚀 MBR Explorer | Date: 2028-07-10 12:30 | Pos: (54.2, -5.4, -68.1) | Distance: 1.32 AU
   ```

---

## 🎨 **Scene View Gizmos**

When you select MBR Explorer in Scene view, you'll see:

- **Cyan line** - Full trajectory path
- **Green sphere** - Start position (launch)
- **Red sphere** - End position (mission end)
- **Yellow sphere** - Current position (during Play mode)
- **White line** - Distance from Sun to spacecraft

---

## 🐛 **Troubleshooting**

### **"CSV file not found" Error**

1. **Check the file exists:**
   - Unity Editor: `Assets/StreamingAssets/Trajectories/trajectory_justitia_output.csv`
   - The file should be visible in Project window

2. **Check the filename in Inspector:**
   - Should be just the filename: `trajectory_justitia_output.csv`
   - NOT a full path

3. **Rebuild the app** if you just added the file to StreamingAssets

---

### **"No SimulationManager found" Error**

1. In Hierarchy, find **"SimulationManager"** GameObject
2. Drag it into the **Simulation Manager** slot in Inspector
3. Or make sure it has **EphemerisBasedSimulation** component

---

### **Spacecraft Not Moving**

1. **Check Console** - Look for load errors
2. **Check SimulationManager** - Is time progressing?
3. **Check CSV Path** - Does the file exist?
4. **Verify Date Range** - Current sim date must be within CSV date range

---

## 📊 **CSV Format**

The script expects CSV with these columns:
```
type,x_au,y_au,z_au,time
spacecraft,-0.939920837,0.292726087,0.124805286,2028-03-02 00:00:00
spacecraft,-0.939018684,0.311923875,0.124771611,2028-03-03 00:00:00
...
```

- **type**: Must be "spacecraft" (other types are ignored)
- **x_au, y_au, z_au**: Position in Astronomical Units
- **time**: Date-time in format `YYYY-MM-DD HH:MM:SS`

---

## 🎯 **Testing**

1. **Press Play**
2. **Check Console** for load success message
3. **Open Planet Selector** dropdown (top-left)
4. **Select "MBR Explorer"**
5. **Camera should follow** the spacecraft!
6. **Increase time speed** to see it move along trajectory
7. **Watch for Venus GA**, **Earth GA**, **Mars GA**, **asteroid flybys**, **Justitia arrival**

---

## ✨ **Features**

- ✅ Reads GA-optimized trajectory from CSV
- ✅ Syncs perfectly with simulation time
- ✅ Smooth interpolation between data points
- ✅ Works with time speedup/slowdown
- ✅ Visual gizmos in Scene view
- ✅ Debug logging with mission day and distance
- ✅ Compatible with existing camera system
- ✅ Trail renderer shows path history

---

## 🚀 **You're All Set!**

Your MBR Explorer now follows the GA-optimized trajectory instead of the hardcoded one!

The trajectory includes:
- **Launch** from Earth
- **Venus Gravity Assist**
- **Earth Gravity Assist**
- **Mars Gravity Assist**
- **Multiple asteroid flybys** (Westerwald, Chimaera, Rockox, Moza, Ousha, Ghaf)
- **Justitia arrival and orbit**

Enjoy watching your optimized mission! 🎉

