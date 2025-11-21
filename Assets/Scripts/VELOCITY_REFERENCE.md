# 🚀 Spacecraft Velocity Reference Card

Quick reference for typical spacecraft velocities in your Unity simulation.

---

## 📐 **Coordinate System**

- **X-axis**: Radial (toward/away from Sun)
- **Y-axis**: Perpendicular to orbital plane (north/south)
- **Z-axis**: Tangential (along orbital direction)

**Default starting position**: (150, 0, 0) - Near Earth's orbit

---

## 🎯 **Common Trajectories**

### **1. Circular Earth Orbit**
```
Velocity X: 0.01
Velocity Y: 0.0
Velocity Z: 0.03
Launch Angle: 0°
```
**Result:** Stays near Earth, circular orbit around Sun

---

### **2. Elliptical Earth Orbit**
```
Velocity X: 0.015
Velocity Y: 0.0
Velocity Z: 0.03
Launch Angle: 0°
```
**Result:** Elongated orbit, varying distance from Sun

---

### **3. Mars Transfer (Hohmann)**
```
Velocity X: 0.01
Velocity Y: 0.0
Velocity Z: 0.035
Launch Angle: 0°
```
**Result:** Trajectory reaches Mars orbit (~228M km)

---

### **4. High Elliptical Orbit**
```
Velocity X: 0.02
Velocity Y: 0.0
Velocity Z: 0.03
Launch Angle: 0°
```
**Result:** Very elongated orbit, reaches asteroid belt

---

### **5. Escape Velocity**
```
Velocity X: 0.02
Velocity Y: 0.0
Velocity Z: 0.05
Launch Angle: 0°
```
**Result:** Escapes solar system!

---

### **6. Inward Spiral (Venus/Mercury)**
```
Velocity X: 0.005
Velocity Y: 0.0
Velocity Z: 0.025
Launch Angle: 0°
```
**Result:** Falls toward Sun, may reach inner planets

---

### **7. Polar Orbit**
```
Velocity X: 0.01
Velocity Y: 0.02
Velocity Z: 0.03
Launch Angle: 0°
```
**Result:** Orbit inclined to solar system plane

---

### **8. Retrograde Orbit**
```
Velocity X: -0.01
Velocity Y: 0.0
Velocity Z: -0.03
Launch Angle: 180°
```
**Result:** Orbits opposite to planets (dangerous!)

---

## 🎲 **Experimental Values**

### **Random Walk**
```
Velocity X: Random(-0.02 to 0.02)
Velocity Y: Random(-0.01 to 0.01)
Velocity Z: Random(0.02 to 0.04)
Launch Angle: Random(0° to 360°)
```

### **Zero Velocity (Freefall)**
```
Velocity X: 0.0
Velocity Y: 0.0
Velocity Z: 0.0
Launch Angle: 0°
```
**Result:** ⚠️ Falls straight into Sun!

---

## 📏 **Velocity Magnitude Guide**

| Speed (m/s Unity) | Real-World Equivalent | Expected Behavior |
|-------------------|----------------------|-------------------|
| 0.01 - 0.02 | ~10-20 km/s | Slow orbit, may fall inward |
| 0.025 - 0.035 | ~25-35 km/s | Stable planetary orbits |
| 0.04 - 0.05 | ~40-50 km/s | High-speed, escape possible |
| 0.06+ | ~60+ km/s | Definitely escapes |

---

## 🧮 **Formula Reference**

### **Circular Orbit Velocity at Distance R:**
```
v_circular ≈ 0.03 * sqrt(150 / R)
```
Where R is distance from Sun in Unity units

### **Escape Velocity at Distance R:**
```
v_escape ≈ 0.042 * sqrt(150 / R)
```

---

## 🎯 **Quick Tips**

1. **Stable orbit near Earth**: vz ≈ 0.03, vx ≈ 0.01
2. **Go faster**: Increase vz (orbital velocity)
3. **Go slower**: Decrease vz (spiral inward)
4. **Change orbit shape**: Adjust vx (radial component)
5. **Out of plane**: Add vy component (north/south)
6. **Start at different angle**: Change Launch Angle

---

## 💡 **Experimentation Ideas**

### **Find the Perfect Transfer**
Try different velocities to reach Mars with minimum fuel:
- Too slow → Falls back toward Sun
- Too fast → Overshoots Mars
- Just right → Arrives at Mars orbit

### **Gravity Assist Challenge**
Launch toward Venus, use its gravity to boost toward Jupiter:
- Initial velocity toward Venus
- Let Venus "slingshot" the spacecraft
- Watch trajectory change!

### **Asteroid Intercept**
Match velocity with specific asteroids in the scene:
- Study asteroid orbital velocity
- Launch to intercept at right time
- Fine-tune approach angle

---

## 📱 **For Kotlin App UI**

### **Suggested Input Ranges:**

```kotlin
// EditText input validation
val velocityX = editVelocityX.text.toString().toFloatOrNull()
    ?.coerceIn(-0.05f, 0.05f) ?: 0.01f

val velocityY = editVelocityY.text.toString().toFloatOrNull()
    ?.coerceIn(-0.03f, 0.03f) ?: 0.0f

val velocityZ = editVelocityZ.text.toString().toFloatOrNull()
    ?.coerceIn(0.0f, 0.08f) ?: 0.03f

val launchAngle = editLaunchAngle.text.toString().toFloatOrNull()
    ?.coerceIn(0f, 360f) ?: 0f
```

### **Preset Buttons:**

Add quick-select buttons in your Kotlin UI:
- **Earth Orbit** → (0.01, 0, 0.03, 0°)
- **Mars Transfer** → (0.01, 0, 0.035, 0°)
- **Escape** → (0.02, 0, 0.05, 0°)

---

**Happy experimenting!** 🚀✨

