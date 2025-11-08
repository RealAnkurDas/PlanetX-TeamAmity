# Troubleshooting: Planets Not Moving

## 🔍 **Check These in Order:**

### **1. Is the Script Running?**

**Check Console for:**
```
EphemerisSim: Using current date/time (UTC): ...
EphemerisSim: Found X objects with 'Celestial' tag
  - Cached Mercury: ...
  - Cached Venus: ...
  ... (for each planet)
EphemerisSim: Total planets cached: 8
EphemerisSim: Simulation initialized and running!
```

**If you DON'T see these messages:**
- ❌ The script hasn't started properly
- **Solution**: Check that EphemerisBasedSimulation component is enabled

---

### **2. Check isRunning Flag**

**In Inspector:**
1. Select SimulationManager GameObject
2. Look at EphemerisBasedSimulation component
3. Find **"Is Running"** under Status section
4. Should show: `true`

**If it shows `false`:**
- Script initialized but something failed
- Check Console for error messages

---

### **3. Are Planets Being Found?**

**Check Console for:**
```
EphemerisSim: Found 8 objects with 'Celestial' tag
```

**If it shows 0 or less than 8:**
- ❌ Planets don't have "Celestial" tag
- **Solution**: Select each planet → Inspector → Tag → "Celestial"

---

### **4. Check FixedUpdate is Running**

**After ~1 second of Play, Console should show:**
```
EphemerisSim: Frame 50, Date: ..., Time: ..., Planets: 8
```

**If you DON'T see this:**
- ❌ FixedUpdate isn't being called
- **Possible causes:**
  - Script is disabled
  - Time.timeScale is 0
  - isRunning is false

**Solutions:**
- Check component is enabled (checkbox next to script name)
- Check Edit → Project Settings → Time → Time Scale = 1
- Check isRunning in Inspector

---

### **5. Are Planets Moving Slowly?**

**Planets might be moving, but VERY slowly:**

**Check:**
- Time Step Multiplier in Inspector
- Default: 86400 (1 day per second)

**If it's set too low** (like 1 or 100):
- Motion is there but imperceptible
- **Solution**: Set to `86400` or higher

---

### **6. Are Old Scripts Still Running?**

**Make sure these are DISABLED:**
- ☐ Orbit (SolarSystem.cs) component
- ☐ PlanetPositionInitializer component  
- ☐ Old SimulationTimeTracker component

**Check:**
1. Find each GameObject with these scripts
2. In Inspector, **uncheck** the checkbox next to script name
3. Don't delete - just disable

**If both old and new are enabled:**
- They might conflict
- Old one might override new positions

---

### **7. Check Rigidbody Settings**

**For each planet, check Rigidbody:**

**Should be:**
- ☐ Use Gravity: **Unchecked**
- Is Kinematic: Can be checked or unchecked (doesn't matter)
- Constraints: None needed

**If "Use Gravity" is checked:**
- Planet will fall due to Unity's built-in gravity
- **Solution**: Uncheck it

---

### **8. Quick Test: Manual Position Change**

**In Console, you should see planets being positioned:**
```
Set Mercury position to (...)
Set Venus position to (...)
```

**If you don't see these:**
- UpdatePlanetPositions() isn't running
- Check the Console for warnings

---

## 🐛 **Common Issues & Solutions:**

### **Issue: "Not running!" warning in Console**

**Cause**: isRunning = false

**Solution:**
1. Check if Start() was called
2. Check if InitializeSimulation() completed
3. Look for errors earlier in Console
4. Try manually setting isRunning to true in Inspector while playing

---

### **Issue: "No planets cached!"**

**Cause**: Planets don't have "Celestial" tag

**Solution:**
1. Select each planet GameObject
2. Inspector → Tag dropdown → Select "Celestial"
3. Stop and restart Play mode

---

### **Issue: Planets jump to (0,0,0)**

**Cause**: Conversion issue or missing Sun

**Solution:**
1. Check that Sun Sphere exists
2. Check Scale To Unity Units = 1E-09
3. Look for any error messages about conversion

---

### **Issue: Console shows "Frame 50..." but planets don't move**

**Cause**: 
- Positions ARE being updated
- But maybe movement is too slow to see
- Or planets have constraints

**Solution:**
1. Increase Time Step Multiplier to 2592000 (1 month/sec)
2. Watch if they move faster
3. Check Rigidbody constraints (should be none)
4. Check parent transforms (planets should be root objects or in unmoving parent)

---

### **Issue: Date/time updates but positions don't**

**Cause**: UpdatePlanetPositions() has an issue

**Solution:**
1. Check Console for "No planets cached!" warning
2. Verify planetObjects.Count > 0 in Inspector (while playing)
3. Try logging inside UpdatePlanetPositions with Debug.Log

---

## ✅ **Verification Steps:**

Run through this checklist while in Play mode:

1. **Console shows initialization messages** ✓
2. **"Is Running" = true** in Inspector ✓
3. **Planets cached count = 8** in Inspector ✓
4. **Frame counter increasing** (every 50 frames log) ✓
5. **Date/time advancing** at bottom of screen ✓
6. **Time Step Multiplier = 86400** ✓
7. **Old scripts disabled** ✓
8. **All planets have "Celestial" tag** ✓

---

## 🔧 **Quick Fix: Add Debug to One Planet**

**Temporary test** - add this to EphemerisBasedSimulation.cs in UpdatePlanetPositions():

```csharp
// Inside the foreach loop, add:
if (planet == Body.Earth)
{
    Debug.Log($"Earth position updated to: {unityPos}, Astro: ({helioPos.x:F2}, {helioPos.y:F2}, {helioPos.z:F2}) AU");
}
```

This will log Earth's position every frame. You should see:
- Position values changing
- Coordinates slowly shifting as Earth orbits

---

## 📞 **What to Report:**

If still not working, check Console and report:

1. ✅ What initialization messages appear?
2. ✅ What is "Is Running" value in Inspector?
3. ✅ How many planets were cached?
4. ✅ Are Frame logs appearing?
5. ✅ Is date/time advancing?
6. ✅ Any error or warning messages?

This will help identify the exact issue! 🔍

