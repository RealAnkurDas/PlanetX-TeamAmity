# ✅ Gravity Simulation Checklist

## 🎯 **Quick Setup Verification**

Follow this checklist to ensure gravity simulation works:

---

## 📋 **On MBR Explorer GameObject:**

### **1. Tag (CRITICAL)**
- [ ] Tag is set to **"SmallBody"** (not "Untagged"!)
- [ ] How to check: Select MBR Explorer → Top of Inspector → Tag dropdown

---

### **2. Rigidbody Component (REQUIRED)**
- [ ] Has **Rigidbody** component
- [ ] **Use Gravity**: ☐ **UNCHECKED** (very important!)
- [ ] **Is Kinematic**: ☐ **UNCHECKED**
- [ ] **Mass**: Any value (e.g., 2464.9 kg)
- [ ] **Drag**: 0
- [ ] **Angular Drag**: 0

**How to check:**
- Select MBR Explorer → Inspector → Find Rigidbody component

---

### **3. Initial Velocity (CHECK THIS)**
- [ ] Rigidbody has velocity when simulation starts
- [ ] Velocity should be set by KotlinLaunchReceiver
- [ ] Check Console for: "✅ Velocity applied: (...)"

---

### **4. Position**
- [ ] Starts at Earth's location (if Launch From Earth is enabled)
- [ ] Or at custom position (150, 0, 0 for near Earth)

---

## 📋 **In Scene:**

### **5. EphemerisBasedSimulation (REQUIRED)**
- [ ] Scene has GameObject with **EphemerisBasedSimulation** component
- [ ] Usually on "SimulationManager" GameObject
- [ ] **Is Running**: Should be ☑ checked (or becomes checked when Play starts)

**How to check:**
- Find "SimulationManager" in Hierarchy
- Inspector → EphemerisBasedSimulation → Is Running checkbox

---

### **6. Earth GameObject (for launch)**
- [ ] Scene has GameObject named **"Earth"** (case-sensitive)
- [ ] Earth has position around (150, 0, 0)
- [ ] Earth may have Rigidbody with velocity

---

## 🔍 **Use Diagnostic Script:**

### **Add to MBR Explorer:**
1. Select **MBR Explorer**
2. **Add Component** → **GravitySimulationDiagnostic**
3. **Press Play**
4. **Check Console** every 2 seconds for diagnostic report

**Console should show:**
```
========================================
🔍 GRAVITY SIMULATION DIAGNOSTIC
   Spacecraft: MBR Explorer
   ✅ Tag: SmallBody (correct!)
   ✅ Rigidbody: Present
      Mass: 2464.9 kg
      Use Gravity: False (should be FALSE)
      Velocity: (0.01, 0, 0.03)
      Speed: 0.031623 m/s
   ✅ Use Gravity: OFF (correct!)
   ✅ Velocity: 0.031623 m/s (good!)
   ✅ MOVING: Moved 0.153 units since last check
   ✅ EphemerisBasedSimulation: Found
========================================
```

---

## ❌ **Common Issues:**

### **Issue: "NOT MOVING"**
**Causes:**
- ❌ Tag is not "SmallBody"
- ❌ No Rigidbody component
- ❌ Velocity is zero
- ❌ Use Gravity is ON (using Unity physics instead)
- ❌ EphemerisBasedSimulation not running

**Fix:**
1. Set Tag to "SmallBody"
2. Turn OFF "Use Gravity" on Rigidbody
3. Check velocity is set (run diagnostic)

---

### **Issue: "Falls into Sun immediately"**
**Cause:**
- ❌ Velocity too low or zero

**Fix:**
- Increase velocity (try vX=0.01, vY=0, vZ=0.03)
- Press 'R' key during Play to reinitialize

---

### **Issue: "Flies away immediately"**
**Cause:**
- ❌ Velocity too high

**Fix:**
- Decrease velocity
- Typical range: 0.02-0.04 m/s total speed

---

### **Issue: "Straight line, no curve"**
**Causes:**
- ❌ Use Gravity is ON (using Unity gravity, not N-body)
- ❌ GravitySimulator not initialized

**Fix:**
1. Turn OFF "Use Gravity"
2. Check Console for "GravitySimulator initialized"
3. Press 'R' key to reinitialize if needed

---

## 🎮 **Quick Test:**

1. **Press Play** in Unity
2. **Wait 5 seconds**
3. **Look at Console** - Should see diagnostic every 2 seconds
4. **Should say:** "✅ MOVING" and show distance moved
5. **If NOT moving** → Check the ❌ items above

---

## 🔧 **Manual Reinitialize:**

If spacecraft is stuck:
1. **Keep Play mode running**
2. **Press 'R' key** on keyboard
3. Console shows: "Manual reinitialization requested"
4. Gravity simulator reloads with current velocity

---

## ✅ **Correct Setup Summary:**

```
MBR Explorer
├── Tag: "SmallBody" ✅
├── Transform: (at Earth's position)
├── Rigidbody ✅
│   ├── Use Gravity: OFF ✅
│   ├── Is Kinematic: OFF ✅
│   └── Velocity: (0.01, 0, 0.03) ✅
├── KotlinLaunchReceiver (on separate GameObject)
└── GravitySimulationDiagnostic ✅ (for testing)

Scene:
├── SimulationManager (with EphemerisBasedSimulation) ✅
└── Earth (for launch position) ✅
```

---

**Run the diagnostic and paste the console output here if still not working!** 🔍

