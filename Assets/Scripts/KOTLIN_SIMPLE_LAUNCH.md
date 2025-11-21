# 🚀 Kotlin Simple Launch Setup

## ✅ **Super Simple - Based on SpacecraftInitialVelocity**

Just attach one script to MBR Explorer, that's it!

---

## 🎯 **Unity Setup (2 Steps)**

### **Step 1: Select MBR Explorer**
1. In Hierarchy, select **"MBR Explorer"** GameObject

---

### **Step 2: Add Script**
1. In Inspector, click **Add Component**
2. Search for **"SpacecraftKotlinLaunch"**
3. Click to add it

---

### **Step 3: Configure (Optional)**
In Inspector → **SpacecraftKotlinLaunch** component:

- **Default Velocity** (for Editor testing):
  - X: `0.01`
  - Y: `0.0`
  - Z: `0.03`

- **Launch From Earth**: ✅ Checked

- **Debug Mode**: ✅ Checked

**Done!** That's all you need in Unity!

---

## 📱 **Kotlin Code (Ultra Simple)**

```kotlin
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayerActivity

class MainActivity : AppCompatActivity() {
    
    private lateinit var editVelocityX: EditText
    private lateinit var editVelocityY: EditText
    private lateinit var editVelocityZ: EditText
    private lateinit var btnLaunch: Button
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        // Initialize UI
        editVelocityX = findViewById(R.id.editVelocityX)
        editVelocityY = findViewById(R.id.editVelocityY)
        editVelocityZ = findViewById(R.id.editVelocityZ)
        btnLaunch = findViewById(R.id.btnLaunch)
        
        // Default values for Earth orbit
        editVelocityX.setText("0.01")
        editVelocityY.setText("0.0")
        editVelocityZ.setText("0.03")
        
        // Launch button
        btnLaunch.setOnClickListener {
            launchSimulation()
        }
    }
    
    private fun launchSimulation() {
        // Read velocity values (with fallbacks)
        val vx = editVelocityX.text.toString().toFloatOrNull() ?: 0.01f
        val vy = editVelocityY.text.toString().toFloatOrNull() ?: 0.0f
        val vz = editVelocityZ.text.toString().toFloatOrNull() ?: 0.03f
        
        // Create Intent for Unity
        val intent = Intent(this, UnityPlayerActivity::class.java)
        intent.putExtra("velocityX", vx)
        intent.putExtra("velocityY", vy)
        intent.putExtra("velocityZ", vz)
        
        // Launch Unity
        startActivity(intent)
    }
}
```

---

## 🎨 **XML Layout (Minimal)**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:padding="16dp"
    android:gravity="center_vertical">

    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="🚀 Spacecraft Velocity"
        android:textSize="28sp"
        android:textStyle="bold"
        android:layout_marginBottom="30dp"
        android:layout_gravity="center"/>

    <!-- Velocity X -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity X (m/s)"
        android:textSize="18sp"
        android:textStyle="bold"/>
    
    <EditText
        android:id="@+id/editVelocityX"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.01"
        android:textSize="20sp"
        android:padding="12dp"/>

    <!-- Velocity Y -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Y (m/s)"
        android:textSize="18sp"
        android:textStyle="bold"
        android:layout_marginTop="16dp"/>
    
    <EditText
        android:id="@+id/editVelocityY"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.0"
        android:textSize="20sp"
        android:padding="12dp"/>

    <!-- Velocity Z -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Z (m/s)"
        android:textSize="18sp"
        android:textStyle="bold"
        android:layout_marginTop="16dp"/>
    
    <EditText
        android:id="@+id/editVelocityZ"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.03"
        android:textSize="20sp"
        android:padding="12dp"/>

    <!-- Launch Button -->
    <Button
        android:id="@+id/btnLaunch"
        android:layout_width="match_parent"
        android:layout_height="70dp"
        android:text="🚀 LAUNCH"
        android:textSize="22sp"
        android:textStyle="bold"
        android:layout_marginTop="40dp"
        android:backgroundTint="#4CAF50"/>

    <!-- Info -->
    <TextView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Examples:\n\n• Earth orbit: 0.01, 0, 0.03\n• Mars transfer: 0.01, 0, 0.035\n• Escape: 0.02, 0, 0.05"
        android:textSize="16sp"
        android:layout_marginTop="30dp"
        android:padding="16dp"
        android:background="#E8F5E9"
        android:lineSpacingMultiplier="1.4"/>
</LinearLayout>
```

---

## ✅ **What It Does:**

1. **Finds Earth** → Positions spacecraft there
2. **Reads Intent** → Gets vX, vY, vZ from Kotlin
3. **Applies velocity** → Sets Rigidbody.linearVelocity
4. **Rotates spacecraft** → Points in velocity direction
5. **Done!** → Gravity simulation takes over

---

## 🎮 **Testing:**

### **In Unity Editor:**
1. Press Play
2. Console shows:
   ```
   SpacecraftKotlinLaunch: Running in Editor, using default values
   ✓ Spacecraft positioned at Earth: (150.2, 0, 0)
   ✓ Spacecraft initial velocity set to (0.01, 0, 0.03)
   ```
3. Watch spacecraft orbit!

### **On Android:**
1. Enter velocities in Kotlin app
2. Press Launch
3. Unity receives values
4. Spacecraft launches from Earth with your velocities!

---

## 📊 **Quick Examples:**

| Scenario | vX | vY | vZ | Result |
|----------|----|----|-----|--------|
| Earth orbit | 0.01 | 0 | 0.03 | Circular |
| Mars transfer | 0.01 | 0 | 0.035 | Elliptical to Mars |
| High orbit | 0.02 | 0 | 0.03 | Very elongated |
| Escape | 0.02 | 0 | 0.05 | Leaves solar system |
| Polar | 0.01 | 0.02 | 0.03 | Tilted orbit |

---

## 🔧 **Requirements (Check These):**

On **MBR Explorer** GameObject:
- ✅ **Rigidbody** component
- ✅ **Use Gravity**: ☐ UNCHECKED
- ✅ **Is Kinematic**: ☐ UNCHECKED
- ✅ **Tag**: "SmallBody"
- ✅ **SpacecraftKotlinLaunch** script

In **Scene**:
- ✅ **Earth** GameObject (for launch position)
- ✅ **SimulationManager** with EphemerisBasedSimulation

---

**That's it! Simple as SpacecraftInitialVelocity but talks to Kotlin!** 🚀✨

