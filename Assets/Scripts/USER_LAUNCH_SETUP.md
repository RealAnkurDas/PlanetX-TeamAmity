# 🚀 User-Controlled Launch Setup Guide

## ✅ **What Was Created**

- **`KotlinLaunchReceiver.cs`** - Receives velocity and angle from Kotlin, applies to spacecraft, starts gravity simulation

---

## 🎯 **Unity Setup Steps**

### **Step 1: Prepare Your Scene**

1. Open **"Solar System Animated View 3D User"** scene
2. Make sure you have **"MBR Explorer"** GameObject with:
   - ✅ **Rigidbody** component (Use Gravity = OFF)
   - ✅ **Tag** set to "SmallBody"
   - ✅ **Transform** at starting position (e.g., near Earth: 150, 0, 0)

---

### **Step 2: Remove Conflicting Scripts**

On **MBR Explorer** GameObject, **remove or disable** these components:

- ❌ **GATrajectoryFollower** (we're not using CSV trajectory)
- ❌ **SpacecraftInitialVelocity** (will be overridden anyway, optional to remove)
- ❌ **MBRExplorerPositioner** (if present)

Keep these:
- ✅ **Transform**
- ✅ **Rigidbody**
- ✅ **Mesh Renderer** / Material
- ✅ **DelayedTrailRenderer** (shows trajectory trail)

---

### **Step 3: Add Launch Receiver**

1. In **Hierarchy**, right-click → **Create Empty**
2. Rename to **"LaunchParameterReceiver"**
3. Select it, in **Inspector** → **Add Component**
4. Search for **"KotlinLaunchReceiver"**
5. Click to add it

---

### **Step 4: Configure Launch Receiver**

In Inspector → **KotlinLaunchReceiver** component:

#### **Spacecraft Reference:**
- Drag **"MBR Explorer"** from Hierarchy → into **Spacecraft** slot
- (Or leave empty, it will auto-find it)

#### **Default Values (Editor Testing):**
These are used when testing in Unity Editor (not on Android):
- **Default Velocity X**: `0.01` (m/s, slight rightward)
- **Default Velocity Y**: `0.0` (m/s, no vertical)
- **Default Velocity Z**: `0.03` (m/s, forward motion)
- **Default Launch Angle**: `0` (degrees)

#### **Launch Position:**
- **Use Custom Launch Position**: ☐ Leave unchecked (uses spacecraft's current position)
- Or check it and set custom position (e.g., 150, 0, 0 for near Earth)

#### **Debug:**
- **Show Debug Info**: ✅ Enabled (see console logs)

---

## 📱 **Kotlin Side Setup**

### **Kotlin Code (MainActivity.kt):**

```kotlin
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayerActivity

class MainActivity : AppCompatActivity() {
    
    // UI elements
    private lateinit var editVelocityX: EditText
    private lateinit var editVelocityY: EditText
    private lateinit var editVelocityZ: EditText
    private lateinit var editLaunchAngle: EditText
    private lateinit var btnLaunchSimulation: Button
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        // Initialize UI
        editVelocityX = findViewById(R.id.editVelocityX)
        editVelocityY = findViewById(R.id.editVelocityY)
        editVelocityZ = findViewById(R.id.editVelocityZ)
        editLaunchAngle = findViewById(R.id.editLaunchAngle)
        btnLaunchSimulation = findViewById(R.id.btnLaunchSimulation)
        
        // Set default values in UI
        editVelocityX.setText("0.01")
        editVelocityY.setText("0.0")
        editVelocityZ.setText("0.03")
        editLaunchAngle.setText("0")
        
        // Launch button click
        btnLaunchSimulation.setOnClickListener {
            launchUnitySimulation()
        }
    }
    
    private fun launchUnitySimulation() {
        // Read values from UI (with fallbacks)
        val vx = editVelocityX.text.toString().toFloatOrNull() ?: 0.01f
        val vy = editVelocityY.text.toString().toFloatOrNull() ?: 0.0f
        val vz = editVelocityZ.text.toString().toFloatOrNull() ?: 0.03f
        val angle = editLaunchAngle.text.toString().toFloatOrNull() ?: 0f
        
        // Validate inputs (optional)
        if (vx == 0f && vy == 0f && vz == 0f) {
            // Show warning: spacecraft will fall into Sun!
            android.widget.Toast.makeText(
                this,
                "Warning: Zero velocity will cause spacecraft to fall into Sun!",
                android.widget.Toast.LENGTH_LONG
            ).show()
        }
        
        // Create Intent for Unity
        val intent = Intent(this, UnityPlayerActivity::class.java)
        
        // Pass parameters via Intent extras
        intent.putExtra("velocityX", vx)
        intent.putExtra("velocityY", vy)
        intent.putExtra("velocityZ", vz)
        intent.putExtra("launchAngle", angle)
        
        // Optional: Pass custom launch position
        // intent.putExtra("launchPosX", 150f)  // Near Earth
        // intent.putExtra("launchPosY", 0f)
        // intent.putExtra("launchPosZ", 0f)
        
        startActivity(intent)
    }
}
```

---

### **XML Layout (activity_main.xml):**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:padding="16dp">

    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Spacecraft Launch Parameters"
        android:textSize="20sp"
        android:textStyle="bold"
        android:layout_marginBottom="20dp"/>

    <!-- Velocity X -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity X (m/s):"
        android:textSize="16sp"/>
    
    <EditText
        android:id="@+id/editVelocityX"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.01"/>

    <!-- Velocity Y -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Y (m/s):"
        android:textSize="16sp"
        android:layout_marginTop="12dp"/>
    
    <EditText
        android:id="@+id/editVelocityY"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.0"/>

    <!-- Velocity Z -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Z (m/s):"
        android:textSize="16sp"
        android:layout_marginTop="12dp"/>
    
    <EditText
        android:id="@+id/editVelocityZ"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.03"/>

    <!-- Launch Angle -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Launch Angle (degrees):"
        android:textSize="16sp"
        android:layout_marginTop="12dp"/>
    
    <EditText
        android:id="@+id/editLaunchAngle"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0"/>

    <!-- Launch Button -->
    <Button
        android:id="@+id/btnLaunchSimulation"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Launch Simulation"
        android:textSize="18sp"
        android:layout_marginTop="30dp"/>

    <!-- Helper Text -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Example values:\n• Earth orbit: X=0.01, Y=0, Z=0.03\n• Mars transfer: X=0.01, Y=0, Z=0.035\n• Escape: X=0.02, Y=0, Z=0.05"
        android:textSize="12sp"
        android:layout_marginTop="20dp"/>
</LinearLayout>
```

---

## 🎮 **How It Works**

### **Workflow:**

```
┌─────────────────────────────────────┐
│  Kotlin App                         │
│  User enters:                       │
│  • Velocity X: 0.015               │
│  • Velocity Y: 0.0                 │
│  • Velocity Z: 0.035               │
│  • Launch Angle: 45°               │
│                                     │
│  [Launch Simulation] ───────────────┼──► Intent.putExtra()
└─────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────┐
│  Unity Scene: Solar System Animated View 3D User│
│                                                  │
│  1. KotlinLaunchReceiver.Start()                │
│     ├─ Read Intent extras                       │
│     ├─ Apply velocity to Rigidbody              │
│     ├─ Set launch angle (rotation)              │
│     └─ Disable conflicting scripts              │
│                                                  │
│  2. EphemerisBasedSimulation                    │
│     ├─ Detects "SmallBody" tag                  │
│     ├─ Reads Rigidbody velocity                 │
│     ├─ Initializes GravitySimulator             │
│     └─ Starts N-body simulation                 │
│                                                  │
│  3. Every FixedUpdate():                        │
│     ├─ Calculate gravity from all planets       │
│     ├─ Update spacecraft position               │
│     └─ Update spacecraft velocity               │
│                                                  │
│  Trajectory unfolds based on:                   │
│  • Initial velocity (from user)                 │
│  • Launch angle (from user)                     │
│  • Real gravity physics                         │
│  • N-body interactions                          │
│                                                  │
│  User clicks [Exit] button ─────────────────────┼──► Application.Unload()
└─────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────┐
│  Back to Kotlin App                 │
│  Change values and try again!       │
└─────────────────────────────────────┘
```

---

## 📊 **Velocity Guidelines**

At Unity scale (1 billion meters = 1 Unity unit):

| Orbit Type | Description | Example Values |
|------------|-------------|----------------|
| **Circular Earth orbit** | Stays near Earth | X=0.01, Y=0, Z=0.03 |
| **Elliptical orbit** | Elongated path | X=0.015, Y=0, Z=0.03 |
| **Mars transfer** | Hohmann transfer | X=0.01, Y=0, Z=0.035 |
| **Escape trajectory** | Leaves solar system | X=0.02, Y=0, Z=0.05 |
| **Radial launch** | Straight toward/away Sun | X=0.03, Y=0, Z=0 |

**Key:**
- **X**: Radial (toward/away from Sun)
- **Y**: Perpendicular to orbit plane
- **Z**: Tangential (along orbit direction)

---

## 🧪 **Testing in Unity Editor**

1. **Press Play** in Unity Editor
2. **Check Console** for:
   ```
   KotlinLaunchReceiver: Running in Editor, using default values
   ✅ Velocity applied: (0.010000, 0.000000, 0.030000)
   🚀 SPACECRAFT LAUNCH INITIALIZED
   ```
3. **Watch spacecraft** follow trajectory based on default values
4. **Adjust default values** in Inspector to test different trajectories

---

## 🐛 **Troubleshooting**

### **Spacecraft falls straight into Sun:**
- ✓ Velocity is too small or zero
- ✓ Increase Z velocity (orbital velocity)
- ✓ Try: vx=0.01, vy=0, vz=0.03

### **Spacecraft flies away immediately:**
- ✓ Velocity is too high
- ✓ Reduce velocity components
- ✓ Try: vx=0.005, vy=0, vz=0.025

### **"No spacecraft found" error:**
- ✓ GameObject must be named "MBR Explorer"
- ✓ Or have tag "SmallBody"
- ✓ Must exist before pressing Play

### **Values from Kotlin not applied:**
- ✓ Check you're building and installing fresh APK
- ✓ Check Intent extras are named correctly
- ✓ Check logcat for error messages

---

## 🎯 **Launch Angle Explanation**

Launch angle rotates the spacecraft around Y-axis:
- **0°** = Facing forward (+Z direction)
- **90°** = Facing right (+X direction)
- **180°** = Facing backward (-Z direction)
- **270°** = Facing left (-X direction)

This affects the **initial direction** but gravity will curve the path!

---

## ✅ **You're Ready!**

Your setup now supports:
- ✅ User input from Kotlin app
- ✅ Custom velocity in 3 dimensions
- ✅ Custom launch angle
- ✅ Real N-body gravity simulation
- ✅ Repeatable with different values

**Try different velocities and watch how gravity affects the trajectory!** 🚀🌌

