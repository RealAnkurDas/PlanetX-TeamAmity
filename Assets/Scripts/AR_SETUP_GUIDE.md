# 🌌 AR Solar System Setup Guide

Complete guide to set up your AR scene with the solar system at full scale.

---

## 🎯 Core Concept

Instead of shrinking the solar system to room-scale, we **scale up the XR Origin** (the "player") to match the solar system's massive scale. This keeps all your existing physics, scripts, and scales intact.

**Example:**
- Solar system scale: Sun is ~1.39 million km diameter
- XR Origin scale: 1000x (makes you 1000x bigger)
- Result: You can see and interact with the full-scale solar system in AR

---

## 📋 Setup Steps

### Step 1: Open Your AR Scene

Open `Solar System Animated View AR.unity`

---

### Step 2: Configure XR Origin

1. **Find XR Origin** in Hierarchy (created when you added XR → XR Origin Mobile AR)

2. **Add AR Solar System Manager:**
   - Select **XR Origin** (or create an empty GameObject called "AR Manager")
   - Add Component → **AR Solar System Manager**
   - Configure:
     - **XR Origin**: Drag your XR Origin here
     - **Solar System Root**: Drag the root GameObject containing your solar system (usually the parent of the Sun)
     - **XR Origin Scale**: Set to `1000` (adjust based on how it looks)
     - **Initial Distance**: `5` (how far in front of camera to place system)
     - **AR Camera**: Should auto-detect, or drag Main Camera from XR Origin
     - **Far Clip Plane**: `10000000` (10 million - for viewing distant planets)
     - **Near Clip Plane**: `0.01`

3. **Inspector Settings for XR Origin:**
   - Transform → Scale: Leave at `1, 1, 1` (script will handle scaling)
   - Position: `0, 0, 0`

---

### Step 3: Disable Non-AR Scripts

#### Method A: Manual (Quick)

Find these GameObjects and **DISABLE** their scripts:
- `TouchCameraController` → Uncheck "enabled"

#### Method B: Automatic (Recommended)

1. Find GameObject with `TouchCameraController`
2. Add Component → **AR Mode Detector**
3. In Inspector:
   - **Disable In AR**: Click `+` to add elements
   - Drag `TouchCameraController` script into the array
4. Script will auto-disable it when in AR mode

---

### Step 4: Configure AR Camera

1. **Find Main Camera** (inside XR Origin → Camera Offset)

2. **Camera Settings:**
   - Clear Flags: **Solid Color**
   - Background: **Black** (RGB: 0, 0, 0, Alpha: 255)
   - Culling Mask: **Everything**
   - Projection → Near: `0.01`
   - Projection → Far: `10000000`

3. **Add AR Camera Manager** (if not present):
   - Select Main Camera
   - Add Component → **AR Camera Manager**
   - Leave settings as default

---

### Step 5: Add AR Session (if not present)

1. Right-click in Hierarchy
2. GameObject → XR → **AR Session**
3. Leave default settings

---

### Step 6: Optional - Add Scale Control UI

If you want to let users zoom in/out by changing scale:

1. **Create UI Button:**
   - Right-click in Hierarchy → UI → **Button**
   - Rename to "Scale Controls"

2. **Add AR Scale Controller:**
   - Create empty GameObject: "AR Scale Controller"
   - Add Component → **AR Scale Controller**
   - Configure:
     - **AR Manager**: Drag the GameObject with ARSolarSystemManager
     - **Min Scale**: `100`
     - **Max Scale**: `10000`
     - **Default Scale**: `1000`

3. **Wire up buttons** (optional):
   - Create 3 buttons: "Zoom In", "Zoom Out", "Reset"
   - On each button's OnClick event, drag AR Scale Controller
   - Select: `IncreaseScale()`, `DecreaseScale()`, `ResetScale()`

---

## 🧪 Testing in Unity Editor

**Warning:** AR won't work fully in Unity Editor (needs real device)

**What you'll see:**
- Console message: "AR Mode detected"
- Solar system placed in scene
- Camera at origin

**To test properly:**
- Build to Android device (ARCore)
- Build to iOS device (ARKit)

---

## 📱 Build Settings

1. **File → Build Settings**

2. **Platform: Android** (or iOS)

3. **Player Settings:**
   - **Minimum API Level**: 24 (Android 7.0) or higher
   - **Target API Level**: Latest
   - **XR Plug-in Management**:
     - ✅ Enable **ARCore** (Android)
     - ✅ Enable **ARKit** (iOS)

4. **Add Scene:**
   - ✅ Solar System Animated View AR

5. **Build and Run** to device

---

## 🎮 How It Works

### XR Origin Scale Magic:

```
Real World: You move 1 meter
XR Origin Scale: 1000x
Solar System Sees: You moved 1000 meters (1 km)
```

This lets you:
- Walk around your room = travel thousands of km in the solar system
- Look up/down = see planets above/below
- Physically move = navigate the solar system

### Example Scales:

| XR Origin Scale | Best For |
|----------------|----------|
| 100x | Very close-up view (individual planets) |
| 1000x | Good balance (inner solar system) |
| 10000x | Wide view (whole solar system) |

---

## 🔧 Troubleshooting

### Problem: Can't see anything in AR

**Solution:**
1. Check XR Origin scale is set (should be ~1000)
2. Check AR Camera far clip plane is 10000000
3. Check solar system is placed (console log should say "Placed solar system")

### Problem: Everything is too small

**Solution:**
- Increase XR Origin Scale: `2000` or `5000`
- Or use AR Scale Controller to adjust dynamically

### Problem: Everything is too big

**Solution:**
- Decrease XR Origin Scale: `500` or `100`

### Problem: Touch controls still active in AR

**Solution:**
- Add AR Mode Detector script
- Add TouchCameraController to "Disable In AR" array

### Problem: Solar system physics not working

**Solution:**
- Make sure Orbit/EphemerisBasedSimulation scripts are still enabled
- These should work fine in AR mode

---

## ✅ Checklist

- [ ] XR Origin added to scene
- [ ] AR Session added to scene
- [ ] AR Solar System Manager added and configured
- [ ] XR Origin scale set (~1000)
- [ ] AR Camera far plane set to 10000000
- [ ] TouchCameraController disabled
- [ ] Scene saved
- [ ] ARCore/ARKit enabled in Project Settings
- [ ] Built to device and tested

---

## 🚀 Next Steps

Once working:
- Adjust XR Origin Scale to find best viewing distance
- Add UI to show current scale
- Add gesture controls for interaction
- Add AR plane visualization (optional)

---

## 📝 Notes

- **Don't rescale the solar system** - XR Origin scale handles everything
- **Physics still works** - Orbit calculations unaffected by XR Origin scale
- **UI still works** - OnGUI renders correctly on AR camera
- **Original scene untouched** - All changes only in AR scene

---

**Need help?** Check console logs for "ARSolarSystemManager" messages.

