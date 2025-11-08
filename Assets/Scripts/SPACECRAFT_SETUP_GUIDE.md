# 🚀 MBR Explorer Spacecraft Setup Guide

## ✅ **What's Already Done**

The following has been configured in your scripts:
- ✅ **GravitySimulator** infrastructure (already built into `EphemerisBasedSimulation.cs`)
- ✅ **PlanetSelector** updated with "MBR Explorer" entry
- ✅ **SpacecraftInitialVelocity.cs** script created
- ✅ Zoom distance set to `0.005f` (very close, since it's small)

---

## 🎯 **Unity Setup Steps**

### **Step 1: Create the Spacecraft GameObject**

1. In **Hierarchy**, right-click → **3D Object → Cube**
2. Rename to: `MBR Explorer` (exact name, case-sensitive!)

### **Step 2: Set Transform**

**Scale** (representing 2×3×4 meters):
```
X: 0.002  (2 meters at 1e-9 scale)
Y: 0.003  (3 meters)
Z: 0.004  (4 meters)
```

**Position** (starting near Earth):
```
X: 150
Y: 0
Z: 0
```
*This places it ~150 million km from Sun (near Earth's orbit)*

**Rotation:**
```
X: 0, Y: 0, Z: 0
```

### **Step 3: Add Components**

#### **A. Add Rigidbody**
1. Click **Add Component → Physics → Rigidbody**
2. Configure:
   - **Mass:** `2464.9`
   - **Drag:** `0`
   - **Angular Drag:** `0`
   - **Use Gravity:** ☑ **OFF** (important!)
   - **Is Kinematic:** ☐ off
   - **Interpolate:** None
   - **Collision Detection:** Discrete

#### **B. Add SpacecraftInitialVelocity Script**
1. Click **Add Component**
2. Search for `SpacecraftInitialVelocity`
3. Set **Initial Velocity:**
   ```
   X: 0
   Y: 0
   Z: 0.03
   ```
   *This gives it ~30 km/s orbital velocity (matching Earth's orbit)*

#### **C. Tag as SmallBody**
1. At the top of Inspector, find **Tag** dropdown
2. If "SmallBody" doesn't exist:
   - Click dropdown → **Add Tag...**
   - Click **+** button
   - Type: `SmallBody`
   - Click **Save**
3. Go back to MBR Explorer GameObject
4. Set **Tag** to: `SmallBody`

### **Step 4: Add Visual Enhancements (Optional)**

#### **A. Add Material (for visibility)**
1. Right-click in Project → **Create → Material**
2. Name it `SpacecraftMaterial`
3. Set **Albedo** color to bright color (e.g., bright red or white)
4. Drag material onto the MBR Explorer cube

#### **B. Add Trail Renderer (to see path)**
1. Select MBR Explorer
2. **Add Component → Effects → Trail Renderer**
3. Configure:
   - **Time:** `200` (how long trail lasts)
   - **Width:** `0.02`
   - **Min Vertex Distance:** `0.01`
4. Create/assign a bright material for the trail

#### **C. Add Text Label (like planets)**
1. Right-click MBR Explorer → **Create Empty**
2. Rename to "Label"
3. Right-click Label → **UI → Text - TextMeshPro**
4. On the TextMeshPro component:
   - Set **Text:** "MBR Explorer"
   - Set **Font Size:** `0.5`
   - Set **Alignment:** Center
5. Add **Billboard.cs** script to Label
6. Add **ConstantSizeText.cs** script to Label
   - Set **Target Size:** `0.2`

---

## 🎮 **How to Use in Game**

1. **Press Play**
2. **Open the planet selector** (top-left dropdown: "▶ Sun")
3. **Click dropdown** → You'll see "MBR Explorer" in the list
4. **Select "MBR Explorer"**
5. **Camera zooms** to your spacecraft!

---

## 🔧 **Adjusting Orbital Velocity**

### **Common Orbital Velocities** (at scale 1e-9):

| Orbit Type | Real Velocity | Unity Velocity (m/s) |
|------------|---------------|---------------------|
| **Earth's orbit** | 30 km/s | 0.03 |
| **Mars' orbit** | 24 km/s | 0.024 |
| **Fast escape** | 50 km/s | 0.05 |
| **Slow orbit** | 20 km/s | 0.02 |

**To change velocity:**
- Select MBR Explorer
- Find `SpacecraftInitialVelocity` component
- Adjust **Initial Velocity** vector

### **Velocity Direction Tips:**

```
// Circular orbit (counter-clockwise when viewed from top)
Velocity: (0, 0, 0.03)  // Tangent to orbit

// Elliptical orbit
Velocity: (0.01, 0, 0.03)  // Combined tangent + radial

// Escape trajectory
Velocity: (0, 0, 0.05)  // Faster than circular
```

---

## ⚙️ **How GravitySimulator Works**

Once set up, the system automatically:

1. **Finds** all GameObjects tagged "SmallBody" (including MBR Explorer)
2. **Reads** initial position and velocity from Transform + Rigidbody
3. **Converts** to astronomical units for calculation
4. **Calculates** gravitational forces from:
   - Sun
   - Mercury, Venus, Earth, Mars
   - Jupiter, Saturn, Uranus, Neptune
5. **Updates** position every frame using numerical integration
6. **Applies** new position back to GameObject

### **Forces Acting on Spacecraft:**
- **Primary:** Sun's gravity (~99% of total force)
- **Secondary:** Jupiter's gravity (if near Jupiter)
- **Tertiary:** Other planets (small but realistic)

---

## 🎯 **Positioning Examples**

### **Near Earth (Low Earth Orbit):**
```
Position: (150, 0, 0)
Velocity: (0, 0, 0.03)
Result: Orbits at same distance as Earth
```

### **Transfer to Mars:**
```
Position: (150, 0, 0)
Velocity: (0, 0, 0.035)
Result: Elliptical transfer orbit (Hohmann transfer)
```

### **Earth-Sun L1 Lagrange Point:**
```
Position: (148.5, 0, 0)
Velocity: (0, 0, 0.03)
Result: Stays between Earth and Sun
```

### **Jupiter Flyby:**
```
Position: (780, 0, 0)  // Jupiter's orbit
Velocity: (0, 0, 0.013)  // Match Jupiter's orbit
Result: Close approach to Jupiter
```

---

## 📊 **Status Monitoring**

In **Console**, you'll see:
```
EphemerisSim: Found 1 small bodies for gravity simulation
EphemerisSim: GravitySimulator initialized with 1 small bodies
Spacecraft initial velocity set to (0.00, 0.00, 0.03)
```

---

## 🐛 **Troubleshooting**

### **Spacecraft not moving:**
- ✓ Check Tag is set to "SmallBody"
- ✓ Check "Use Gravity" is OFF on Rigidbody
- ✓ Check Initial Velocity is not (0,0,0)
- ✓ Check simulation "isRunning" is enabled

### **Can't find in dropdown:**
- ✓ GameObject must be named exactly "MBR Explorer"
- ✓ GameObject must exist before pressing Play

### **Spacecraft disappears:**
- ✓ Velocity too high (escape velocity)
- ✓ Check position is within view distance
- ✓ Add Trail Renderer to see path

### **Camera doesn't zoom correctly:**
- ✓ Zoom distance is `0.005f` (very close)
- ✓ Adjust in `PlanetSelector.cs` if needed
- ✓ Spacecraft may be very small, increase scale if needed

---

## 🚀 **Next Steps**

1. **Set up the spacecraft** following steps above
2. **Test basic orbit** with velocity (0, 0, 0.03)
3. **Experiment** with different positions and velocities
4. **Add trail renderer** to visualize path
5. **Try different missions**:
   - Mars transfer
   - Jupiter flyby
   - Sun dive
   - Escape solar system

---

## 📝 **Technical Details**

**Mass:** 2464.9 kg
- Small compared to planets (Earth = 5.972 × 10²⁴ kg)
- 4-5 orders of magnitude larger than typical satellites
- Gravity influence on planets: **negligible**
- Planets' influence on spacecraft: **significant**

**Size:** 2 × 3 × 4 meters
- Unity scale: 0.002 × 0.003 × 0.004
- Visual representation only
- Collision not typically used in orbital simulation

**Simulation Method:**
- Uses `GravitySimulator` class from Astronomy Engine
- Numerical integration (RK4 or similar)
- Updates every `FixedUpdate()` call
- Time step controlled by `timeStepMultiplier`

---

Enjoy your spacecraft simulation! 🚀✨

